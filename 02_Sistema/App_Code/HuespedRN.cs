/* =====================================================
   HuespedRN.cs — Reglas de negocio de huéspedes
   Codificación: UTF-8
   ===================================================== */
using System;
using System.Data;
using Datos;
using Entidades;

namespace RN
{
    public static class HuespedRN
    {
        public static DataTable Listar(string filtro)     { return HuespedDAO.Listar(filtro); }
        public static DataTable ListarParaCombo()          { return HuespedDAO.ListarParaCombo(); }
        public static DataRow   Obtener(int id)            { return HuespedDAO.Obtener(id); }

        /// <summary>
        /// Guarda (inserta o actualiza) un huésped.
        /// Valida CI, nombres, apellidos, email y teléfono antes de persistir.
        /// </summary>
        public static void Guardar(Huesped h)
        {
            h.Ci          = (h.Ci          ?? "").Trim();
            h.Nombres     = (h.Nombres     ?? "").Trim();
            h.Apellidos   = (h.Apellidos   ?? "").Trim();
            h.Email       = (h.Email       ?? "").Trim();
            h.Telefono    = (h.Telefono    ?? "").Trim();
            h.Nacionalidad= (h.Nacionalidad ?? "").Trim();

            if (h.Ci.Length < 5)
                throw new ApplicationException("El documento de identidad debe tener al menos 5 caracteres.");
            if (h.Nombres.Length == 0 || h.Apellidos.Length == 0)
                throw new ApplicationException("Nombres y apellidos son obligatorios.");

            // Validar email (si se proporcionó)
            if (!string.IsNullOrEmpty(h.Email))
            {
                try { new System.Net.Mail.MailAddress(h.Email); }
                catch { throw new ApplicationException("El correo electrónico no tiene un formato válido."); }
            }

            // Validar teléfono (regla pura en Reglas.cs → testeable con NUnit)
            string errTel = Reglas.ValidarTelefono(h.Telefono);
            if (errTel != null) throw new ApplicationException(errTel);

            if (string.IsNullOrWhiteSpace(h.Nacionalidad)) h.Nacionalidad = "Boliviana";

            if (HuespedDAO.ExisteCi(h.Ci, h.Id))
                throw new ApplicationException("Ya existe un huésped registrado con ese documento.");

            if (h.Id == 0) HuespedDAO.Insertar(h); else HuespedDAO.Actualizar(h);
        }

        public static void Eliminar(int id)
        {
            if (HuespedDAO.TotalReservas(id) > 0)
                throw new ApplicationException("No se puede eliminar: el huésped tiene reservas registradas.");
            HuespedDAO.Eliminar(id);
        }
    }
}
