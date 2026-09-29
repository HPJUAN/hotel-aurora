using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace Datos
{
    /// <summary>Acceso a MySQL. Toda consulta usa parámetros (evita inyección SQL).</summary>
    public static class Db
    {
        public static MySqlConnection Abrir()
        {
            MySqlConnection c = new MySqlConnection(ConfigurationManager.ConnectionStrings["HotelDb"].ConnectionString);
            c.Open();
            return c;
        }

        public static MySqlParameter P(string nombre, object valor)
        {
            return new MySqlParameter(nombre, valor ?? DBNull.Value);
        }

        public static DataTable Consultar(string sql, params MySqlParameter[] ps)
        {
            using (MySqlConnection c = Abrir())
            using (MySqlCommand cmd = new MySqlCommand(sql, c))
            {
                cmd.Parameters.AddRange(ps);
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataTable t = new DataTable();
                    da.Fill(t);
                    return t;
                }
            }
        }

        public static object Escalar(string sql, params MySqlParameter[] ps)
        {
            using (MySqlConnection c = Abrir())
            using (MySqlCommand cmd = new MySqlCommand(sql, c))
            {
                cmd.Parameters.AddRange(ps);
                return cmd.ExecuteScalar();
            }
        }

        public static int Ejecutar(string sql, params MySqlParameter[] ps)
        {
            using (MySqlConnection c = Abrir())
            using (MySqlCommand cmd = new MySqlCommand(sql, c))
            {
                cmd.Parameters.AddRange(ps);
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
