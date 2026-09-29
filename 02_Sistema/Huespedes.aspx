<%@ Page Title="Huéspedes" Language="C#" MasterPageFile="~/Site.master"
         AutoEventWireup="true" CodeFile="Huespedes.aspx.cs" Inherits="Huespedes" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="grid-2">

    <%-- ===== Formulario ===== --%>
    <section class="card-x">
        <h2><asp:Literal ID="litTitulo" runat="server" Text="Nuevo huésped" /></h2>
        <asp:Label ID="lblMsg" runat="server" Visible="false" />
        <asp:HiddenField ID="hfId" runat="server" Value="0" />

        <div class="mb-3">
            <label class="form-label" for="<%= txtCi.ClientID %>">Documento de identidad</label>
            <asp:TextBox ID="txtCi" runat="server" CssClass="form-control"
                MaxLength="20" placeholder="Ej. 4521367" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCi"
                CssClass="val" Display="Dynamic"
                ErrorMessage="Ingrese el documento de identidad." />
        </div>

        <div class="row g-2 mb-3">
            <div class="col-6">
                <label class="form-label" for="<%= txtNombres.ClientID %>">Nombres</label>
                <asp:TextBox ID="txtNombres" runat="server" CssClass="form-control" MaxLength="60" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNombres"
                    CssClass="val" Display="Dynamic" ErrorMessage="Nombres es obligatorio." />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtApellidos.ClientID %>">Apellidos</label>
                <asp:TextBox ID="txtApellidos" runat="server" CssClass="form-control" MaxLength="60" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtApellidos"
                    CssClass="val" Display="Dynamic" ErrorMessage="Apellidos es obligatorio." />
            </div>
        </div>

        <div class="mb-3">
            <label class="form-label" for="<%= txtEmail.ClientID %>">Correo electrónico <span class="text-secondary">(opcional)</span></label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control"
                MaxLength="100" TextMode="Email" placeholder="correo@ejemplo.com" />
            <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmail"
                CssClass="val" Display="Dynamic"
                ValidationExpression="^$|^[^@\s]+@[^@\s]+\.[^@\s]+$"
                ErrorMessage="El correo no tiene formato válido." />
        </div>

        <div class="row g-2 mb-3">
            <div class="col-6">
                <label class="form-label" for="<%= txtTelefono.ClientID %>">Teléfono <span class="text-secondary">(opcional)</span></label>
                <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control"
                    MaxLength="20" placeholder="Ej. +591 70123456" />
                <asp:RegularExpressionValidator runat="server" ControlToValidate="txtTelefono"
                    CssClass="val" Display="Dynamic"
                    ValidationExpression="^$|^\+?[\d\s\-\(\)]{7,20}$"
                    ErrorMessage="Teléfono inválido (7-20 dígitos o caracteres permitidos)." />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtNacionalidad.ClientID %>">Nacionalidad</label>
                <asp:TextBox ID="txtNacionalidad" runat="server" CssClass="form-control"
                    MaxLength="40" Text="Boliviana" />
            </div>
        </div>

        <asp:Button ID="btnGuardar" runat="server" Text="Guardar huésped"
            CssClass="btn btn-brass" OnClick="btnGuardar_Click" />
        <asp:Button ID="btnCancelar" runat="server" Text="Limpiar"
            CssClass="btn btn-ghost ms-2" CausesValidation="false"
            OnClick="btnCancelar_Click" />
    </section>

    <%-- ===== Lista de huéspedes ===== --%>
    <section class="card-x">
        <h2>Huéspedes registrados</h2>
        <div class="d-flex gap-2 mb-3">
            <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control"
                placeholder="Buscar por documento, nombre o apellido" />
            <asp:Button ID="btnBuscar" runat="server" Text="Buscar"
                CssClass="btn btn-ghost" CausesValidation="false"
                OnClick="btnBuscar_Click" />
        </div>
        <div class="tbl-wrap">
            <asp:GridView ID="gv" runat="server"
                AutoGenerateColumns="false" GridLines="None" CssClass="tbl"
                DataKeyNames="id_huesped" OnRowCommand="gv_RowCommand"
                ShowHeaderWhenEmpty="true"
                AllowPaging="true" PageSize="12"
                OnPageIndexChanging="gv_PageIndexChanging">
                <PagerStyle CssClass="pager" HorizontalAlign="Center" />
                <EmptyDataTemplate>
                    <div class="empty">
                        <i class="bi bi-person-x"></i>
                        <p>No hay huéspedes que coincidan con la búsqueda.</p>
                    </div>
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="ci"          HeaderText="Documento" />
                    <asp:TemplateField HeaderText="Huésped">
                        <ItemTemplate>
                            <b><%# Eval("apellidos") %></b>, <%# Eval("nombres") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="email"        HeaderText="Correo" />
                    <asp:BoundField DataField="telefono"     HeaderText="Teléfono" />
                    <asp:BoundField DataField="nacionalidad" HeaderText="Nac." />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton runat="server" CssClass="act"
                                CommandName="Editar"
                                CommandArgument='<%# Eval("id_huesped") %>'
                                CausesValidation="false">
                                <i class="bi bi-pencil"></i> Editar
                            </asp:LinkButton>
                            <asp:LinkButton runat="server" CssClass="act del"
                                CommandName="Borrar"
                                CommandArgument='<%# Eval("id_huesped") %>'
                                CausesValidation="false"
                                OnClientClick="return confirm('¿Eliminar este huésped?');">
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
