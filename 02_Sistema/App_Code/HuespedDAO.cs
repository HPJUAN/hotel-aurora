using System.Data;
using Entidades;

namespace Datos
{
    public static class HuespedDAO
    {
        public static DataTable Listar(string filtro)
        {
            return Db.Consultar(
                @"SELECT id_huesped, ci, nombres, apellidos, email, telefono, nacionalidad
                  FROM huesped
                  WHERE @f = '' OR ci LIKE CONCAT('%',@f,'%') OR nombres LIKE CONCAT('%',@f,'%')
                        OR apellidos LIKE CONCAT('%',@f,'%')
                  ORDER BY apellidos, nombres", Db.P("@f", filtro ?? ""));
        }

        public static DataTable ListarParaCombo()
        {
            return Db.Consultar("SELECT id_huesped, CONCAT(apellidos,' ',nombres,' - CI ',ci) AS etiqueta FROM huesped ORDER BY apellidos, nombres");
        }

        public static DataRow Obtener(int id)
        {
            DataTable t = Db.Consultar("SELECT * FROM huesped WHERE id_huesped=@id", Db.P("@id", id));
            return t.Rows.Count > 0 ? t.Rows[0] : null;
        }

        public static bool ExisteCi(string ci, int excluirId)
        {
            return System.Convert.ToInt32(Db.Escalar(
                "SELECT COUNT(*) FROM huesped WHERE ci=@ci AND id_huesped<>@id", Db.P("@ci", ci), Db.P("@id", excluirId))) > 0;
        }

        public static int TotalReservas(int id)
        {
            return System.Convert.ToInt32(Db.Escalar("SELECT COUNT(*) FROM reserva WHERE id_huesped=@id", Db.P("@id", id)));
        }

        public static void Insertar(Huesped h)
        {
            Db.Ejecutar(@"INSERT INTO huesped(ci,nombres,apellidos,email,telefono,nacionalidad)
                          VALUES(@ci,@n,@a,@e,@t,@nac)",
                Db.P("@ci", h.Ci), Db.P("@n", h.Nombres), Db.P("@a", h.Apellidos),
                Db.P("@e", h.Email), Db.P("@t", h.Telefono), Db.P("@nac", h.Nacionalidad));
        }

        public static void Actualizar(Huesped h)
        {
            Db.Ejecutar(@"UPDATE huesped SET ci=@ci, nombres=@n, apellidos=@a, email=@e, telefono=@t, nacionalidad=@nac
                          WHERE id_huesped=@id",
                Db.P("@ci", h.Ci), Db.P("@n", h.Nombres), Db.P("@a", h.Apellidos),
                Db.P("@e", h.Email), Db.P("@t", h.Telefono), Db.P("@nac", h.Nacionalidad), Db.P("@id", h.Id));
        }

        public static void Eliminar(int id)
        {
            Db.Ejecutar("DELETE FROM huesped WHERE id_huesped=@id", Db.P("@id", id));
        }
    }
}
