/* =====================================================
   Reservas.aspx.cs — Code-behind de la página de reservas
   Codificación: UTF-8
   ===================================================== */
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using RN;

public partial class Reservas : System.Web.UI.Page
{
    /// <summary>
    /// Lista de huéspedes serializada como JSON para el autocompletado del cliente.
    /// Se recarga en cada petición (GET y postback) para que el JS siempre la tenga.
    /// </summary>
    protected string JsonHuespedes = "[]";

    protected void Page_Load(object sender, EventArgs e)
    {
        // Siempre cargar la lista de huéspedes para el autocompletado JS
        CargarJsonHuespedes();

        if (IsPostBack) return;

        // Fechas por defecto: hoy / mañana
        DateTime hoy = DateTime.Today;
        txtEntrada.Text = hoy.ToString("yyyy-MM-dd");
        txtSalida.Text  = hoy.AddDays(1).ToString("yyyy-MM-dd");
        Cargar();
    }

    /// <summary>
    /// Serializa todos los huéspedes como [{id:"1", label:"Apellido Nombre - CI 123"}]
    /// para que el autocompletado en el cliente funcione sin postback.
    /// </summary>
    private void CargarJsonHuespedes()
    {
        try
        {
            var lista = new List<object>();
            foreach (DataRow r in HuespedRN.ListarParaCombo().Rows)
                lista.Add(new { id = r["id_huesped"].ToString(), label = r["etiqueta"].ToString() });
            JsonHuespedes = new JavaScriptSerializer().Serialize(lista);
        }
        catch { /* Si falla, el autocompletado queda vacío pero la página no rompe */ }
    }

    // ---- Carga del grid ----

    private void Cargar()
    {
        DateTime? desde = LeerFechaFiltro(txtDesde.Text);
        DateTime? hasta = LeerFechaFiltro(txtHasta.Text);
        gv.DataSource   = ReservaRN.Listar(
            ddlEstado.SelectedValue, txtBuscar.Text.Trim(), desde, hasta);
        gv.DataBind();
    }

    private static DateTime? LeerFechaFiltro(string texto)
    {
        DateTime d;
        return DateTime.TryParseExact(texto, "yyyy-MM-dd",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out d) ? (DateTime?)d : null;
    }

    protected void Filtrar(object sender, EventArgs e) { gv.PageIndex = 0; Cargar(); }

    protected void btnLimpiarFiltros_Click(object sender, EventArgs e)
    {
        ddlEstado.SelectedIndex = 0;
        txtBuscar.Text = txtDesde.Text = txtHasta.Text = "";
        gv.PageIndex   = 0;
        Cargar();
    }

    protected void gv_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv.PageIndex = e.NewPageIndex;
        Cargar();
    }

    // ---- Lectura de fechas del formulario ----

    private bool LeerFechas(out DateTime entrada, out DateTime salida)
    {
        bool ok1 = DateTime.TryParseExact(txtEntrada.Text, "yyyy-MM-dd",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out entrada);
        bool ok2 = DateTime.TryParseExact(txtSalida.Text, "yyyy-MM-dd",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out salida);
        if (!ok1 || !ok2)
            Ui.Msg(lblMsg, "Seleccione las fechas de entrada y salida.", false);
        return ok1 && ok2;
    }

    // ---- Buscar habitaciones disponibles ----

    protected void btnBuscarHab_Click(object sender, EventArgs e)
    {
        lblMsg.Visible = false;
        pnlHab.Visible = false;
        DateTime entrada, salida;
        if (!LeerFechas(out entrada, out salida)) return;
        try
        {
            DataTable libres = ReservaRN.BuscarDisponibles(
                entrada, salida, Ui.ToInt(txtPersonas.Text, 1));
            if (libres.Rows.Count == 0)
            {
                Ui.Msg(lblMsg,
                    "No hay habitaciones libres para esas fechas y cantidad de personas. Pruebe otras fechas.",
                    false);
                return;
            }
            ddlHab.DataSource = libres;
            ddlHab.DataBind();
            pnlHab.Visible = true;
            ActualizarResumen();
        }
        catch (ApplicationException ex) { Ui.Msg(lblMsg, ex.Message, false); }
    }

    protected void ddlHab_SelectedIndexChanged(object sender, EventArgs e) { ActualizarResumen(); }

    private void ActualizarResumen()
    {
        DateTime entrada, salida;
        if (!LeerFechas(out entrada, out salida) || ddlHab.Items.Count == 0) return;
        int     noches = Reglas.Noches(entrada, salida);
        decimal total  = ReservaRN.Calcular(Ui.ToInt(ddlHab.SelectedValue, 0), entrada, salida);
        litNoches.Text = noches + (noches == 1 ? " noche" : " noches") + " · total estimado";
        litTotal.Text  = "Bs " + total.ToString("N2");
    }

    // ---- Confirmar reserva ----

    protected void btnConfirmar_Click(object sender, EventArgs e)
    {
        // ID del huésped viene del campo oculto poblado por el autocompletado JS
        int idHuesped = Ui.ToInt(hfIdHuesped.Value, 0);
        if (idHuesped <= 0)
        {
            Ui.Msg(lblMsg, "Selecciona un huésped de la lista de sugerencias.", false);
            return;
        }

        DateTime entrada, salida;
        if (!LeerFechas(out entrada, out salida)) return;
        try
        {
            int id = ReservaRN.Crear(
                idHuesped,
                Ui.ToInt(ddlHab.SelectedValue, 0),
                entrada, salida,
                Ui.ToInt(txtPersonas.Text, 1),
                txtObs.Text);
            Ui.Toast(this, "Reserva N.º " + id + " confirmada correctamente.", true);
            // Limpiar formulario
            pnlHab.Visible          = false;
            txtObs.Text             = "";
            txtBuscarHuesped.Text   = "";
            hfIdHuesped.Value       = "0";
            gv.PageIndex            = 0;
            Cargar();
        }
        catch (ApplicationException ex) { Ui.Msg(lblMsg, ex.Message, false); }
    }

    // ---- Acciones del grid (CheckIn / CheckOut / Cancelar / IniciarPago) ----

    protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int id = Ui.ToInt(Convert.ToString(e.CommandArgument), 0);

        if (e.CommandName == "IniciarPago")
        {
            AbrirPanelPago(id);
            return;
        }

        string nuevo = null, msg = null;
        switch (e.CommandName)
        {
            case "CheckIn":  nuevo = "CheckIn";   msg = "Check-in registrado.";  break;
            case "CheckOut": nuevo = "CheckOut";  msg = "Check-out registrado."; break;
            case "Cancelar": nuevo = "Cancelada"; msg = "Reserva cancelada.";    break;
        }
        if (nuevo == null) return;

        try
        {
            ReservaRN.CambiarEstado(id, nuevo);
            Ui.Toast(this, msg, true);
            pnlPago.Visible = false;
            Cargar();
        }
        catch (ApplicationException ex) { Ui.Msg(lblMsg, ex.Message, false); }
    }

    // ---- Panel de pago ----

    private void AbrirPanelPago(int idReserva)
    {
        hfIdPago.Value     = idReserva.ToString();
        litIdPago.Text     = idReserva.ToString();
        decimal saldo      = ReservaRN.SaldoPendiente(idReserva);
        litSaldo.Text      = saldo.ToString("N2");
        txtMonto.Text      = saldo.ToString("N2");
        lblPagoMsg.Visible = false;
        pnlPago.Visible    = true;
    }

    protected void btnCerrarPago_Click(object sender, EventArgs e)
    {
        pnlPago.Visible = false;
    }

    protected void btnPagar_Click(object sender, EventArgs e)
    {
        int idReserva = Ui.ToInt(hfIdPago.Value, 0);
        decimal monto;
        string montoTexto = (txtMonto.Text ?? "").Replace(",", ".");
        if (!decimal.TryParse(montoTexto, NumberStyles.Any,
                CultureInfo.InvariantCulture, out monto) || monto <= 0)
        {
            Ui.Msg(lblPagoMsg, "Ingrese un monto válido mayor a cero.", false);
            return;
        }
        try
        {
            ReservaRN.PagarReserva(idReserva, monto, ddlMetodo.SelectedValue);
            pnlPago.Visible = false;
            Ui.Toast(this, "Pago de Bs " + monto.ToString("N2") + " registrado.", true);
            Cargar();
        }
        catch (ApplicationException ex) { Ui.Msg(lblPagoMsg, ex.Message, false); }
    }
}
