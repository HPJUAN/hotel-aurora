# EVIDENCIAS.md — Hotel Aurora
## Sistema de Reservas · Sprint 1 · Proyecto Final Universitario

---

## 1. PRUEBAS UNITARIAS EJECUTADAS

**Comando:**
```bash
cd 04_Pruebas
dotnet test --verbosity normal
```

**Resultado verificado el 28/09/2026 (salida real de `dotnet test`):**

| Métrica | Valor |
|---------|-------|
| Total de tests | **37** |
| Aprobados | **37** |
| Fallidos | 0 |
| Omitidos | 0 |
| Duración | ~5 s |
| Framework | NUnit 3.14.0 sobre .NET 8 (proyecto `04_Pruebas`). El sitio es .NET Framework 4.7.2. |

**Grupos de tests (todos en `Reglas.cs`, sin dependencia de BD):**

| Grupo | Tests | Casos límite cubiertos |
|-------|-------|------------------------|
| Noches y total | 3 | cálculo correcto, cero noches lanza excepción |
| Validación de fechas | 7 | entrada=hoy ✓, entrada pasada ✗, salida=entrada ✗, salida<entrada ✗, 30 noches ✓, 31 noches ✗ |
| Capacidad | 4 TestCases | persona 0 ✗, 1 ✓, igual cap. ✓, supera cap. ✗ |
| Transiciones de estado | 8 TestCases | todas las aristas del ciclo de vida (incl. CheckOut→Cancelada) |
| Validación de teléfono | 9 TestCases | vacío ✓, formatos internacionales ✓, corto ✗, letras ✗ |
| Monto de pago | 6 | cero ✗, negativo ✗, válido ✓, igual saldo ✓, supera saldo ✗, saldo cero ✗ |

---

## 2. ARCHIVOS MODIFICADOS (trabajo técnico real)

### Pruebas y XP
| Archivo | Cambio |
|---------|--------|
| `04_Pruebas/ReglasTests.cs` | +3 tests: `SalidaMenorAEntrada`, `Exactamente30Noches`, TestCase `CheckOut→Cancelada` |

### Git y CI
| Archivo | Cambio |
|---------|--------|
| `.gitignore` | Reescrito: excluye `bin/`, `02_Sistema/Web.config`, `TestResults/` |
| `02_Sistema/Web.config.example` | Plantilla con `TUUSUARIO`/`TU_CLAVE` (sin credenciales reales) |
| `.github/workflows/ci.yml` | `dotnet restore` explícito, rama `main`, comentarios de compatibilidad |
| `README.md` | Badge CI real, instrucciones `Web.config.example` |

### Base de datos
| Archivo | Cambio |
|---------|--------|
| `01_BaseDatos/evidencias_bd.sql` | 9 bloques SELECT/SHOW para phpMyAdmin (solo lectura) |
| `01_BaseDatos/01_crear_base_datos.sql` | Advertencia sobre `hotel_reservas` vs `hpjuan_hotel` |

### Demostración
| Archivo | Cambio |
|---------|--------|
| `01_BaseDatos/datos_demo.sql` | Script de demo sin borrar datos existentes |
| `EVIDENCIAS.md` | Este archivo |

---

## 3. ALCANCE DEL SPRINT 1

El Sprint 1 duró 3 días (ritmo de proyecto universitario intensivo) e incluye **12 historias de usuario (HU-01 a HU-12), 55 puntos de historia**.

| HU | Historia | Estado |
|----|----------|--------|
| HU-01 | Registrar huésped | ✅ Implementado |
| HU-02 | Registrar habitación | ✅ Implementado |
| HU-03 | Crear reserva (con bloqueo de fechas cruzadas) | ✅ Implementado |
| HU-04 | Consultar disponibilidad | ✅ Implementado |
| HU-05 | Tablero de estado de habitaciones | ✅ Implementado |
| HU-06 | Indicadores del dashboard | ✅ Implementado |
| HU-07 | Check-in y Check-out | ✅ Implementado |
| HU-08 | Cancelación de reserva | ✅ Implementado |
| HU-09 | Reporte de ingresos mensuales | ✅ Implementado |
| HU-10 | Reporte por tipo de habitación | ✅ Implementado |
| HU-11 | Próximas llegadas | ✅ Implementado |
| HU-12 | Registrar pago | ✅ Implementado |
| HU-13 | Servicios adicionales | ⏳ Trabajo posterior (modelo de datos listo, sin UI) |
| HU-14 | Reportes avanzados | ⏳ Trabajo posterior |
| HU-15 | Publicación en servidor externo | ⏳ Trabajo posterior |

---

## 4. INCONSISTENCIAS: DOCUMENTO vs. SISTEMA

| Historia | Posición original en documento | Estado real | Acción requerida |
|----------|--------------------------------|-------------|-----------------|
| **HU-07** Check-in / Check-out | Trabajo posterior | ✅ Implementado en Sprint 1 | Corregir numeración en el Word |
| **HU-08** Cancelación | Trabajo posterior | ✅ Implementado en Sprint 1 | Verificar en el Word |
| **HU-12** Registrar pago | Trabajo posterior | ✅ Implementado en Sprint 1 | Mover al Sprint 1 en el Word |
| **HU-13** Servicios | Trabajo posterior | ⚠️ Parcial (tablas sin UI) | Dejar como trabajo posterior |

> **Nota sobre nombres de BD:** El script `01_crear_base_datos.sql` usa `hotel_reservas` (nombre local de desarrollo). En alwaysdata la base se llama `hpjuan_hotel`. Esta diferencia está anotada en el script y no afecta al funcionamiento — el sistema usa la cadena de conexión de `Web.config`.

---

## 5. AFIRMACIONES SCRUM/XP — ESTADO DE EVIDENCIA

| Afirmación | Estado | Observación |
|-----------|--------|-------------|
| Sprint 1 de 3 días con 12 historias (HU-01 a HU-12), 55 puntos | ✅ Documentado | Verificable en el Word y en el código |
| Pruebas unitarias NUnit (XP — TDD) | ✅ Verificado | 37/37 correctas, salida real de `dotnet test` |
| Integración continua (XP) | ⏳ **PENDIENTE DE EVIDENCIA** | El workflow existe; badge se verificará tras el push a GitHub |
| Commits atómicos sobre `main` | ⏳ **PENDIENTE DE EVIDENCIA** | Los commits se harán en la Fase 3 del proceso de Git |
| Refactorización documentada | ⏳ **PENDIENTE DE EVIDENCIA** | Se propusieron 3 refactorizaciones; ninguna se ha commiteado aún como `refactor:` |
| Tablero de seguimiento Scrum | ⏳ **PENDIENTE DE EVIDENCIA** | No hay Trello ni burndown registrado; el estudiante lo crea manualmente |
| Burndown real del sprint | ⏳ **PENDIENTE DE EVIDENCIA** | No fue registrado durante el desarrollo |
| Daily standup / retrospectiva | ⏳ **PENDIENTE DE EVIDENCIA** | No hay actas; si el docente lo exige, añadir al Word |
| Trabajo individual (sin programación en parejas) | ✅ | No se afirma en ningún artefacto |
| Sin ramas por historia, sin etiquetas de versión | ✅ | Todo va en commits sobre `main` |

---

## 6. CRITERIOS DE ACEPTACIÓN Y GUIÓN DE DEMO (HU-01 a HU-12)

| HU | Historia | Criterio de aceptación | Pasos en vivo | Captura a tomar |
|----|----------|------------------------|---------------|-----------------|
| HU-01 | Registrar huésped | Huésped guardado con CI único; CI duplicado muestra error | Ir a Huéspedes → formulario → guardar → toast verde; repetir mismo CI → error | Formulario con toast ✓ y error de duplicado |
| HU-02 | Registrar habitación | Habitación guardada con número único y tipo válido | Ir a Habitaciones → formulario → guardar → aparece en lista | Lista con nueva habitación |
| HU-03 | Crear reserva sin solapamiento | Reserva creada; intento de fechas cruzadas rechazado | Reservar hab 103 → confirmar; intentar mismas fechas en hab 201 (ocupada) → error | Toast de éxito + mensaje de rechazo |
| HU-04 | Consultar disponibilidad | Solo aparecen habitaciones libres en las fechas pedidas | Nueva reserva → cambiar fechas → "Ver habitaciones disponibles" → lista filtrada | Lista de disponibles |
| HU-05 | Tablero de estado | Cada habitación muestra su estado con color; tooltip con huésped | Inicio → Tablero de llaves → llave dorada → tooltip | Tablero con tooltip visible |
| HU-06 | Indicadores del dashboard | % ocupación, habitaciones libres, reservas activas, ingresos del mes | Inicio → franja de 4 indicadores | Bloque de KPIs |
| HU-07 | Check-in y Check-out | Estado: Confirmada→CheckIn→CheckOut; CheckIn solo desde fecha de entrada | Reservas → botón CI → CheckIn; luego CO → CheckOut | Antes y después del estado |
| HU-08 | Cancelación | Confirmada o CheckIn → Cancelada; CheckOut no se puede cancelar | Reservas → botón Cancelar en reserva Confirmada → estado Cancelada | Cambio de estado a Cancelada |
| HU-09 | Reporte de ingresos mensuales | Gráfico barras+línea por mes; tabla con ticket promedio | Reportes → año 2026 → gráfico + tabla | Gráfico completo |
| HU-10 | Reporte por tipo de habitación | Gráfico doughnut con participación por tipo | Misma página → sección doughnut | Gráfico doughnut |
| HU-11 | Próximas llegadas | Lista de hasta 8 reservas Confirmadas desde hoy | Inicio → "Próximas llegadas" → reserva de Ana Demo visible | Tabla de próximas llegadas |
| HU-12 | Registrar pago | Pago guardado; monto > saldo muestra error | Reservas → 💰 en reserva CheckIn → monto → registrar; monto mayor → error | Panel de pago + error de validación |

---

## 7. LIMITACIONES CONOCIDAS

| Código | Descripción | Impacto en la demo |
|--------|-------------|-------------------|
| L-01 | Reportes asignan el total al mes de `fecha_entrada`. Reservas que cruzan meses se contabilizan íntegras en el mes de inicio | Bajo — mencionar como limitación conocida |
| L-02 | Tablas `servicio`/`reserva_servicio` sin UI de gestión | Ninguno — mostrar estructura en phpMyAdmin |
| L-03 | `hotel_reservas` en script local ≠ `hpjuan_hotel` en alwaysdata | Ninguno — aclarado en el script y en este documento |

---

## 8. PENDIENTES MANUALES (los hace el estudiante)

- [ ] **Verificar badge CI verde** en https://github.com/HPJUAN/hotel-aurora/actions — ⏳ PENDIENTE DE EVIDENCIA
- [ ] **Tomar captura de `git log --oneline`** tras el push — ⏳ PENDIENTE DE EVIDENCIA
- [ ] **Tomar captura de GitHub Actions** mostrando el workflow verde — ⏳ PENDIENTE DE EVIDENCIA
- [ ] **Ejecutar `datos_demo.sql`** en phpMyAdmin antes de cada demo o defensa
- [ ] **Crear tablero Trello** con las 12 HU del Sprint 1 (HU-01 a HU-12) y tomar captura
- [ ] **Actualizar el documento Word**: corregir numeración HU-07, HU-08, HU-12 y moverlas al Sprint 1
- [ ] **Tomar las capturas de pantalla** del sistema en vivo listadas en la guía de demo
- [ ] **Publicar en alwaysdata** y verificar que la URL pública responde

---

*Revisión técnica completada el 28/09/2026. Git y documentación actualizados el 29/09/2026.*
