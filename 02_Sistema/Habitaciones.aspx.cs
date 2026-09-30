/* =====================================================
   Habitaciones.aspx.cs — Code-behind del inventario de habitaciones
   Codificación: UTF-8
   ===================================================== */
using System;
using System.Data;
using System.Web.UI.WebControls;
using Entidades;
using RN;

public partial class Habitaciones : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ddlTipo.DataSource = HabitacionRN.ListarTipos();
            ddlTipo.DataBind();
            Cargar();
        }
    }

    private void Cargar()
    {
        gv.DataSource = HabitacionRN.Listar(txtBuscar.Text.Trim());
        gv.DataBind();
    }

    private void Limpiar()
    {
        hfId.Value        = "0";
        txtNumero.Text    = "";
        txtPiso.Text      = "1";
        ddlEstado.SelectedIndex = 0;
        litTitulo.Text    = "Nueva habitación";
        lblMsg.Visible    = false;
    }

    protected void btnGuardar_Click(object sender, EventArgs e)
    {
        if (!IsValid) return;
        try
        {
            Habitacion h = new Habitacion
            {
                Id     = Ui.ToInt(hfId.Value, 0),
                Numero = txtNumero.Text,
                Piso   = Ui.ToInt(txtPiso.Text, 0),
                IdTipo = Ui.ToInt(ddlTipo.SelectedValue, 0),
                Estado = ddlEstado.SelectedValue
            };
            HabitacionRN.Guardar(h);
            string accion = h.Id == 0 ? "registrada" : "actualizada";
            Ui.Toast(this, "Habitación " + h.Numero + " " + accion + " correctamente.", true);
            Limpiar();
            Cargar();
        }
        catch (ApplicationException ex) { Ui.Msg(lblMsg, ex.Message, false); }
    }

    protected void btnCancelar_Click(object sender, EventArgs e) { Limpiar(); }

    protected void btnBuscar_Click(object sender, EventArgs e) { Cargar(); }

    protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int id = Ui.ToInt(Convert.ToString(e.CommandArgument), 0);
        if (e.CommandName == "Editar")
        {
            DataRow r = HabitacionRN.Obtener(id);
            if (r == null) return;
            hfId.Value              = id.ToString();
            txtNumero.Text          = r["numero"].ToString();
            txtPiso.Text            = r["piso"].ToString();
            ddlTipo.SelectedValue   = r["id_tipo"].ToString();
            ddlEstado.SelectedValue = r["estado"].ToString();
            litTitulo.Text          = "Editar habitación " + r["numero"];
            lblMsg.Visible          = false;
        }
        else if (e.CommandName == "Borrar")
        {
            try
            {
                HabitacionRN.Eliminar(id);
                Ui.Toast(this, "Habitación eliminada.", true);
                Cargar();
            }
            catch (ApplicationException ex) { Ui.Msg(lblMsg, ex.Message, false); }
        }
    }
}
