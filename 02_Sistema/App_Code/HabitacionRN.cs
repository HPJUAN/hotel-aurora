using System;
using System.Data;
using Datos;
using Entidades;

namespace RN
{
    public static class HabitacionRN
    {
        public static DataTable Listar(string filtro = null) { return HabitacionDAO.Listar(filtro); }
        public static DataTable ListarTipos() { return HabitacionDAO.ListarTipos(); }
        public static DataRow Obtener(int id) { return HabitacionDAO.Obtener(id); }

        public static DataTable Disponibles(DateTime entrada, DateTime salida, int personas)
        {
            return HabitacionDAO.Disponibles(entrada, salida, personas);
        }

        public static void Guardar(Habitacion h)
        {
            h.Numero = (h.Numero ?? "").Trim();
            if (h.Numero.Length == 0) throw new ApplicationException("El número de habitación es obligatorio.");
            if (h.Piso < 1 || h.Piso > 20) throw new ApplicationException("El piso debe estar entre 1 y 20.");
            if (h.IdTipo <= 0) throw new ApplicationException("Seleccione el tipo de habitación.");
            if (HabitacionDAO.ExisteNumero(h.Numero, h.Id)) throw new ApplicationException("Ya existe una habitación con ese número.");

            if (h.Id == 0) HabitacionDAO.Insertar(h); else HabitacionDAO.Actualizar(h);
        }

        public static void Eliminar(int id)
        {
            if (HabitacionDAO.TotalReservas(id) > 0)
                throw new ApplicationException("No se puede eliminar: la habitación tiene reservas. Póngala en Mantenimiento.");
            HabitacionDAO.Eliminar(id);
        }
    }
}
