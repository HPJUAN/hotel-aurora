using System;
using System.Web.UI;

public partial class SiteMaster : MasterPage
{
    protected string Act(string pagina)
    {
        return Request.Path.EndsWith("/" + pagina, StringComparison.OrdinalIgnoreCase) ? "active" : "";
    }
}
