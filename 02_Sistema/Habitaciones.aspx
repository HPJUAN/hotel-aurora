<%@ Page Title="Habitaciones" Language="C#" MasterPageFile="~/Site.master"
         AutoEventWireup="true" CodeFile="Habitaciones.aspx.cs" Inherits="Habitaciones" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="grid-2">

    <%-- ===== Formulario ===== --%>
    <section class="card-x">
        <h2><asp:Literal ID="litTitulo" runat="server" Text="Nueva habitación" /></h2>
        <asp:Label    ID="lblMsg" runat="server" Visible="false" />
        <asp:HiddenField ID="hfId" runat="server" Value="0" />

        <div class="row g-2 mb-3">
            <div class="col-6">
                <label class="form-label" for="<%= txtNumero.ClientID %>">Número</label>
                <asp:TextBox ID="txtNumero" runat="server" CssClass="form-control"
                    MaxLength="10" placeholder="Ej. 204" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNumero"
                    CssClass="val" Display="Dynamic" ErrorMessage="El número es obligatorio." />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtPiso.ClientID %>">Piso (1-20)</label>
                <asp:TextBox ID="txtPiso" runat="server" CssClass="form-control"
                    TextMode="Number" Text="1" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPiso"
                    CssClass="val" Display="Dynamic" ErrorMessage="El piso es obligatorio." />
                <asp:RangeValidator runat="server" ControlToValidate="txtPiso"
                    CssClass="val" Display="Dynamic"
                    MinimumValue="1" MaximumValue="20" Type="Integer"
                    ErrorMessage="El piso debe estar entre 1 y 20." />
            </div>
        </div>

        <div class="mb-3">
            <label class="form-label" for="<%= ddlTipo.ClientID %>">Tipo de habitación</label>
            <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-select"
                DataTextField="etiqueta" DataValueField="id_tipo" />
        </div>

        <div class="mb-3">
            <label class="form-label" for="<%= ddlEstado.ClientID %>">Estado</label>
            <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select">
                <asp:ListItem>Disponible</asp:ListItem>
                <asp:ListItem>Mantenimiento</asp:ListItem>
            </asp:DropDownList>
        </div>

        <asp:Button ID="btnGuardar" runat="server" Text="Guardar habitación"
            CssClass="btn btn-brass" OnClick="btnGuardar_Click" />
        <asp:Button ID="btnCancelar" runat="server" Text="Limpiar"
            CssClass="btn btn-ghost ms-2" CausesValidation="false"
            OnClick="btnCancelar_Click" />
    </section>

    <%-- ===== Inventario ===== --%>
    <section class="card-x">
        <h2>Inventario de habitaciones</h2>
        <div class="d-flex gap-2 mb-3">
            <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control"
                placeholder="Buscar por número o tipo" />
            <asp:Button ID="btnBuscar" runat="server" Text="Buscar"
                CssClass="btn btn-ghost" CausesValidation="false"
                OnClick="btnBuscar_Click" />
        </div>
        <div class="tbl-wrap">
            <asp:GridView ID="gv" runat="server"
                AutoGenerateColumns="false" GridLines="None" CssClass="tbl"
                DataKeyNames="id_habitacion" OnRowCommand="gv_RowCommand"
                ShowHeaderWhenEmpty="true">
                <EmptyDataTemplate>
                    <div class="empty">
                        <i class="bi bi-key"></i>
                        <p>No se encontraron habitaciones.<br />Registra la primera con el formulario.</p>
                    </div>
                </EmptyDataTemplate>
                <Columns>
                    <asp:TemplateField HeaderText="N.º">
                        <ItemTemplate>
                            <span class="serif" style="font-size:1.15rem"><%# Eval("numero") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="piso"        HeaderText="Piso" ItemStyle-CssClass="num" />
                    <asp:BoundField DataField="tipo"        HeaderText="Tipo" />
                    <asp:BoundField DataField="capacidad"   HeaderText="Cap." ItemStyle-CssClass="num" />
                    <asp:BoundField DataField="precio_noche" HeaderText="Bs/noche"
                        DataFormatString="{0:N2}"
                        ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                    <asp:TemplateField HeaderText="Estado">
                        <ItemTemplate>
                            <span class="st st-<%# Eval("estado") %>"><%# Eval("estado") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CssClass="act"
                                CommandName="Editar"
                                CommandArgument='<%# Eval("id_habitacion") %>'
                                CausesValidation="false">
                                <i class="bi bi-pencil"></i> Editar
                            </asp:LinkButton>
                            <asp:LinkButton runat="server" CssClass="act del"
                                CommandName="Borrar"
                                CommandArgument='<%# Eval("id_habitacion") %>'
                                CausesValidation="false"
                                OnClientClick="return confirm('¿Eliminar esta habitación?');">
                                <i class="bi bi-trash"></i>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>
</div>
</asp:Content>

<asp:Content ContentPlaceHolderID="Scripts" runat="server" />
