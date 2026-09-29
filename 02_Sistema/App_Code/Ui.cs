/* =====================================================
   Ui.cs — Utilidades de interfaz compartidas por las páginas.
   Codificación: UTF-8
   ===================================================== */
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public static class Ui
{
    /// <summary>
    /// Muestra un mensaje en un Label con estilo Bootstrap (inline, persistente).
    /// Úsalo para errores que el usuario debe ver junto al formulario.
    /// </summary>
    public static void Msg(Label lbl, string texto, bool ok)
    {
        lbl.Text     = HttpUtility.HtmlEncode(texto);
        lbl.CssClass = "alert d-block mb-3 " + (ok ? "alert-success" : "alert-danger");
        lbl.Visible  = true;
    }

    /// <summary>
    /// Dispara un toast flotante (esquina inferior-derecha) desde el servidor.
    /// Ideal para confirmaciones de éxito; el toast desaparece automáticamente.
    /// </summary>
    /// <param name="page">Página actual (para RegisterStartupScript).</param>
    /// <param name="texto">Texto del mensaje.</param>
    /// <param name="ok">true=éxito (verde), false=error (rojo).</param>
    /// <param name="ms">Milisegundos hasta el auto-cierre (default 4500).</param>
    public static void Toast(Page page, string texto, bool ok, int ms = 4500)
    {
        string seguro = (texto ?? "")
            .Replace("\\", "\\\\")
            .Replace("'",  "\\'")
            .Replace("\r", "")
            .Replace("\n", " ");
        string script = string.Format(
            "mostrarToast('{0}',{1},{2});", seguro, ok ? "true" : "false", ms);
        ScriptManager.RegisterStartupScript(
            page, page.GetType(),
            "toast_" + Guid.NewGuid().ToString("N"),
            script, addScriptTags: true);
    }

    /// <summary>
    /// Ejecuta una acción y muestra el mensaje de excepción en un Label si falla.
    /// Evita repetir el patrón try/catch en las páginas para errores simples.
    /// </summary>
    public static bool Safe(Action accion, Label lbl)
    {
        try
        {
            accion();
            return true;
        }
        catch (ApplicationException ex)
        {
            Msg(lbl, ex.Message, false);
            return false;
        }
        catch (Exception ex)
        {
            Msg(lbl, "Ocurrió un error inesperado. Detalle técnico: " + ex.Message, false);
            return false;
        }
    }

    /// <summary>Convierte un string a int; devuelve porDefecto si no es válido.</summary>
    public static int ToInt(string s, int porDefecto)
    {
        int n;
        return int.TryParse(s, out n) ? n : porDefecto;
    }

    /// <summary>Formatea un decimal como moneda boliviana con 2 decimales.</summary>
    public static string FormatBs(decimal valor)
    {
        return valor.ToString("N2");
    }
}
