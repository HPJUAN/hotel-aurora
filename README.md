# Hotel Aurora – Sistema de Reservas

Sistema de gestión de reservas hoteleras desarrollado como proyecto final universitario.

[![CI](https://github.com/HPJUAN/hotel-aurora/actions/workflows/ci.yml/badge.svg)](https://github.com/HPJUAN/hotel-aurora/actions/workflows/ci.yml)

## Tecnologías

| Capa | Tecnología |
|------|-----------|
| Lenguaje | C# |
| Framework web | ASP.NET Web Forms (.NET Framework 4.7.2) |
| Base de datos | MySQL (ADO.NET, sin ORM) |
| UI | Bootstrap 5 · Chart.js · CSS personalizado |
| Pruebas | NUnit 3 (proyecto .NET 8 multiplataforma) |
| CI | GitHub Actions |

## Estructura del proyecto

```
01_BaseDatos/        Scripts SQL: creación de BD, tablas y datos de prueba
02_Sistema/          Sitio web ASP.NET Web Forms
  App_Code/          Capas: Entidades, Datos (DAO), RN (reglas de negocio)
  Content/           CSS (Site.css)
03_Documentacion/    Documento Word final y diagramas UML
04_Pruebas/          Proyecto NUnit — pruebas de reglas de negocio puras
.github/workflows/   Workflow de integración continua (ci.yml)
```

## Configuración local

### 1. Base de datos (MySQL en alwaysdata)

En phpMyAdmin, selecciona tu base `hpjuan_hotel` y ejecuta en orden:

```sql
01_BaseDatos/02_crear_tablas.sql   -- crea todas las tablas y la vista
01_BaseDatos/03_datos_prueba.sql   -- carga datos iniciales
```

> El script `01_crear_base_datos.sql` es para entorno local con nombre `hotel_reservas`.
> En alwaysdata la base se llama `hpjuan_hotel` y ya existe desde el panel de control.

### 2. Configurar Web.config

`Web.config` **no está en este repositorio** (contiene la contraseña real). Debes crearlo:

```bash
# Windows (PowerShell o cmd)
copy 02_Sistema\Web.config.example 02_Sistema\Web.config
```

Edita `02_Sistema/Web.config` y reemplaza `TUUSUARIO` y `TU_CLAVE` con tus datos de alwaysdata.

### 3. Abrir en Visual Studio

*Archivo → Abrir → Sitio web…* → selecciona la carpeta `02_Sistema` → `Ctrl+F5`

> Si aparece error de ensamblado MySQL: `MySql.Data.dll` debe estar en `02_Sistema/bin/`.
> Descárgalo vía NuGet o cópialo desde tu instalación de MySQL Connector.

## Ejecutar las pruebas

```bash
cd 04_Pruebas
dotnet test --verbosity normal
```

**Resultado esperado:** `37 pruebas · 0 errores · 0 omitidas`

Las pruebas cubren las reglas de negocio puras de `Reglas.cs` sin necesitar conexión a la base de datos.

## Integración continua (GitHub Actions)

En cada push a `main`, GitHub Actions ejecuta automáticamente los 37 tests:

- Ver workflow: [`.github/workflows/ci.yml`](.github/workflows/ci.yml)
- Ver ejecuciones: [Actions](https://github.com/HPJUAN/hotel-aurora/actions)

> El workflow usa .NET 8 (multiplataforma) para ejecutar las pruebas de `04_Pruebas/`.
> La compilación completa del sitio ASP.NET (.NET Framework 4.7.2) se realiza con Visual Studio en Windows.

## Seguridad

- `Web.config` está en `.gitignore` — **nunca subas tu contraseña real**.
- Usa `Web.config.example` como plantilla.
- Todas las consultas SQL utilizan parámetros (`MySqlParameter`) — sin concatenación de strings con datos del usuario.

## Funcionalidades (Sprint 1 — 12 historias de usuario)

| Módulo | Funcionalidades |
|--------|----------------|
| Huéspedes | Registro, búsqueda, edición con validación CI/email/teléfono |
| Habitaciones | CRUD con tipos, piso, estado (Disponible/Mantenimiento) |
| Reservas | Crear con bloqueo de fechas cruzadas (`SELECT FOR UPDATE`), check-in, check-out, cancelación |
| Pagos | Registro de pagos parciales con control de saldo pendiente |
| Dashboard | Tablero de llaves, KPIs de ocupación, próximas llegadas |
| Reportes | Ingresos mensuales, rendimiento por tipo, exportar CSV |
