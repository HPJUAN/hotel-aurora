<%@ Page Title="Reservas" Language="C#" MasterPageFile="~/Site.master"
         AutoEventWireup="true" CodeFile="Reservas.aspx.cs" Inherits="Reservas" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<%-- Panel de registro de pago (aparece al hacer clic en "Pagar") --%>
<asp:Panel ID="pnlPago" runat="server" Visible="false" CssClass="card-x mb-3">
    <div class="d-flex justify-content-between align-items-center mb-3">
        <h3 class="mb-0"><i class="bi bi-credit-card me-2"></i>Registrar pago — Reserva N.º <asp:Literal ID="litIdPago" runat="server" /></h3>
        <asp:Button ID="btnCerrarPago" runat="server" Text="✕ Cerrar"
            CssClass="btn btn-ghost btn-sm" CausesValidation="false"
            OnClick="btnCerrarPago_Click" />
    </div>
    <asp:HiddenField ID="hfIdPago" runat="server" Value="0" />
    <asp:Label ID="lblPagoMsg" runat="server" Visible="false" />
    <div class="row g-3 align-items-end">
        <div class="col-md-3">
            <label class="form-label">Saldo pendiente (Bs)</label>
            <div class="resumen py-2 px-3 mb-0">
                <span class="t">Bs <asp:Literal ID="litSaldo" runat="server" /></span>
            </div>
        </div>
        <div class="col-md-3">
            <label class="form-label" for="<%= txtMonto.ClientID %>">Monto a pagar (Bs)</label>
            <asp:TextBox ID="txtMonto" runat="server" CssClass="form-control"
                TextMode="Number" placeholder="0.00" />
        </div>
        <div class="col-md-3">
            <label class="form-label" for="<%= ddlMetodo.ClientID %>">Método de pago</label>
            <asp:DropDownList ID="ddlMetodo" runat="server" CssClass="form-select">
                <asp:ListItem>Efectivo</asp:ListItem>
                <asp:ListItem>Tarjeta</asp:ListItem>
                <asp:ListItem>Transferencia</asp:ListItem>
                <asp:ListItem>QR</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="col-md-3">
            <asp:Button ID="btnPagar" runat="server" Text="Registrar pago"
                CssClass="btn btn-brass w-100" CausesValidation="false"
                OnClick="btnPagar_Click" />
        </div>
    </div>
</asp:Panel>

<%-- ===== Contenido principal: formulario + lista ===== --%>
<asp:Label ID="lblMsg" runat="server" Visible="false" />
<div class="grid-2">

    <%-- ---- Formulario nueva reserva ---- --%>
    <section class="card-x">
        <h2>Nueva reserva</h2>

        <div class="mb-3 ac-wrap">
            <label class="form-label">Huésped</label>
            <%-- Input de búsqueda (visible) + campo oculto con el ID seleccionado --%>
            <asp:TextBox ID="txtBuscarHuesped" runat="server" CssClass="form-control"
                placeholder="Escribe nombre, apellido o CI…" />
            <asp:HiddenField ID="hfIdHuesped" runat="server" Value="0" />
            <%-- Lista de sugerencias (JS la muestra/oculta) --%>
            <div id="acHuespedes" class="ac-list" style="display:none"></div>
            <div id="acInfo" class="ac-sel-info" style="display:none">
                <i class="bi bi-person-check"></i> <span id="acInfoNombre"></span>
            </div>
            <div class="kpi-mini">¿Es nuevo? <a href="Huespedes.aspx">Regístralo primero</a>.</div>
        </div>

        <div class="row g-2 mb-3">
            <div class="col-6">
                <label class="form-label" for="<%= txtEntrada.ClientID %>">Entrada</label>
                <asp:TextBox ID="txtEntrada" runat="server" CssClass="form-control"
                    TextMode="Date" onchange="validarFechasCliente()" />
            </div>
            <div class="col-6">
                <label class="form-label" for="<%= txtSalida.ClientID %>">Salida</label>
                <asp:TextBox ID="txtSalida" runat="server" CssClass="form-control"
                    TextMode="Date" onchange="validarFechasCliente()" />
            </div>
        </div>
        <span id="spanFechaErr" class="val" style="display:none;margin-top:-.5rem;margin-bottom:.5rem">
            La fecha de salida debe ser posterior a la de entrada.
        </span>

        <div class="mb-3">
            <label class="form-label" for="<%= txtPersonas.ClientID %>">Personas</label>
            <asp:TextBox ID="txtPersonas" runat="server" CssClass="form-control"
                TextMode="Number" Text="1" />
        </div>

        <asp:Button ID="btnBuscarHab" runat="server" Text="Ver habitaciones disponibles"
            CssClass="btn btn-ghost w-100" CausesValidation="false"
            OnClick="btnBuscarHab_Click" />

        <asp:Panel ID="pnlHab" runat="server" Visible="false" CssClass="mt-3">
            <label class="form-label" for="<%= ddlHab.ClientID %>">Habitaciones libres en esas fechas</label>
            <asp:DropDownList ID="ddlHab" runat="server" CssClass="form-select"
                DataTextField="etiqueta" DataValueField="id_habitacion"
                AutoPostBack="true" OnSelectedIndexChanged="ddlHab_SelectedIndexChanged" />

            <div class="resumen">
                <div class="kpi-mini"><asp:Literal ID="litNoches" runat="server" /></div>
                <div class="t"><asp:Literal ID="litTotal"  runat="server" /></div>
            </div>

            <label class="form-label" for="<%= txtObs.ClientID %>">Observaciones (opcional)</label>
            <asp:TextBox ID="txtObs" runat="server" CssClass="form-control mb-3"
                MaxLength="250" TextMode="MultiLine" Rows="2"
                placeholder="Ej. llegada tarde, cama extra" />

            <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar reserva"
                CssClass="btn btn-brass w-100" CausesValidation="false"
                OnClick="btnConfirmar_Click" />
        </asp:Panel>
    </section>

    <%-- ---- Lista de reservas con filtros ---- --%>
    <section class="card-x">
        <h2>Reservas</h2>

        <%-- Fila 1: estado y texto --%>
        <div class="d-flex gap-2 mb-2 flex-wrap">
            <asp:DropDownList ID="ddlEstado" runat="server" CssClass="form-select"
                style="max-width:190px" AutoPostBack="true" OnSelectedIndexChanged="Filtrar">
                <asp:ListItem Value="" Text="Todos los estados" />
                <asp:ListItem>Confirmada</asp:ListItem>
                <asp:ListItem>CheckIn</asp:ListItem>
                <asp:ListItem>CheckOut</asp:ListItem>
                <asp:ListItem>Cancelada</asp:ListItem>
            </asp:DropDownList>
            <asp:TextBox ID="txtBuscar" runat="server" CssClass="form-control"
                style="max-width:240px" placeholder="Huésped, CI o n.º hab." />
            <asp:Button ID="btnBuscar" runat="server" Text="Buscar"
                CssClass="btn btn-ghost" CausesValidation="false" OnClick="Filtrar" />
        </div>

        <%-- Fila 2: rango de fechas --%>
        <div class="d-flex gap-2 mb-3 flex-wrap align-items-center">
            <label class="form-label mb-0 text-nowrap">Entrada entre</label>
            <asp:TextBox ID="txtDesde" runat="server" CssClass="form-control"
                TextMode="Date" style="max-width:160px" />
            <label class="form-label mb-0">y</label>
            <asp:TextBox ID="txtHasta" runat="server" CssClass="form-control"
                TextMode="Date" style="max-width:160px" />
            <asp:Button ID="btnFiltrarFecha" runat="server" Text="Filtrar"
                CssClass="btn btn-ghost" CausesValidation="false" OnClick="Filtrar" />
            <asp:Button ID="btnLimpiarFiltros" runat="server" Text="Limpiar"
                CssClass="btn btn-ghost" CausesValidation="false"
                OnClick="btnLimpiarFiltros_Click" />
        </div>

        <div class="tbl-wrap">
            <asp:GridView ID="gv" runat="server"
                AutoGenerateColumns="false" GridLines="None" CssClass="tbl"
                OnRowCommand="gv_RowCommand" ShowHeaderWhenEmpty="true"
                AllowPaging="true" PageSize="10"
                OnPageIndexChanging="gv_PageIndexChanging">
                <PagerStyle CssClass="pager" HorizontalAlign="Center" />
                <EmptyDataTemplate>
                    <div class="empty">
                        <i class="bi bi-calendar2-x"></i>
                        <p>No hay reservas con ese filtro.</p>
                    </div>
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="id_reserva" HeaderText="N.º" ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                    <asp:BoundField DataField="huesped"    HeaderText="Huésped" />
                    <asp:TemplateField HeaderText="Habitación">
                        <ItemTemplate>
                            <b><%# Eval("numero") %></b>
                            <span class="text-secondary small"><%# Eval("tipo") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="fecha_entrada" HeaderText="Entrada" DataFormatString="{0:dd/MM/yy}" />
                    <asp:BoundField DataField="fecha_salida"  HeaderText="Salida"  DataFormatString="{0:dd/MM/yy}" />
                    <asp:BoundField DataField="total" HeaderText="Total Bs"
                        DataFormatString="{0:N2}"
                        ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                    <asp:TemplateField HeaderText="Estado">
                        <ItemTemplate>
                            <span class="st st-<%# Eval("estado") %>"><%# Eval("estado") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Acciones">
                        <ItemTemplate>
                            <%-- Check-in: solo si está Confirmada --%>
                            <asp:LinkButton runat="server" CssClass="act"
                                CommandName="CheckIn"
                                CommandArgument='<%# Eval("id_reserva") %>'
                                Visible='<%# Eval("estado").ToString() == "Confirmada" %>'
                                CausesValidation="false">
                                <i class="bi bi-box-arrow-in-right"></i> CI
                            </asp:LinkButton>
                            <%-- Check-out: solo si está en CheckIn --%>
                            <asp:LinkButton runat="server" CssClass="act"
                                CommandName="CheckOut"
                                CommandArgument='<%# Eval("id_reserva") %>'
                                Visible='<%# Eval("estado").ToString() == "CheckIn" %>'
                                CausesValidation="false">
                                <i class="bi bi-box-arrow-right"></i> CO
                            </asp:LinkButton>
                            <%-- Pago: para Confirmada y CheckIn --%>
                            <asp:LinkButton runat="server" CssClass="act"
                                CommandName="IniciarPago"
                                CommandArgument='<%# Eval("id_reserva") %>'
                                Visible='<%# Eval("estado").ToString() == "Confirmada" || Eval("estado").ToString() == "CheckIn" %>'
                                CausesValidation="false">
                                <i class="bi bi-cash-coin"></i>
                            </asp:LinkButton>
                            <%-- Cancelar: solo si está Confirmada --%>
                            <asp:LinkButton runat="server" CssClass="act del"
                                CommandName="Cancelar"
                                CommandArgument='<%# Eval("id_reserva") %>'
                                Visible='<%# Eval("estado").ToString() == "Confirmada" %>'
                                CausesValidation="false"
                                OnClientClick="return confirm('¿Cancelar esta reserva?');">
                                <i class="bi bi-x-circle"></i>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </section>
</div>
</asp:Content>

<asp:Content ContentPlaceHolderID="Scripts" runat="server">
<script>
/* =====================================================
   Autocompletado de huéspedes — Reservas.aspx
   Busca por nombre, apellido o CI sin postback.
   ===================================================== */
(function () {
    // JSON inyectado desde el servidor (ver Reservas.aspx.cs → JsonHuespedes)
    var HUESPEDES = <%= JsonHuespedes %>;

    var input  = document.getElementById('<%= txtBuscarHuesped.ClientID %>');
    var hidden = document.getElementById('<%= hfIdHuesped.ClientID %>');
    var lista  = document.getElementById('acHuespedes');
    var info   = document.getElementById('acInfo');
    var infoNm = document.getElementById('acInfoNombre');
    var selIdx = -1;

    /* ---- Utilidades ---- */
    function esc(s) {
        return String(s).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;');
    }
    function escReg(s) {
        return s.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    }
    function resaltar(texto, q) {
        if (!q) return texto;
        return texto.replace(new RegExp('(' + escReg(q) + ')', 'gi'), '<mark>$1</mark>');
    }

    /* ---- Muestra/oculta info del huésped seleccionado ---- */
    function mostrarInfo(nombre) {
        if (nombre) {
            infoNm.textContent = nombre;
            info.style.display = 'block';
        } else {
            info.style.display = 'none';
        }
    }

    /* ---- Renderiza la lista de sugerencias ---- */
    function renderizar(items, q) {
        lista.innerHTML = '';
        selIdx = -1;
        if (!items.length) {
            lista.innerHTML = '<div class="ac-no-results">Sin resultados para "' + esc(q) + '"</div>';
            lista.style.display = 'block';
            return;
        }
        items.slice(0, 10).forEach(function (h) {
            var div = document.createElement('div');
            div.className = 'ac-item';
            div.innerHTML = resaltar(esc(h.label), q);
            div.addEventListener('mousedown', function (e) {
                e.preventDefault(); // evitar que blur cierre antes del click
                seleccionar(h);
            });
            lista.appendChild(div);
        });
        lista.style.display = 'block';
    }

    /* ---- Selecciona un huésped de la lista ---- */
    function seleccionar(h) {
        input.value  = h.label;
        hidden.value = h.id;
        lista.style.display = 'none';
        mostrarInfo(h.label.split(' - CI')[0]); // muestra solo nombre sin "- CI xxxxx"
    }

    /* ---- Filtra al escribir ---- */
    input.addEventListener('input', function () {
        var q = this.value.trim().toLowerCase();
        hidden.value = '0';
        mostrarInfo(null);
        if (q.length < 1) { lista.style.display = 'none'; return; }
        var res = HUESPEDES.filter(function (h) {
            return h.label.toLowerCase().indexOf(q) >= 0;
        });
        renderizar(res, q);
    });

    /* ---- Navegación con teclado ---- */
    input.addEventListener('keydown', function (e) {
        var items = lista.querySelectorAll('.ac-item');
        if (!items.length) return;
        if (e.key === 'ArrowDown') {
            e.preventDefault();
            selIdx = Math.min(selIdx + 1, items.length - 1);
            items.forEach(function (x, i) { x.classList.toggle('activo', i === selIdx); });
        } else if (e.key === 'ArrowUp') {
            e.preventDefault();
            selIdx = Math.max(selIdx - 1, 0);
            items.forEach(function (x, i) { x.classList.toggle('activo', i === selIdx); });
        } else if (e.key === 'Enter' && selIdx >= 0) {
            e.preventDefault();
            items[selIdx].dispatchEvent(new MouseEvent('mousedown'));
        } else if (e.key === 'Escape') {
            lista.style.display = 'none';
        }
    });

    /* ---- Cierra la lista cuando pierde el foco ---- */
    input.addEventListener('blur', function () {
        setTimeout(function () { lista.style.display = 'none'; }, 180);
    });

    /* ---- Al cargar la página: si ya hay un huésped (postback), muestra su nombre ---- */
    if (hidden.value && hidden.value !== '0') {
        var encontrado = HUESPEDES.find(function (x) { return x.id === hidden.value; });
        if (encontrado) {
            input.value = encontrado.label;
            mostrarInfo(encontrado.label.split(' - CI')[0]);
        }
    }
}());

/* ---- Validación de fechas en el cliente ---- */
function validarFechasCliente() {
    var e   = document.getElementById('<%= txtEntrada.ClientID %>');
    var s   = document.getElementById('<%= txtSalida.ClientID  %>');
    var err = document.getElementById('spanFechaErr');
    if (!e || !s || !e.value || !s.value) { if(err) err.style.display='none'; return; }
    err.style.display = (s.value <= e.value) ? 'inline' : 'none';
}
</script>
</asp:Content>

