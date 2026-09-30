-- =====================================================================
-- evidencias_bd.sql — Consultas de evidencia para phpMyAdmin
-- Hotel Aurora · Base: hpjuan_hotel (alwaysdata)
-- Ejecuta cada bloque por separado en phpMyAdmin → pestaña SQL.
-- NO modifica ningún dato; todas son consultas SELECT / SHOW.
-- =====================================================================

-- -----------------------------------------------------------------------
-- 1. TABLAS EXISTENTES EN LA BASE
-- -----------------------------------------------------------------------
SHOW TABLES;

-- -----------------------------------------------------------------------
-- 2. CANTIDAD DE REGISTROS POR TABLA
-- -----------------------------------------------------------------------
SELECT 'tipo_habitacion' AS tabla, COUNT(*) AS registros FROM tipo_habitacion
UNION ALL
SELECT 'habitacion',       COUNT(*) FROM habitacion
UNION ALL
SELECT 'huesped',          COUNT(*) FROM huesped
UNION ALL
SELECT 'reserva',          COUNT(*) FROM reserva
UNION ALL
SELECT 'servicio',         COUNT(*) FROM servicio
UNION ALL
SELECT 'reserva_servicio', COUNT(*) FROM reserva_servicio
UNION ALL
SELECT 'pago',             COUNT(*) FROM pago;

-- -----------------------------------------------------------------------
-- 3. ESTRUCTURA COMPLETA (SHOW CREATE TABLE)
-- -----------------------------------------------------------------------
SHOW CREATE TABLE tipo_habitacion;
SHOW CREATE TABLE habitacion;
SHOW CREATE TABLE huesped;
SHOW CREATE TABLE reserva;
SHOW CREATE TABLE servicio;
SHOW CREATE TABLE reserva_servicio;
SHOW CREATE TABLE pago;

-- -----------------------------------------------------------------------
-- 4. ÍNDICES DE CADA TABLA
-- -----------------------------------------------------------------------
SHOW INDEX FROM tipo_habitacion;
SHOW INDEX FROM habitacion;
SHOW INDEX FROM huesped;
SHOW INDEX FROM reserva;
SHOW INDEX FROM servicio;
SHOW INDEX FROM reserva_servicio;
SHOW INDEX FROM pago;

-- -----------------------------------------------------------------------
-- 5. RELACIONES (CLAVES FORÁNEAS) vía information_schema
-- -----------------------------------------------------------------------
SELECT
    kcu.TABLE_NAME        AS tabla,
    kcu.COLUMN_NAME       AS columna,
    kcu.CONSTRAINT_NAME   AS restriccion,
    kcu.REFERENCED_TABLE_NAME  AS ref_tabla,
    kcu.REFERENCED_COLUMN_NAME AS ref_columna,
    rc.DELETE_RULE,
    rc.UPDATE_RULE
FROM information_schema.KEY_COLUMN_USAGE kcu
JOIN information_schema.REFERENTIAL_CONSTRAINTS rc
    ON rc.CONSTRAINT_NAME   = kcu.CONSTRAINT_NAME
    AND rc.CONSTRAINT_SCHEMA = kcu.TABLE_SCHEMA
WHERE kcu.TABLE_SCHEMA = DATABASE()
  AND kcu.REFERENCED_TABLE_NAME IS NOT NULL
ORDER BY kcu.TABLE_NAME, kcu.COLUMN_NAME;

-- -----------------------------------------------------------------------
-- 6. VISTA vw_reservas_detalle
-- -----------------------------------------------------------------------
-- 6a. Definición de la vista
SHOW CREATE VIEW vw_reservas_detalle;

-- 6b. Primeras 10 filas de la vista
SELECT * FROM vw_reservas_detalle ORDER BY fecha_entrada DESC LIMIT 10;

-- -----------------------------------------------------------------------
-- 7. RESTRICCIONES CHECK (information_schema — MySQL 8+)
-- -----------------------------------------------------------------------
SELECT
    tc.TABLE_NAME    AS tabla,
    cc.CONSTRAINT_NAME AS restriccion,
    cc.CHECK_CLAUSE    AS condicion
FROM information_schema.TABLE_CONSTRAINTS tc
JOIN information_schema.CHECK_CONSTRAINTS cc
    ON cc.CONSTRAINT_NAME   = tc.CONSTRAINT_NAME
    AND cc.CONSTRAINT_SCHEMA = tc.TABLE_SCHEMA
WHERE tc.TABLE_SCHEMA = DATABASE()
  AND tc.CONSTRAINT_TYPE = 'CHECK'
ORDER BY tc.TABLE_NAME, cc.CONSTRAINT_NAME;

-- -----------------------------------------------------------------------
-- 8. MOTOR, COLLATION Y UTF8MB4 DE CADA TABLA
-- -----------------------------------------------------------------------
SELECT
    TABLE_NAME      AS tabla,
    ENGINE          AS motor,
    TABLE_COLLATION AS collation_tabla,
    TABLE_ROWS      AS filas_aprox
FROM information_schema.TABLES
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_TYPE   = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- -----------------------------------------------------------------------
-- 9. CASO ESPECIAL: intento de reserva con fechas cruzadas
--    (muestra cómo el índice ix_reserva_fechas protege la consulta
--    de detección de solapamiento usada en ReservaDAO.InsertarSiDisponible)
-- -----------------------------------------------------------------------
-- Reservas activas para la habitación 1 que se solaparían
-- con un intento de entrada 2026-10-01 → salida 2026-10-05:
SELECT r.id_reserva, r.id_habitacion, r.fecha_entrada, r.fecha_salida, r.estado
FROM reserva r
WHERE r.id_habitacion = 1          -- ajusta al id real
  AND r.estado <> 'Cancelada'
  AND r.fecha_entrada < '2026-10-05'
  AND r.fecha_salida  > '2026-10-01';
-- Si devuelve 0 filas, la habitación está libre en esas fechas.
-- Si devuelve ≥ 1 fila, InsertarSiDisponible rechazaría la reserva.
