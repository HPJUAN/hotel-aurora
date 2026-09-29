# Hotel Aurora – Sistema de Reservas (Sprint 1)

ASP.NET Web Forms (.NET Framework 4.7.2) · C# · MySQL · Bootstrap 5 (tema oscuro) · Chart.js

[![CI](https://github.com/HPJUAN/hotel-aurora/actions/workflows/ci.yml/badge.svg)](https://github.com/HPJUAN/hotel-aurora/actions/workflows/ci.yml)

## Estructura

| Carpeta | Contenido |
|---|---|
| `01_BaseDatos` | Scripts SQL: `01_crear_base_datos`, `02_crear_tablas`, `03_datos_prueba` |
| `02_Sistema` | Sitio web ASP.NET Web Forms. Capas en `App_Code`: `Entidades`, `Datos` (DAO) y `RN` (reglas) |
| `03_Documentacion` | Documento del proyecto (Word) y diagramas |
| `04_Pruebas` | Pruebas unitarias NUnit de las reglas de negocio (37 tests) |

## Cómo ejecutarlo localmente

### 1. Base de datos (MySQL en alwaysdata)
En phpMyAdmin, ejecuta en orden:
- `01_BaseDatos/02_crear_tablas.sql`
- `01_BaseDatos/03_datos_prueba.sql`

### 2. Configuración de conexión
El archivo `Web.config` **no está en Git** (contiene tu contraseña). Debes crearlo:

```bash
# Copia la plantilla
cp 02_Sistema/Web.config.example 02_Sistema/Web.config
```

Luego edita `02_Sistema/Web.config` y reemplaza `TUUSUARIO` y `TU_CLAVE` con tus datos reales de alwaysdata.

### 3. Abrir en Visual Studio
*Archivo → Abrir → Sitio web…* → carpeta `02_Sistema` → `Ctrl+F5`

> Si aparece error de ensamblado MySQL: el archivo `MySql.Data.dll` debe estar en `02_Sistema/bin/`. No está en Git; descárgalo vía NuGet o cópialo desde tu instalación.

## Pruebas unitarias

```bash
cd 04_Pruebas
dotnet test --verbosity normal
```

Resultado esperado: **37 tests · 0 errores · 0 omitidos**

## Integración continua

GitHub Actions corre automáticamente los 37 tests en cada push a `main`.  
Ver workflow: [`.github/workflows/ci.yml`](.github/workflows/ci.yml)

> **Nota:** El workflow solo corre las pruebas unitarias (net8.0, multiplataforma).  
> La compilación completa del sitio (.NET Framework 4.7.2) se verifica con Visual Studio en Windows.

## Seguridad

- `Web.config` está en `.gitignore` — **nunca lo subas con tu contraseña real**.
- Usa `Web.config.example` como plantilla.
- Todas las consultas SQL usan parámetros (no hay concatenación de strings con datos del usuario).
