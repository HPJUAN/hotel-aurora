<%@ Page Title="Reportes" Language="C#" MasterPageFile="~/Site.master"
         AutoEventWireup="true" CodeFile="Reportes.aspx.cs" Inherits="Reportes" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<asp:Label ID="lblMsg" runat="server" Visible="false" />

<%-- ===== Barra de filtros ===== --%>
<div class="d-flex align-items-center gap-3 mb-3 flex-wrap">
    <label class="form-label mb-0" for="<%= ddlAnio.ClientID %>">Año</label>
    <asp:DropDownList ID="ddlAnio" runat="server" CssClass="form-select" style="width:120px"
        AutoPostBack="true" OnSelectedIndexChanged="ddlAnio_SelectedIndexChanged" />

    <div class="ms-auto d-flex gap-2">
        <asp:Button ID="btnExportarCsv" runat="server" Text="⬇ Exportar CSV"
            CssClass="btn btn-ghost" CausesValidation="false"
            OnClick="btnExportarCsv_Click" />
        <button type="button" class="btn btn-ghost" onclick="window.print()">
            <i class="bi bi-printer"></i> Imprimir
        </button>
    </div>
</div>

<%-- ===== Indicadores anuales ===== --%>
<div class="stats">
    <div class="stat gold">
        <div class="v"><small>Bs</small> <asp:Literal ID="litTotal"  runat="server" /></div>
        <div class="l">Ingresos del año (sin canceladas)</div>
    </div>
    <div class="stat">
        <div class="v"><asp:Literal ID="litNoches" runat="server" /></div>
        <div class="l">Noches vendidas</div>
    </div>
    <div class="stat">
        <div class="v"><small>Bs</small> <asp:Literal ID="litAdr" runat="server" /></div>
        <div class="l">Tarifa promedio por noche (ADR)</div>
    </div>
    <div class="stat">
        <div class="v"><asp:Literal ID="litCancel" runat="server" /><small>%</small></div>
        <div class="l">Tasa de cancelación</div>
    </div>
</div>

<%-- ===== Reporte 1: ingresos y ocupación mensuales ===== --%>
<section class="card-x mb-3">
    <h2>Reporte 1 · Ingresos y ocupación mensual</h2>
    <p class="sub">Barras: ingresos en Bs. Línea: porcentaje de ocupación del mes.</p>
    <div class="chart-box"><canvas id="cMes"></canvas></div>
    <div class="tbl-wrap mt-3">
        <asp:GridView ID="gvMes" runat="server"
            AutoGenerateColumns="false" GridLines="None" CssClass="tbl"
            ShowHeaderWhenEmpty="true">
            <EmptyDataTemplate>
                <div class="empty">
                    <i class="bi bi-bar-chart"></i>
                    <p>Sin reservas para el año seleccionado.</p>
                </div>
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField DataField="nombre"   HeaderText="Mes" />
                <asp:BoundField DataField="reservas" HeaderText="Reservas"
                    ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                <asp:BoundField DataField="noches"   HeaderText="Noches"
                    ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                <asp:BoundField DataField="ocupacion" HeaderText="Ocupación %"
                    DataFormatString="{0:N1}"
                    ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                <asp:BoundField DataField="ticket"    HeaderText="Ticket Bs"
                    DataFormatString="{0:N2}"
                    ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                <asp:BoundField DataField="ingresos"  HeaderText="Ingresos Bs"
                    DataFormatString="{0:N2}"
                    ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
            </Columns>
        </asp:GridView>
    </div>
</section>

<%-- ===== Reporte 2: por tipo + top huéspedes ===== --%>
<div class="grid-eq">
    <section class="card-x">
        <h2>Reporte 2 · Rendimiento por tipo de habitación</h2>
        <p class="sub">Qué categoría genera más ingresos.</p>
        <div class="chart-box" style="height:250px"><canvas id="cTipo"></canvas></div>
        <div class="tbl-wrap mt-3">
            <asp:GridView ID="gvTipo" runat="server"
                AutoGenerateColumns="false" GridLines="None" CssClass="tbl"
                ShowHeaderWhenEmpty="true">
                <EmptyDataTemplate>
                    <div class="empty"><i class="bi bi-bar-chart-line"></i><p>Sin datos.</p></div>
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="tipo"     HeaderText="Tipo" />
                    <asp:BoundField DataField="reservas" HeaderText="Reservas"
                        ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                    <asp:BoundField DataField="noches"   HeaderText="Noches"
                        ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                    <asp:BoundField DataField="ingresos" HeaderText="Ingresos Bs"
                        DataFormatString="{0:N2}"
                        ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                </Columns>
            </asp:GridView>
        </div>
    </section>

    <section class="card-x">
        <h2>Huéspedes con mayor consumo</h2>
        <p class="sub">Los cinco que más han gastado en el año.</p>
        <div class="tbl-wrap">
            <asp:GridView ID="gvTop" runat="server"
                AutoGenerateColumns="false" GridLines="None" CssClass="tbl"
                ShowHeaderWhenEmpty="true">
                <EmptyDataTemplate>
                    <div class="empty">
                        <i class="bi bi-people"></i>
                        <p>Sin reservas en el año seleccionado.</p>
                    </div>
                </EmptyDataTemplate>
                <Columns>
                    <asp:BoundField DataField="huesped"  HeaderText="Huésped" />
                    <asp:BoundField DataField="reservas" HeaderText="Reservas"
                        ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                    <asp:BoundField DataField="noches"   HeaderText="Noches"
                        ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                    <asp:BoundField DataField="gastado"  HeaderText="Gastado Bs"
                        DataFormatString="{0:N2}"
                        ItemStyle-CssClass="num" HeaderStyle-CssClass="num" />
                </Columns>
            </asp:GridView>
        </div>
    </section>
</div>
</asp:Content>

<asp:Content ContentPlaceHolderID="Scripts" runat="server">
<script>
Chart.defaults.color = '#8f9ab3';
Chart.defaults.font.family = 'Manrope';
var grid = { color: '#27334f' };

new Chart(document.getElementById('cMes'), {
    data: {
        labels: <%= JsonMeses %>,
        datasets: [
            {
                type: 'bar', label: 'Ingresos (Bs)',
                data: <%= JsonIngresos %>,
                backgroundColor: 'rgba(201,164,92,.75)',
                hoverBackgroundColor: '#c9a45c',
                borderRadius: 6, maxBarThickness: 44, yAxisID: 'y'
            },
            {
                type: 'line', label: 'Ocupación (%)',
                data: <%= JsonOcupacion %>,
                borderColor: '#5bb5a2', backgroundColor: 'rgba(91,181,162,.08)',
                fill: true, tension: .35, pointRadius: 4,
                pointBackgroundColor: '#5bb5a2', yAxisID: 'y1'
            }
        ]
    },
    options: {
        maintainAspectRatio: false,
        interaction: { mode: 'index', intersect: false },
        plugins: { legend: { position: 'bottom', labels: { boxWidth: 12 } } },
        scales: {
            x:  { grid: { display: false } },
            y:  { grid: grid, ticks: { callback: function(v){ return 'Bs ' + v.toLocaleString('es-BO'); } } },
            y1: { position: 'right', min: 0, max: 100, grid: { display: false },
                  ticks: { callback: function(v){ return v + '%'; } } }
        }
    }
});

new Chart(document.getElementById('cTipo'), {
    type: 'doughnut',
    data: {
        labels: <%= JsonTipos %>,
        datasets: [{
            data: <%= JsonIngTipo %>,
            backgroundColor: ['#c9a45c','#5bb5a2','#7a9be0','#d9736b','#b08fd4'],
            borderColor: '#161e31', borderWidth: 3
        }]
    },
    options: {
        maintainAspectRatio: false,
        cutout: '62%',
        plugins: { legend: { position: 'right', labels: { boxWidth: 12 } } }
    }
});
</script>
</asp:Content>
