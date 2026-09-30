/* =====================================================
   HabitacionDAO.cs — Acceso a datos de habitaciones
   Codificación: UTF-8
   ===================================================== */
using System;
using System.Data;
using Entidades;

namespace Datos
{
    public static class HabitacionDAO
    {
        /// <summary>Lista habitaciones con filtro opcional por número o tipo.</summary>
        public static DataTable Listar(string filtro = null)
        {
            return Db.Consultar(
                @"SELECT hb.id_habitacion, hb.numero, hb.piso,
                         t.nombre AS tipo, t.capacidad, t.precio_noche, hb.estado
                  FROM habitacion hb JOIN tipo_habitacion t ON t.id_tipo = hb.id_tipo
                  WHERE (@f = '' OR hb.numero LIKE CONCAT('%',@f,'%')
                               OR t.nombre   LIKE CONCAT('%',@f,'%'))
                  ORDER BY hb.numero",
                Db.P("@f", filtro ?? ""));
        }

        public static DataTable ListarTipos()
        {
            return Db.Consultar(
                @"SELECT id_tipo,
                         CONCAT(nombre,' - Bs ',FORMAT(precio_noche,2),' (cap. ',capacidad,')') AS etiqueta
                  FROM tipo_habitacion ORDER BY precio_noche");
        }

        public static DataRow Obtener(int id)
        {
            DataTable t = Db.Consultar(
                @"SELECT hb.*, t.capacidad, t.precio_noche, t.nombre AS tipo
                  FROM habitacion hb JOIN tipo_habitacion t ON t.id_tipo = hb.id_tipo
                  WHERE hb.id_habitacion=@id", Db.P("@id", id));
            return t.Rows.Count > 0 ? t.Rows[0] : null;
        }

        public static bool ExisteNumero(string numero, int excluirId)
        {
            return Convert.ToInt32(Db.Escalar(
                "SELECT COUNT(*) FROM habitacion WHERE numero=@n AND id_habitacion<>@id",
                Db.P("@n", numero), Db.P("@id", excluirId))) > 0;
        }

        public static int TotalReservas(int id)
        {
            return Convert.ToInt32(Db.Escalar(
                "SELECT COUNT(*) FROM reserva WHERE id_habitacion=@id", Db.P("@id", id)));
        }

        public static DataTable Disponibles(DateTime entrada, DateTime salida, int personas)
        {
            return Db.Consultar(
                @"SELECT hb.id_habitacion,
                         CONCAT('Hab. ',hb.numero,' - ',t.nombre,' - Bs ',FORMAT(t.precio_noche,2),'/noche') AS etiqueta
                  FROM habitacion hb JOIN tipo_habitacion t ON t.id_tipo = hb.id_tipo
                  WHERE hb.estado = 'Disponible' AND t.capacidad >= @p
                    AND NOT EXISTS (
                          SELECT 1 FROM reserva r
                          WHERE r.id_habitacion = hb.id_habitacion
                            AND r.estado <> 'Cancelada'
                            AND r.fecha_entrada < @s AND r.fecha_salida > @e)
                  ORDER BY t.precio_noche, hb.numero",
                Db.P("@p", personas), Db.P("@e", entrada.Date), Db.P("@s", salida.Date));
        }

        public static void Insertar(Habitacion h)
        {
            Db.Ejecutar(
                "INSERT INTO habitacion(numero,piso,id_tipo,estado) VALUES(@n,@p,@t,@e)",
                Db.P("@n", h.Numero), Db.P("@p", h.Piso),
                Db.P("@t", h.IdTipo), Db.P("@e", h.Estado));
        }

        public static void Actualizar(Habitacion h)
        {
            Db.Ejecutar(
                "UPDATE habitacion SET numero=@n, piso=@p, id_tipo=@t, estado=@e WHERE id_habitacion=@id",
                Db.P("@n", h.Numero), Db.P("@p", h.Piso), Db.P("@t", h.IdTipo),
                Db.P("@e", h.Estado), Db.P("@id", h.Id));
        }

        public static void Eliminar(int id)
        {
            Db.Ejecutar("DELETE FROM habitacion WHERE id_habitacion=@id", Db.P("@id", id));
        }
    }
}
