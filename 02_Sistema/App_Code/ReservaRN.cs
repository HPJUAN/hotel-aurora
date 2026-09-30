/* =====================================================
   ReservaRN.cs — Reglas de negocio de reservas y pagos
   Codificación: UTF-8
   ===================================================== */
using System;
using System.Data;
using Datos;
using Entidades;

namespace RN
{
    public static class ReservaRN
    {
        // ---- Consultas ----

        public static DataTable Listar(string estado, string filtro,
                                       DateTime? desde = null, DateTime? hasta = null)
        {
            return ReservaDAO.Listar(estado, filtro, desde, hasta);
        }

        public static DataTable BuscarDisponibles(DateTime entrada, DateTime salida, int personas)
        {
            string error = Reglas.ValidarFechas(entrada, salida, DateTime.Today);
            if (error != null) throw new ApplicationException(error);
            if (personas < 1) throw new ApplicationException("Indique al menos 1 persona.");
            return HabitacionRN.Disponibles(entrada, salida, personas);
        }

        public static decimal Calcular(int idHabitacion, DateTime entrada, DateTime salida)
        {
            DataRow hab = HabitacionDAO.Obtener(idHabitacion);
            if (hab == null) throw new ApplicationException("La habitación seleccionada no existe.");
            return Reglas.CalcularTotal(Reglas.Noches(entrada, salida), Convert.ToDecimal(hab["precio_noche"]));
        }

        public static decimal SaldoPendiente(int idReserva)
        {
            return ReservaDAO.SaldoPendiente(idReserva);
        }

        // ---- Crear reserva ----

        public static int Crear(int idHuesped, int idHabitacion,
                                DateTime entrada, DateTime salida,
                                int personas, string obs)
        {
            string error = Reglas.ValidarFechas(entrada, salida, DateTime.Today);
            if (error != null) throw new ApplicationException(error);
            if (idHuesped <= 0) throw new ApplicationException("Seleccione un huésped.");

            DataRow hab = HabitacionDAO.Obtener(idHabitacion);
            if (hab == null) throw new ApplicationException("Seleccione una habitación disponible.");
            if (hab["estado"].ToString() != "Disponible")
                throw new ApplicationException("La habitación está en mantenimiento.");
            if (!Reglas.CapacidadValida(personas, Convert.ToInt32(hab["capacidad"])))
                throw new ApplicationException("La habitación admite máximo " + hab["capacidad"] + " persona(s).");

            Reserva r = new Reserva
            {
                IdHuesped     = idHuesped,
                IdHabitacion  = idHabitacion,
                Entrada       = entrada.Date,
                Salida        = salida.Date,
                Personas      = personas,
                Observaciones = string.IsNullOrWhiteSpace(obs) ? null : obs.Trim(),
                Total         = Reglas.CalcularTotal(Reglas.Noches(entrada, salida),
                                                     Convert.ToDecimal(hab["precio_noche"]))
            };

            int id = ReservaDAO.InsertarSiDisponible(r);
            if (id == 0)
                throw new ApplicationException("Lo sentimos, la habitación acaba de ser reservada para esas fechas.");
            return id;
        }

        // ---- Cambiar estado (CheckIn / CheckOut / Cancelar) ----

        public static void CambiarEstado(int idReserva, string nuevo)
        {
            DataRow r = ReservaDAO.Obtener(idReserva);
            if (r == null) throw new ApplicationException("La reserva no existe.");
            string actual = r["estado"].ToString();
            if (!Reglas.TransicionValida(actual, nuevo))
                throw new ApplicationException("No se puede pasar de '" + actual + "' a '" + nuevo + "'.");
            if (nuevo == "CheckIn" && !Reglas.PuedeHacerCheckIn(Convert.ToDateTime(r["fecha_entrada"]), DateTime.Today))
                throw new ApplicationException("El check-in solo puede hacerse desde la fecha de entrada.");
            if (!ReservaDAO.CambiarEstado(idReserva, actual, nuevo))
                throw new ApplicationException("La reserva fue modificada por otro usuario. Actualice la lista.");
        }

        // ---- Registrar pago ----

        /// <summary>
        /// Registra un pago parcial o total para una reserva activa.
        /// Valida que el monto sea positivo y no supere el saldo pendiente.
        /// </summary>
        public static void PagarReserva(int idReserva, decimal monto, string metodo)
        {
            decimal saldo = ReservaDAO.SaldoPendiente(idReserva);
            string error  = Reglas.ValidarMontoPago(monto, saldo);
            if (error != null) throw new ApplicationException(error);

            Pago p = new Pago
            {
                IdReserva = idReserva,
                Monto     = monto,
                Metodo    = metodo,
                FechaPago = DateTime.Now
            };
            ReservaDAO.RegistrarPago(p);
        }
    }
}
