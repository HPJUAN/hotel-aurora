/* =====================================================
   ReporteDAO.cs — Consultas de reportes e indicadores
   Codificación: UTF-8
   ===================================================== */
using System;
using System.Data;

namespace Datos
{
    public static class ReporteDAO
    {
        // ---- Indicadores del tablero ----

        /// <summary>
        /// Total de habitaciones operativas (excluye Mantenimiento).
        /// Se usa como denominador para calcular % de ocupación.
        /// </summary>
        public static int TotalOperativas()
        {
            return Convert.ToInt32(Db.Escalar(
                "SELECT COUNT(*) FROM habitacion WHERE estado <> 'Mantenimiento'"));
        }

        /// <summary>Habitaciones disponibles (estado = 'Disponible') sin reserva activa hoy.</summary>
        public static int TotalHabitaciones()
        {
            return Convert.ToInt32(Db.Escalar(
                "SELECT COUNT(*) FROM habitacion WHERE estado = 'Disponible'"));
        }

        /// <summary>Habitaciones con reserva activa hoy (CheckIn o Confirmada con fechas que cubren hoy).</summary>
        public static int OcupadasHoy()
        {
            return Convert.ToInt32(Db.Escalar(
                @"SELECT COUNT(DISTINCT id_habitacion) FROM reserva
                  WHERE estado <> 'Cancelada'
                    AND fecha_entrada <= CURDATE() AND fecha_salida > CURDATE()"));
        }

        public static int ReservasActivas()
        {
            return Convert.ToInt32(Db.Escalar(
                "SELECT COUNT(*) FROM reserva WHERE estado IN ('Confirmada','CheckIn')"));
        }

        public static decimal IngresosMesActual()
        {
            object o = Db.Escalar(
                @"SELECT COALESCE(SUM(total),0) FROM reserva WHERE estado <> 'Cancelada'
                  AND YEAR(fecha_entrada)=YEAR(CURDATE()) AND MONTH(fecha_entrada)=MONTH(CURDATE())");
            return Convert.ToDecimal(o);
        }

        // ---- Tablero de llaves ----

        /// <summary>
        /// Estado de cada habitación hoy, incluyendo el nombre del huésped activo (si existe).
        /// Se usa para construir el rack de llaves del inicio.
        /// </summary>
        public static DataTable EstadoHoy()
        {
            return Db.Consultar(
                @"SELECT hb.numero, hb.piso, t.nombre AS tipo, hb.estado,
                         EXISTS(SELECT 1 FROM reserva r
                                WHERE r.id_habitacion = hb.id_habitacion
                                  AND r.estado <> 'Cancelada'
                                  AND r.fecha_entrada <= CURDATE()
                                  AND r.fecha_salida > CURDATE()) AS ocupada,
                         (SELECT CONCAT(hu.nombres,' ',hu.apellidos)
                          FROM reserva r JOIN huesped hu ON hu.id_huesped = r.id_huesped
                          WHERE r.id_habitacion = hb.id_habitacion
                            AND r.estado <> 'Cancelada'
                            AND r.fecha_entrada <= CURDATE()
                            AND r.fecha_salida  > CURDATE()
                          LIMIT 1) AS huesped_nombre
                  FROM habitacion hb JOIN tipo_habitacion t ON t.id_tipo = hb.id_tipo
                  ORDER BY hb.piso DESC, hb.numero");
        }

        // ---- Próximas llegadas ----

        public static DataTable ProximasLlegadas()
        {
            return Db.Consultar(
                @"SELECT huesped, numero, tipo, fecha_entrada, noches, personas
                  FROM vw_reservas_detalle
                  WHERE estado = 'Confirmada' AND fecha_entrada >= CURDATE()
                  ORDER BY fecha_entrada, id_reserva
                  LIMIT 8");
        }

        // ---- Reporte 1: ingresos y ocupación mensuales ----

        /// <summary>Reservas, noches e ingresos por mes de un año (excluye canceladas).</summary>
        public static DataTable IngresosMensuales(int anio)
        {
            return Db.Consultar(
                @"SELECT MONTH(fecha_entrada) AS mes,
                         COUNT(*)          AS reservas,
                         SUM(noches)       AS noches,
                         SUM(total)        AS ingresos
                  FROM vw_reservas_detalle
                  WHERE estado <> 'Cancelada' AND YEAR(fecha_entrada) = @a
                  GROUP BY MONTH(fecha_entrada)
                  ORDER BY mes",
                Db.P("@a", anio));
        }

        // ---- Reporte 2: rendimiento por tipo de habitación ----

        public static DataTable PorTipo(int anio)
        {
            return Db.Consultar(
                @"SELECT tipo,
                         COUNT(*) AS reservas,
                         SUM(noches) AS noches,
                         SUM(total)  AS ingresos
                  FROM vw_reservas_detalle
                  WHERE estado <> 'Cancelada' AND YEAR(fecha_entrada) = @a
                  GROUP BY tipo
                  ORDER BY ingresos DESC",
                Db.P("@a", anio));
        }

        // ---- Ranking de huéspedes ----

        public static DataTable TopHuespedes(int anio)
        {
            return Db.Consultar(
                @"SELECT huesped,
                         COUNT(*) AS reservas,
                         SUM(noches) AS noches,
                         SUM(total)  AS gastado
                  FROM vw_reservas_detalle
                  WHERE estado <> 'Cancelada' AND YEAR(fecha_entrada) = @a
                  GROUP BY id_huesped, huesped
                  ORDER BY gastado DESC
                  LIMIT 5",
                Db.P("@a", anio));
        }

        // ---- Cancelaciones ----

        public static DataTable Cancelaciones(int anio)
        {
            return Db.Consultar(
                @"SELECT SUM(estado='Cancelada') AS canceladas, COUNT(*) AS total
                  FROM reserva WHERE YEAR(fecha_entrada) = @a",
                Db.P("@a", anio));
        }
    }
}
