/* =====================================================
   Reportes.aspx.cs — Code-behind de la página de reportes
   Codificación: UTF-8
   ===================================================== */
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Datos;

public partial class Reportes : System.Web.UI.Page
{
    protected string JsonMeses     = "[]";
    protected string JsonIngresos  = "[]";
    protected string JsonOcupacion = "[]";
    protected string JsonTipos     = "[]";
    protected string JsonIngTipo   = "[]";

    private static readonly string[] NombresMeses =
    {
        "Enero","Febrero","Marzo","Abril","Mayo","Junio",
        "Julio","Agosto","Septiembre","Octubre","Noviembre","Diciembre"
    };

    // Guardamos la tabla mensual para reutilizarla en la exportación CSV
    private DataTable _tablaMensual;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            // Poblar selector de año: 4 años atrás hasta el siguiente
            int actual = DateTime.Today.Year;
            for (int a = actual - 4; a <= actual + 1; a++)
                ddlAnio.Items.Add(a.ToString());
            ddlAnio.SelectedValue = actual.ToString();
        }
        try { Generar(); }
        catch (Exception ex)
        {
            Ui.Msg(lblMsg,
                "No se pudo generar el reporte. Revisa la conexión a la base de datos. Detalle: " + ex.Message,
                false);
        }
    }

    protected void ddlAnio_SelectedIndexChanged(object sender, EventArgs e) { /* Generar() se llama en Page_Load */ }

    // ---- Lógica central ----

    private void Generar()
    {
        int anio        = int.Parse(ddlAnio.SelectedValue);
        int habitaciones = Math.Max(1, ReporteDAO.TotalOperativas());

        // ---- Reporte 1: mensual ----
        DataTable datos   = ReporteDAO.IngresosMensuales(anio);
        DataTable mensual = new DataTable();
        mensual.Columns.Add("nombre",   typeof(string));
        mensual.Columns.Add("reservas", typeof(int));
        mensual.Columns.Add("noches",   typeof(int));
        mensual.Columns.Add("ocupacion",typeof(double));
        mensual.Columns.Add("ticket",   typeof(decimal));
        mensual.Columns.Add("ingresos", typeof(decimal));

        var ingresos = new List<decimal>();
        var ocup     = new List<double>();
        decimal totalIng = 0;
        int     totalNoches = 0;

        for (int m = 1; m <= 12; m++)
        {
            int reservas = 0, noches = 0; decimal ing = 0;
            DataRow[] f = datos.Select("mes = " + m);
            if (f.Length > 0)
            {
                reservas = Convert.ToInt32(f[0]["reservas"]);
                noches   = Convert.ToInt32(f[0]["noches"]);
                ing      = Convert.ToDecimal(f[0]["ingresos"]);
            }
            double oc = Math.Min(100.0,
                noches * 100.0 / (habitaciones * DateTime.DaysInMonth(anio, m)));
            // Ticket promedio = ingresos / reservas del mes
            decimal ticket = reservas > 0 ? ing / reservas : 0;

            mensual.Rows.Add(NombresMeses[m - 1], reservas, noches,
                             Math.Round(oc, 1), ticket, ing);
            ingresos.Add(ing);
            ocup.Add(Math.Round(oc, 1));
            totalIng    += ing;
            totalNoches += noches;
        }
        _tablaMensual = mensual;
        gvMes.DataSource = mensual;
        gvMes.DataBind();

        // ---- Reporte 2: por tipo y top huéspedes ----
        DataTable tipos = ReporteDAO.PorTipo(anio);
        gvTipo.DataSource = tipos; gvTipo.DataBind();
        gvTop.DataSource  = ReporteDAO.TopHuespedes(anio); gvTop.DataBind();

        var nombres = new List<string>();
        var ingTipo = new List<decimal>();
        foreach (DataRow r in tipos.Rows)
        {
            nombres.Add(r["tipo"].ToString());
            ingTipo.Add(Convert.ToDecimal(r["ingresos"]));
        }

        // ---- KPIs anuales ----
        litTotal.Text  = totalIng.ToString("N0");
        litNoches.Text = totalNoches.ToString("N0");
        litAdr.Text    = (totalNoches > 0 ? totalIng / totalNoches : 0).ToString("N0");

        DataRow c   = ReporteDAO.Cancelaciones(anio).Rows[0];
        double tot  = c["total"]     == DBNull.Value ? 0 : Convert.ToDouble(c["total"]);
        double can  = c["canceladas"]== DBNull.Value ? 0 : Convert.ToDouble(c["canceladas"]);
        litCancel.Text = (tot > 0 ? can * 100 / tot : 0).ToString("0.0");

        // ---- JSON para Chart.js ----
        var js = new JavaScriptSerializer();
        JsonMeses     = js.Serialize(new[] { "Ene","Feb","Mar","Abr","May","Jun",
                                              "Jul","Ago","Sep","Oct","Nov","Dic" });
        JsonIngresos  = js.Serialize(ingresos);
        JsonOcupacion = js.Serialize(ocup);
        JsonTipos     = js.Serialize(nombres);
        JsonIngTipo   = js.Serialize(ingTipo);
    }

    // ---- Exportar CSV ----

    /// <summary>
    /// Genera un archivo CSV con el Reporte 1 (mensual) y lo envía al navegador
    /// como descarga. Sin dependencias externas.
    /// </summary>
    protected void btnExportarCsv_Click(object sender, EventArgs e)
    {
        // Regenerar la tabla si no está en memoria (postback directo)
        if (_tablaMensual == null) Generar();

        int anio = int.Parse(ddlAnio.SelectedValue);
        var sb   = new StringBuilder();
        // BOM UTF-8 para que Excel lo abra correctamente
        sb.Append('\uFEFF');
        // Encabezados
        sb.AppendLine("Mes,Reservas,Noches,Ocupacion %,Ticket Bs,Ingresos Bs");
        // Filas
        foreach (DataRow r in _tablaMensual.Rows)
        {
            sb.AppendFormat("{0},{1},{2},{3:N1},{4:N2},{5:N2}\r\n",
                r["nombre"], r["reservas"], r["noches"],
                r["ocupacion"], r["ticket"], r["ingresos"]);
        }

        Response.Clear();
        Response.ContentType    = "text/csv";
        Response.Charset        = "UTF-8";
        Response.ContentEncoding = Encoding.UTF8;
        Response.AddHeader("Content-Disposition",
            "attachment; filename=reporte_mensual_" + anio + ".csv");
        Response.Write(sb.ToString());
        Response.End();
    }
}
