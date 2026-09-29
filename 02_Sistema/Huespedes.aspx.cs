/* =====================================================
   Huespedes.aspx.cs — Code-behind de la página de huéspedes
   Codificación: UTF-8
   ===================================================== */
using System;
using System.Data;
using System.Web.UI.WebControls;
using Entidades;
using RN;

public partial class Huespedes : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack) Cargar();
    }

    private void Cargar()
    {
        gv.DataSource = HuespedRN.Listar(txtBuscar.Text.Trim());
        gv.DataBind();
    }

    private void Limpiar()
    {
        hfId.Value = "0";
        txtCi.Text = txtNombres.Text = txtApellidos.Text =
        txtEmail.Text = txtTelefono.Text = "";
        txtNacionalidad.Text = "Boliviana";
        litTitulo.Text = "Nuevo huésped";
        lblMsg.Visible = false;
    }

    protected void gv_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gv.PageIndex = e.NewPageIndex;
        Cargar();
    }

    protected void btnGuardar_Click(object sender, EventArgs e)
    {
        if (!IsValid) return;
        try
        {
            Huesped h = new Huesped
            {
                Id          = Ui.ToInt(hfId.Value, 0),
                Ci          = txtCi.Text,
                Nombres     = txtNombres.Text,
                Apellidos   = txtApellidos.Text,
                Email       = txtEmail.Text,
                Telefono    = txtTelefono.Text,
                Nacionalidad= txtNacionalidad.Text
            };
            HuespedRN.Guardar(h);
            string accion = h.Id == 0 ? "registrado" : "actualizado";
            Ui.Toast(this, "Huésped " + accion + " correctamente.", true);
            Limpiar();
            Cargar();
        }
        catch (ApplicationException ex) { Ui.Msg(lblMsg, ex.Message, false); }
    }

    protected void btnCancelar_Click(object sender, EventArgs e) { Limpiar(); }

    protected void btnBuscar_Click(object sender, EventArgs e)
    {
        gv.PageIndex = 0;
        Cargar();
    }

    protected void gv_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        int id = Ui.ToInt(Convert.ToString(e.CommandArgument), 0);
        if (e.CommandName == "Editar")
        {
            DataRow r = HuespedRN.Obtener(id);
            if (r == null) return;
            hfId.Value           = id.ToString();
            txtCi.Text           = r["ci"].ToString();
            txtNombres.Text      = r["nombres"].ToString();
            txtApellidos.Text    = r["apellidos"].ToString();
            txtEmail.Text        = r["email"] == DBNull.Value ? "" : r["email"].ToString();
            txtTelefono.Text     = r["telefono"] == DBNull.Value ? "" : r["telefono"].ToString();
            txtNacionalidad.Text = r["nacionalidad"].ToString();
            litTitulo.Text       = "Editar huésped";
            lblMsg.Visible       = false;
        }
        else if (e.CommandName == "Borrar")
        {
            try
            {
                HuespedRN.Eliminar(id);
                Ui.Toast(this, "Huésped eliminado.", true);
                gv.PageIndex = 0;
                Cargar();
            }
            catch (ApplicationException ex) { Ui.Msg(lblMsg, ex.Message, false); }
        }
    }
}
