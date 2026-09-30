/* =====================================================
   ReglasTests.cs — Pruebas unitarias NUnit de las reglas de negocio puras.
   Proyecto: Hotel.Pruebas (.NET Framework)
   Codificación: UTF-8
   ===================================================== */
using System;
using NUnit.Framework;
using RN;

namespace Hotel.Pruebas
{
    [TestFixture]
    public class ReglasTests
    {
        private static readonly DateTime Hoy = new DateTime(2026, 9, 28);

        // ============================================================
        // 1. NOCHES Y TOTAL
        // ============================================================

        [Test]
        public void Noches_CuentaDiasEntreEntradaYSalida()
        {
            Assert.AreEqual(3, Reglas.Noches(new DateTime(2026, 10, 1), new DateTime(2026, 10, 4)));
        }

        [Test]
        public void CalcularTotal_MultiplicaNochesPorPrecio()
        {
            Assert.AreEqual(1260m, Reglas.CalcularTotal(3, 420m));
        }

        [Test]
        public void CalcularTotal_SinNoches_LanzaExcepcion()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Reglas.CalcularTotal(0, 420m));
        }

        // ============================================================
        // 2. VALIDACIÓN DE FECHAS
        // ============================================================

        [Test]
        public void ValidarFechas_Correctas_DevuelveNull()
        {
            Assert.IsNull(Reglas.ValidarFechas(Hoy, Hoy.AddDays(2), Hoy));
        }

        [Test]
        public void ValidarFechas_EntradaPasada_DevuelveError()
        {
            Assert.IsNotNull(Reglas.ValidarFechas(Hoy.AddDays(-1), Hoy.AddDays(2), Hoy));
        }

        [Test]
        public void ValidarFechas_SalidaIgualAEntrada_DevuelveError()
        {
            Assert.IsNotNull(Reglas.ValidarFechas(Hoy, Hoy, Hoy));
        }

        /// <summary>salida estrictamente menor a entrada también debe fallar.</summary>
        [Test]
        public void ValidarFechas_SalidaMenorAEntrada_DevuelveError()
        {
            Assert.IsNotNull(Reglas.ValidarFechas(Hoy.AddDays(3), Hoy.AddDays(1), Hoy));
        }

        /// <summary>Exactamente 30 noches es el límite permitido → debe ser válido.</summary>
        [Test]
        public void ValidarFechas_Exactamente30Noches_DevuelveNull()
        {
            Assert.IsNull(Reglas.ValidarFechas(Hoy, Hoy.AddDays(30), Hoy));
        }

        [Test]
        public void ValidarFechas_MasDe30Noches_DevuelveError()
        {
            Assert.IsNotNull(Reglas.ValidarFechas(Hoy, Hoy.AddDays(31), Hoy));
        }

        // ============================================================
        // 3. CAPACIDAD
        // ============================================================

        [TestCase(1, 2, true)]
        [TestCase(2, 2, true)]
        [TestCase(3, 2, false)]
        [TestCase(0, 2, false)]
        public void CapacidadValida_RespetaElMaximoDelTipo(int personas, int capacidad, bool esperado)
        {
            Assert.AreEqual(esperado, Reglas.CapacidadValida(personas, capacidad));
        }

        // ============================================================
        // 4. ESTADOS DE LA RESERVA
        // ============================================================

        [TestCase("Confirmada", "CheckIn",   true)]
        [TestCase("Confirmada", "Cancelada", true)]
        [TestCase("Confirmada", "CheckOut",  false)]
        [TestCase("CheckIn",    "CheckOut",  true)]
        [TestCase("CheckIn",    "Cancelada", false)]
        [TestCase("CheckOut",   "CheckIn",   false)]
        [TestCase("CheckOut",   "Cancelada", false)]   // ← caso faltante
        [TestCase("Cancelada",  "Confirmada",false)]
        public void TransicionValida_SigueElCicloDeVida(string actual, string nuevo, bool esperado)
        {
            Assert.AreEqual(esperado, Reglas.TransicionValida(actual, nuevo));
        }

        [Test]
        public void PuedeHacerCheckIn_SoloDesdeLaFechaDeEntrada()
        {
            Assert.IsFalse(Reglas.PuedeHacerCheckIn(Hoy.AddDays(1), Hoy));
            Assert.IsTrue (Reglas.PuedeHacerCheckIn(Hoy, Hoy));
        }

        // ============================================================
        // 5. VALIDACIÓN DE TELÉFONO (nueva — Entrega 4)
        // ============================================================

        [TestCase(null,             null)]    // campo opcional, vacío OK
        [TestCase("",              null)]
        [TestCase("  ",            null)]
        [TestCase("70123456",      null)]     // 8 dígitos, válido
        [TestCase("+591 70123456", null)]     // formato internacional
        [TestCase("591-70123456",  null)]     // con guión
        public void ValidarTelefono_ValoresValidos_DevuelveNull(string tel, object esperadoNull)
        {
            Assert.IsNull(Reglas.ValidarTelefono(tel));
        }

        [TestCase("123")]             // muy corto (< 7 chars)
        [TestCase("abc-1234567")]     // letras no permitidas
        [TestCase("12345678901234567890123")] // demasiado largo (> 20)
        public void ValidarTelefono_ValoresInvalidos_DevuelveMensaje(string tel)
        {
            Assert.IsNotNull(Reglas.ValidarTelefono(tel));
        }

        // ============================================================
        // 6. VALIDACIÓN DE MONTO DE PAGO (nueva — Entrega 3)
        // ============================================================

        [Test]
        public void ValidarMontoPago_MontoValido_DevuelveNull()
        {
            Assert.IsNull(Reglas.ValidarMontoPago(100m, 500m));
        }

        [Test]
        public void ValidarMontoPago_MontoCero_DevuelveError()
        {
            Assert.IsNotNull(Reglas.ValidarMontoPago(0m, 500m));
        }

        [Test]
        public void ValidarMontoPago_MontoNegativo_DevuelveError()
        {
            Assert.IsNotNull(Reglas.ValidarMontoPago(-50m, 500m));
        }

        [Test]
        public void ValidarMontoPago_MontoSuperaSaldo_DevuelveError()
        {
            Assert.IsNotNull(Reglas.ValidarMontoPago(600m, 500m));
        }

        [Test]
        public void ValidarMontoPago_MontoExactoIgualSaldo_DevuelveNull()
        {
            // Pago exacto del saldo pendiente: es válido
            Assert.IsNull(Reglas.ValidarMontoPago(500m, 500m));
        }

        [Test]
        public void ValidarMontoPago_SaldoCero_DevuelveError()
        {
            // Reserva ya pagada completamente
            Assert.IsNotNull(Reglas.ValidarMontoPago(1m, 0m));
        }
    }
}
