/* =====================================================
   Reglas.cs — Reglas de negocio PURAS (sin BD ni ASP.NET).
   Se prueban con NUnit en 04_Pruebas/ReglasTests.cs.
   Codificación: UTF-8
   ===================================================== */
using System;
using System.Text.RegularExpressions;

namespace RN
{
    public static class Reglas
    {
        // ---- Cálculo de noches y total ----

        public static int Noches(DateTime entrada, DateTime salida)
        {
            return (salida.Date - entrada.Date).Days;
        }

        public static decimal CalcularTotal(int noches, decimal precioNoche)
        {
            if (noches < 1) throw new ArgumentOutOfRangeException("noches");
            return noches * precioNoche;
        }

        // ---- Validación de fechas ----

        /// <summary>Devuelve mensaje de error o null si las fechas son válidas.</summary>
        public static string ValidarFechas(DateTime entrada, DateTime salida, DateTime hoy)
        {
            if (entrada.Date < hoy.Date)          return "La fecha de entrada no puede ser anterior a hoy.";
            if (salida.Date <= entrada.Date)       return "La fecha de salida debe ser posterior a la de entrada.";
            if (Noches(entrada, salida) > 30)      return "La estadía máxima permitida es de 30 noches.";
            return null;
        }

        // ---- Capacidad ----

        public static bool CapacidadValida(int personas, int capacidad)
        {
            return personas >= 1 && personas <= capacidad;
        }

        // ---- Ciclo de vida de la reserva ----

        /// <summary>Confirmada → CheckIn | Cancelada ; CheckIn → CheckOut.</summary>
        public static bool TransicionValida(string actual, string nuevo)
        {
            if (actual == "Confirmada") return nuevo == "CheckIn" || nuevo == "Cancelada";
            if (actual == "CheckIn")   return nuevo == "CheckOut";
            return false;
        }

        public static bool PuedeHacerCheckIn(DateTime entrada, DateTime hoy)
        {
            return hoy.Date >= entrada.Date;
        }

        // ---- Validación de teléfono ----

        /// <summary>
        /// Valida un número de teléfono (campo opcional).
        /// Acepta dígitos, +, espacios, guiones y paréntesis. Longitud 7-20.
        /// Devuelve null si está vacío o es válido; mensaje de error si no lo es.
        /// </summary>
        public static string ValidarTelefono(string tel)
        {
            if (string.IsNullOrWhiteSpace(tel)) return null; // opcional
            if (!Regex.IsMatch(tel.Trim(), @"^\+?[\d\s\-\(\)]{7,20}$"))
                return "El teléfono debe tener entre 7 y 20 caracteres (dígitos, +, espacios o guiones).";
            return null;
        }

        // ---- Validación de monto de pago ----

        /// <summary>
        /// Valida que un monto de pago sea positivo y no supere el saldo pendiente.
        /// Devuelve null si es válido, o mensaje de error.
        /// </summary>
        public static string ValidarMontoPago(decimal monto, decimal saldoPendiente)
        {
            if (monto <= 0)
                return "El monto del pago debe ser mayor a cero.";
            if (saldoPendiente <= 0)
                return "Esta reserva ya no tiene saldo pendiente.";
            if (monto > saldoPendiente)
                return string.Format("El monto no puede superar el saldo pendiente de Bs {0:N2}.", saldoPendiente);
            return null;
        }
    }
}
