/* =====================================================
   ReservaDAO.cs — Acceso a datos de reservas y pagos
   Todas las consultas usan parámetros (evita inyección SQL).
   Codificación: UTF-8
   ===================================================== */
using System;
using System.Data;
using Entidades;
using MySql.Data.MySqlClient;

namespace Datos
{
    public static class ReservaDAO
    {
        // ---- Consultas de reservas ----

        /// <summary>
        /// Lista reservas con filtros opcionales de estado, texto libre y rango de fechas.
        /// Las fechas nulas se ignoran.
        /// </summary>
        public static DataTable Listar(string estado, string filtro,
                                       DateTime? desde = null, DateTime? hasta = null)
        {
            return Db.Consultar(
                @"SELECT * FROM vw_reservas_detalle
                  WHERE (@e  = ''   OR estado = @e)
                    AND (@f  = ''   OR ci LIKE CONCAT('%',@f,'%')
                                   OR huesped LIKE CONCAT('%',@f,'%')
                                   OR numero = @f)
                    AND (@d IS NULL OR fecha_entrada >= @d)
                    AND (@h IS NULL OR fecha_entrada <= @h)
                  ORDER BY fecha_entrada DESC, id_reserva DESC
                  LIMIT 200",
                Db.P("@e", estado ?? ""),
                Db.P("@f", filtro  ?? ""),
                Db.P("@d", (object)desde  ?? DBNull.Value),
                Db.P("@h", (object)hasta  ?? DBNull.Value));
        }

        public static DataRow Obtener(int id)
        {
            DataTable t = Db.Consultar(
                "SELECT * FROM reserva WHERE id_reserva=@id", Db.P("@id", id));
            return t.Rows.Count > 0 ? t.Rows[0] : null;
        }

        /// <summary>
        /// Inserta la reserva SOLO si la habitación sigue libre. Bloquea la fila de la habitación
        /// (SELECT ... FOR UPDATE) dentro de una transacción para evitar reservas dobles simultáneas.
        /// Devuelve el id generado, o 0 si ya no estaba disponible.
        /// </summary>
        public static int InsertarSiDisponible(Reserva r)
        {
            using (MySqlConnection c = Db.Abrir())
            using (MySqlTransaction tx = c.BeginTransaction())
            {
                try
                {
                    // Bloqueo pesimista de la fila
                    MySqlCommand bloqueo = new MySqlCommand(
                        "SELECT id_habitacion FROM habitacion WHERE id_habitacion=@h FOR UPDATE", c, tx);
                    bloqueo.Parameters.AddWithValue("@h", r.IdHabitacion);
                    bloqueo.ExecuteScalar();

                    // Verificar solapamiento de fechas
                    MySqlCommand chk = new MySqlCommand(
                        @"SELECT COUNT(*) FROM reserva
                          WHERE id_habitacion=@h AND estado<>'Cancelada'
                            AND fecha_entrada < @s AND fecha_salida > @e", c, tx);
                    chk.Parameters.AddWithValue("@h", r.IdHabitacion);
                    chk.Parameters.AddWithValue("@e", r.Entrada.Date);
                    chk.Parameters.AddWithValue("@s", r.Salida.Date);
                    if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                    {
                        tx.Rollback();
                        return 0; // habitación ya reservada
                    }

                    // Insertar
                    MySqlCommand ins = new MySqlCommand(
                        @"INSERT INTO reserva
                            (id_huesped,id_habitacion,fecha_entrada,fecha_salida,
                             personas,total,estado,observaciones)
                          VALUES(@hu,@h,@e,@s,@p,@t,'Confirmada',@o)", c, tx);
                    ins.Parameters.AddWithValue("@hu", r.IdHuesped);
                    ins.Parameters.AddWithValue("@h",  r.IdHabitacion);
                    ins.Parameters.AddWithValue("@e",  r.Entrada.Date);
                    ins.Parameters.AddWithValue("@s",  r.Salida.Date);
                    ins.Parameters.AddWithValue("@p",  r.Personas);
                    ins.Parameters.AddWithValue("@t",  r.Total);
                    ins.Parameters.AddWithValue("@o",
                        string.IsNullOrEmpty(r.Observaciones) ? (object)DBNull.Value : r.Observaciones);
                    ins.ExecuteNonQuery();
                    long id = ins.LastInsertedId;
                    tx.Commit();
                    return (int)id;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>Cambia el estado solo si el estado actual coincide (evita cambios concurrentes).</summary>
        public static bool CambiarEstado(int id, string actual, string nuevo)
        {
            return Db.Ejecutar(
                "UPDATE reserva SET estado=@n WHERE id_reserva=@id AND estado=@a",
                Db.P("@n", nuevo), Db.P("@id", id), Db.P("@a", actual)) > 0;
        }

        // ---- Pagos ----

        /// <summary>Registra un pago en la tabla pago.</summary>
        public static void RegistrarPago(Pago p)
        {
            Db.Ejecutar(
                @"INSERT INTO pago(id_reserva, monto, metodo, fecha_pago)
                  VALUES(@r, @m, @met, @f)",
                Db.P("@r",   p.IdReserva),
                Db.P("@m",   p.Monto),
                Db.P("@met", p.Metodo),
                Db.P("@f",   p.FechaPago));
        }

        /// <summary>
        /// Devuelve el saldo pendiente de una reserva (total − suma de pagos ya registrados).
        /// </summary>
        public static decimal SaldoPendiente(int idReserva)
        {
            object total  = Db.Escalar(
                "SELECT total FROM reserva WHERE id_reserva=@id", Db.P("@id", idReserva));
            object pagado = Db.Escalar(
                "SELECT COALESCE(SUM(monto),0) FROM pago WHERE id_reserva=@id", Db.P("@id", idReserva));
            if (total == null || total == DBNull.Value) return 0;
            return Convert.ToDecimal(total) - Convert.ToDecimal(pagado);
        }

        /// <summary>Lista los pagos de una reserva ordenados por fecha.</summary>
        public static DataTable ListarPagos(int idReserva)
        {
            return Db.Consultar(
                @"SELECT id_pago, monto, metodo, fecha_pago
                  FROM pago WHERE id_reserva=@id ORDER BY fecha_pago",
                Db.P("@id", idReserva));
        }
    }
}
