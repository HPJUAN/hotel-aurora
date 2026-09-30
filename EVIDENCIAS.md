# EVIDENCIAS — Hotel Aurora

## Pruebas unitarias

**Comando de ejecución:**
```bash
cd 04_Pruebas
dotnet test --verbosity normal
```

**Resultado verificado (28/09/2026):**

| Métrica | Valor |
|---------|-------|
| Total | **37** |
| Aprobadas | **37** |
| Fallidas | 0 |
| Omitidas | 0 |
| Duración | ~5 s |
| Framework | NUnit 3.14.0 · proyecto net8.0 · sitio .NET Framework 4.7.2 |

**Cobertura por grupo:**

| Grupo | Tests | Casos cubiertos |
|-------|-------|----------------|
| Noches y cálculo de total | 3 | Cálculo correcto · 0 noches lanza excepción |
| Validación de fechas | 7 | Entrada=hoy ✓ · Entrada pasada ✗ · Salida=Entrada ✗ · Salida<Entrada ✗ · 30 noches ✓ · 31 noches ✗ |
| Capacidad de habitación | 4 | Persona 0 ✗ · 1 ✓ · Igual cap. ✓ · Supera cap. ✗ |
| Transiciones de estado | 8 | Todas las aristas del ciclo de vida (incl. CheckOut→Cancelada) |
| Validación de teléfono | 9 | Formatos nacionales e internacionales ✓ · Vacío ✓ · Corto ✗ · Con letras ✗ |
| Monto de pago | 6 | Cero ✗ · Negativo ✗ · Válido ✓ · Igual saldo ✓ · Supera saldo ✗ · Saldo cero ✗ |

---

## Alcance del Sprint 1

Duración: 3 días (ritmo intensivo). **12 historias de usuario completadas.**

| HU | Historia | Estado |
|----|----------|--------|
| HU-01 | Registrar huésped (CI único, validación email/teléfono) | ✅ |
| HU-02 | Registrar habitación (tipo, piso, estado) | ✅ |
| HU-03 | Crear reserva con bloqueo de fechas cruzadas | ✅ |
| HU-04 | Consultar disponibilidad por fechas y personas | ✅ |
| HU-05 | Tablero de estado de habitaciones (rack de llaves) | ✅ |
| HU-06 | Indicadores del dashboard (ocupación, ingresos, reservas) | ✅ |
| HU-07 | Check-in y Check-out con ciclo de vida validado | ✅ |
| HU-08 | Cancelación de reserva | ✅ |
| HU-09 | Reporte de ingresos mensuales con gráfico | ✅ |
| HU-10 | Reporte de rendimiento por tipo de habitación | ✅ |
| HU-11 | Próximas llegadas (8 reservas Confirmadas desde hoy) | ✅ |
| HU-12 | Registrar pago con control de saldo pendiente | ✅ |
| HU-13 | Servicios adicionales | ⏳ Trabajo posterior (modelo de datos listo, sin UI) |
| HU-14 | Reportes avanzados | ⏳ Trabajo posterior |
| HU-15 | Publicación en servidor externo | ⏳ Trabajo posterior |

---

## Guión de demostración en vivo

Ejecutar `01_BaseDatos/datos_demo.sql` en phpMyAdmin antes de la demo.

| HU | Qué demostrar | Pasos | Captura |
|----|--------------|-------|---------|
| HU-01 | Registrar huésped y validar CI duplicado | Huéspedes → formulario → guardar → toast; repetir mismo CI → error | Formulario + toast + error |
| HU-02 | Registrar habitación | Habitaciones → formulario → guardar → aparece en lista | Lista con nueva hab. |
| HU-03 | Reserva sin solapamiento | Reservar hab 103 → confirmar; intentar hab 201 con mismas fechas → error de bloqueo | Toast éxito + error solapamiento |
| HU-04 | Consultar disponibilidad | Nueva reserva → fechas → "Ver disponibles" → solo libres | Lista filtrada |
| HU-05 | Tablero de llaves | Inicio → rack → llave dorada → tooltip con nombre del huésped | Tablero con tooltip |
| HU-06 | KPIs del dashboard | Inicio → 4 indicadores (% ocupación, libres, reservas, ingresos) | Bloque de KPIs |
| HU-07 | Check-in y Check-out | Reservas → botón CI → CheckIn; botón CO → CheckOut | Cambios de estado |
| HU-08 | Cancelación | Reservas → botón Cancelar en reserva Confirmada → estado Cancelada | Estado cancelado |
| HU-09 | Reporte mensual | Reportes → año 2026 → gráfico barras+línea + tabla | Gráfico completo |
| HU-10 | Reporte por tipo | Misma página → gráfico doughnut | Gráfico doughnut |
| HU-11 | Próximas llegadas | Inicio → sección llegadas → reserva demo visible | Tabla de llegadas |
| HU-12 | Registrar pago | Reservas → 💰 → monto → registrar; monto mayor → error | Panel pago + error |

---

## Limitaciones conocidas

| Código | Descripción | Impacto |
|--------|-------------|---------|
| L-01 | Los reportes asignan el total al mes de `fecha_entrada`. Reservas que cruzan meses (ej. 28/09–05/10) se contabilizan íntegras en el mes de inicio | Bajo — simplificación documentada |
| L-02 | Tablas `servicio` y `reserva_servicio` sin interfaz de gestión UI | Ninguno — datos precargados en `03_datos_prueba.sql` |
| L-03 | El script `01_crear_base_datos.sql` usa `hotel_reservas` (desarrollo local). En alwaysdata la base se llama `hpjuan_hotel` | Ninguno — anotado en el script |

---

## Integración continua

- Workflow: `.github/workflows/ci.yml`
- Se ejecuta en cada push a `main`
- Corre los 37 tests con `dotnet test` en Ubuntu (net8.0)
- Estado actual: verificar en https://github.com/HPJUAN/hotel-aurora/actions
