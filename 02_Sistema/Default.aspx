<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">

<%-- Mensaje de error de conexión (inline, no toast) --%>
<asp:Label ID="lblMsg" runat="server" Visible="false" />

<%-- ===== Franja de indicadores ===== --%>
<div class="stats">
    <div class="stat gold">
        <div class="v"><asp:Literal ID="litOcupacion" runat="server" /><small>%</small></div>
        <div class="l">Ocupación de hoy</div>
    </div>
    <div class="stat">
        <div class="v"><asp:Literal ID="litLibres" runat="server" /></div>
        <div class="l">Habitaciones libres hoy</div>
    </div>
    <div class="stat">
        <div class="v"><asp:Literal ID="litActivas" runat="server" /></div>
        <div class="l">Reservas activas</div>
    </div>
    <div class="stat">
        <div class="v"><small>Bs</small> <asp:Literal ID="litIngresos" runat="server" /></div>
        <div class="l">Ingresos reservados este mes</div>
    </div>
</div>

<%-- ===== Tablero de llaves + Próximas llegadas ===== --%>
<div class="grid-eq">
    <section class="card-x">
        <h2>Tablero de llaves</h2>
        <p class="sub">Estado de cada habitación esta noche.</p>
        <div class="rack">
            <asp:Literal ID="litRack" runat="server" />
        </div>
        <div class="legend">
            <span><i style="background:var(--brass)"></i>Ocupada</span>
            <span><i style="background:var(--ok)"></i>Libre</span>
            <span><i style="border:1px dashed var(--muted)"></i>Mantenimiento</span>
        </div>
    </section>

    <section class="card-x">
        <div class="d-flex justify-content-between align-items-start mb-3">
            <div>
                <h2 class="mb-0">Próximas llegadas</h2>
                <p class="sub mb-0">Reservas confirmadas con entrada desde hoy.</p>
            </div>
            <a class="btn btn-brass btn-sm" href="Reservas.aspx">
                <i class="bi bi-plus-lg me-1"></i>Nueva reserva
            </a>
        </div>
        <div class="tbl-wrap">
            <asp:GridView ID="gvLlegadas" runat="server"
                AutoGenerateColumns="false" GridLines="None" CssClass="tbl"
                ShowHeaderWhenEmpty="true">
                <EmptyDataTemplate>
                    <div class="empty">
                        <i class="bi bi-calendar2-x"></i>
                        <p>No hay llegadas programadas.<br /><a href="Reservas.aspx">Crear una reserva</a></p>
                    </div>
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="fecha_entrada" HeaderText="Llega" DataFormatString="{0:dd MMM}" />
                    <asp:BoundField DataField="huesped"       HeaderText="Huésped" />
                    <asp:TemplateField HeaderText="Habitación">
                        <ItemTemplate>
                            <b><%# Eval("numero") %></b>
                            <span class="text-secondary"><%# Eval("tipo") %></span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="noches"   HeaderText="Noches" ItemStyle-CssClass="num" />
                    <asp:BoundField DataField="personas" HeaderText="Huésp." ItemStyle-CssClass="num" />
                </Columns>
            </asp:GridView>
        </div>
    </section>
</div>

<%-- ===== Gráfico de ingresos mensuales ===== --%>
<section class="card-x mt-4">
    <h2>Ingresos por mes, <asp:Literal ID="litAnio" runat="server" /></h2>
    <p class="sub">Reservas no canceladas. Haz clic en una barra para ver el mes en Reportes.</p>
    <div class="chart-box"><canvas id="cIng"></canvas></div>
</section>

</asp:Content>

<asp:Content ContentPlaceHolderID="Scripts" runat="server">
<script>
Chart.defaults.color = '#8f9ab3';
Chart.defaults.font.family = 'Manrope';

var cIng = new Chart(document.getElementById('cIng'), {
    type: 'bar',
    data: {
        labels: <%= JsonMeses %>,
        datasets: [{
            label: 'Ingresos (Bs)',
            data: <%= JsonIngresos %>,
            backgroundColor: 'rgba(201,164,92,.75)',
            hoverBackgroundColor: '#c9a45c',
            borderRadius: 7,
            maxBarThickness: 50
        }]
    },
    options: {
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: {
            x: { grid: { display: false } },
            y: {
                grid: { color: '#27334f' },
                ticks: { callback: function(v){ return 'Bs ' + v.toLocaleString('es-BO'); } }
            }
        },
        onClick: function(e, items) {
            /* Al hacer clic en una barra, abre Reportes con el año actual */
            if (items.length) window.location.href = 'Reportes.aspx';
        }
    }
});
</script>
</asp:Content>
