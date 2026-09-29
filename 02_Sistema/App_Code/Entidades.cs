/* =====================================================
   Entidades.cs — Clases de dominio del sistema
   Codificación: UTF-8
   ===================================================== */
using System;

namespace Entidades
{
    public class Huesped
    {
        public int    Id          { get; set; }
        public string Ci          { get; set; }
        public string Nombres     { get; set; }
        public string Apellidos   { get; set; }
        public string Email       { get; set; }
        public string Telefono    { get; set; }
        public string Nacionalidad{ get; set; }
    }

    public class Habitacion
    {
        public int    Id     { get; set; }
        public string Numero { get; set; }
        public int    Piso   { get; set; }
        public int    IdTipo { get; set; }
        public string Estado { get; set; }
    }

    public class Reserva
    {
        public int      Id           { get; set; }
        public int      IdHuesped    { get; set; }
        public int      IdHabitacion { get; set; }
        public DateTime Entrada      { get; set; }
        public DateTime Salida       { get; set; }
        public int      Personas     { get; set; }
        public decimal  Total        { get; set; }
        public string   Estado       { get; set; }
        public string   Observaciones{ get; set; }
    }

    /// <summary>Registro de un pago vinculado a una reserva.</summary>
    public class Pago
    {
        public int      Id         { get; set; }
        public int      IdReserva  { get; set; }
        public decimal  Monto      { get; set; }
        /// <summary>Efectivo | Tarjeta | Transferencia | QR</summary>
        public string   Metodo     { get; set; }
        public DateTime FechaPago  { get; set; }
    }
}
