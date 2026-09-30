/* =====================================================
   Default.aspx.cs — Code-behind del tablero de inicio
   Codificación: UTF-8
   ===================================================== */
using System;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Datos;

public partial class _Default : System.Web.UI.Page
{
    protected string JsonMeses    = "[]";
    protected string JsonIngresos = "[]";

    protected void Page_Load(object sender, EventArgs e)
    {
        // Cada bloque tiene su propio try/catch para no ocultar todos los datos si uno falla
        CargarIndicadores();
        CargarLlegadas();
        CargarGrafico();
    }

    private void CargarIndicadores()
    {
        try
        {
            int operativas = ReporteDAO.TotalOperativas();  // habitaciones no en mantenimiento
            int ocupadas   = ReporteDAO.OcupadasHoy();      // con reserva activa hoy
            int libres     = Math.Max(0, operativas - ocupadas);

            litOcupacion.Text = operativas > 0
                ? Math.Min(100, (int)Math.Round(ocupadas * 100.0 / operativas)).ToString()
                : "0";
            litLibres.Text   = libres.ToString();
            litActivas.Text  = ReporteDAO.ReservasActivas().ToString();
            litIngresos.Text = ReporteDAO.IngresosMesActual().ToString("N0");
        }
        catch (Exception ex)
        {
            Ui.Msg(lblMsg,
                "No se pudieron cargar los indicadores. Revisa la conexión a la base de datos. Detalle: " + ex.Message,
                false);
        }
    }

    private void CargarLlegadas()
    {
        try
        {
            gvLlegadas.DataSource = ReporteDAO.ProximasLlegadas();
            gvLlegadas.DataBind();
            litRack.Text = ConstruirTablero(ReporteDAO.EstadoHoy());
        }
        catch (Exception ex)
        {
            litRack.Text = "<p class='text-secondary small'>Error al cargar el tablero: " +
                           HttpUtility.HtmlEncode(ex.Message) + "</p>";
        }
    }

    private void CargarGrafico()
    {
        try
        {
            int anio = DateTime.Today.Year;
            litAnio.Text = anio.ToString();

            string[] nombres = { "Ene", "Feb", "Mar", "Abr", "May", "Jun",
                                  "Jul", "Ago", "Sep", "Oct", "Nov", "Dic" };
            decimal[] valores = new decimal[12];

            foreach (DataRow r in ReporteDAO.IngresosMensuales(anio).Rows)
                valores[Convert.ToInt32(r["mes"]) - 1] = Convert.ToDecimal(r["ingresos"]);

            JavaScriptSerializer js = new JavaScriptSerializer();
            JsonMeses    = js.Serialize(nombres);
            JsonIngresos = js.Serialize(valores);
        }
        catch
        {
            /* Si el gráfico falla, la página igual se muestra */
        }
    }

    /// <summary>
    /// Construye el HTML del tablero de llaves agrupando habitaciones por piso.
    /// Si la habitación está ocupada, muestra el nombre del huésped como tooltip.
    /// </summary>
    private static string ConstruirTablero(DataTable t)
    {
        var sb = new StringBuilder();
        int pisoActual = -1;

        foreach (DataRow r in t.Rows)
        {
            int piso = Convert.ToInt32(r["piso"]);
            if (piso != pisoActual)
            {
                if (pisoActual != -1) sb.Append("</div></div>");
                sb.Append("<div class='floor'><div class='floor-l'>Piso " + piso + "</div><div class='keys'>");
                pisoActual = piso;
            }

            string estado  = r["estado"].ToString();
            bool   ocupada = Convert.ToInt32(r["ocupada"]) == 1;
            string clase   = estado == "Mantenimiento" ? "mant" : (ocupada ? "ocupada" : "libre");
            string numero  = HttpUtility.HtmlEncode(r["numero"].ToString());
            string tipo    = HttpUtility.HtmlEncode(r["tipo"].ToString());

            // Tooltip con el huésped activo (si hay)
            string tooltip = "";
            if (ocupada && r["huesped_nombre"] != DBNull.Value)
                tooltip = " title=\"" + HttpUtility.HtmlAttributeEncode(r["huesped_nombre"].ToString()) + "\"";

            sb.Append("<div class='key " + clase + "'" + tooltip + ">" +
                      "<b>" + numero + "</b>" +
                      "<span>" + tipo + "</span>" +
                      "</div>");
        }

        if (pisoActual != -1) sb.Append("</div></div>");

        if (sb.Length == 0)
            sb.Append("<div class='empty'><i class='bi bi-building'></i><p>No hay habitaciones registradas.</p></div>");

        return sb.ToString();
    }
}
