-- =====================================================================
-- datos_demo.sql — Script de demostración (10 minutos)
-- Hotel Aurora · Base: hpjuan_hotel
-- ¡IMPORTANTE! Este script NO borra datos existentes.
-- Inserta filas con IDs controlados solo si no existen.
-- Ejecuta en phpMyAdmin o MySQL Workbench antes de la demo.
-- =====================================================================

-- -----------------------------------------------------------------------
-- PASO 0: Verificar estado antes de la demo
-- -----------------------------------------------------------------------
SELECT 'ESTADO PREVIO A LA DEMO' AS info;
SELECT 'tipo_habitacion' AS tabla, COUNT(*) AS registros FROM tipo_habitacion
UNION ALL SELECT 'habitacion',  COUNT(*) FROM habitacion
UNION ALL SELECT 'huesped',     COUNT(*) FROM huesped
UNION ALL SELECT 'reserva',     COUNT(*) FROM reserva
UNION ALL SELECT 'pago',        COUNT(*) FROM pago;

-- -----------------------------------------------------------------------
-- PASO 1: Huésped de demo (HU-01 — Registrar huésped)
-- Usamos CI ficticio 'DEMO-001' para no colisionar con datos reales.
-- -----------------------------------------------------------------------
INSERT INTO huesped (ci, nombres, apellidos, email, telefono, nacionalidad)
SELECT 'DEMO-001','Ana','Demo Evaluador','demo@hotel-aurora.edu','79000001','Boliviana'
WHERE NOT EXISTS (SELECT 1 FROM huesped WHERE ci = 'DEMO-001');

INSERT INTO huesped (ci, nombres, apellidos, email, telefono, nacionalidad)
SELECT 'DEMO-002','Luis','Demo Conflicto','demo2@hotel-aurora.edu','79000002','Boliviana'
WHERE NOT EXISTS (SELECT 1 FROM huesped WHERE ci = 'DEMO-002');

-- -----------------------------------------------------------------------
-- PASO 2: Reservas de demo (HU-03, HU-04, HU-08, HU-07)
--
-- Usamos la habitación 103 (Doble, piso 1) para la demo principal.
-- Fechas: desde HOY + 1 día, para que aparezca en "Próximas llegadas".
-- -----------------------------------------------------------------------

-- 2a. Reserva principal: Ana Demo, hab 103, entrada mañana, 3 noches (HU-03)
INSERT INTO reserva (id_huesped, id_habitacion, fecha_entrada, fecha_salida, personas, total, estado)
SELECT
    (SELECT id_huesped FROM huesped WHERE ci = 'DEMO-001'),
    (SELECT id_habitacion FROM habitacion WHERE numero = '103'),
    DATE_ADD(CURDATE(), INTERVAL 1 DAY),
    DATE_ADD(CURDATE(), INTERVAL 4 DAY),
    2,
    1260.00,
    'Confirmada'
WHERE NOT EXISTS (
    SELECT 1 FROM reserva r
    WHERE r.id_habitacion = (SELECT id_habitacion FROM habitacion WHERE numero = '103')
      AND r.estado <> 'Cancelada'
      AND r.fecha_entrada < DATE_ADD(CURDATE(), INTERVAL 4 DAY)
      AND r.fecha_salida  > DATE_ADD(CURDATE(), INTERVAL 1 DAY)
);

-- 2b. Reserva con Check-in activo HOY: Luis Demo, hab 201 (HU-08)
INSERT INTO reserva (id_huesped, id_habitacion, fecha_entrada, fecha_salida, personas, total, estado)
SELECT
    (SELECT id_huesped FROM huesped WHERE ci = 'DEMO-002'),
    (SELECT id_habitacion FROM habitacion WHERE numero = '201'),
    CURDATE(),
    DATE_ADD(CURDATE(), INTERVAL 2 DAY),
    2,
    840.00,
    'CheckIn'
WHERE NOT EXISTS (
    SELECT 1 FROM reserva r
    WHERE r.id_habitacion = (SELECT id_habitacion FROM habitacion WHERE numero = '201')
      AND r.estado <> 'Cancelada'
      AND r.fecha_entrada < DATE_ADD(CURDATE(), INTERVAL 2 DAY)
      AND r.fecha_salida  > CURDATE()
);

-- 2c. Registrar un pago parcial para la reserva de Luis (HU-07)
INSERT INTO pago (id_reserva, monto, metodo, fecha_pago)
SELECT
    (SELECT id_reserva FROM reserva r
     JOIN huesped h ON h.id_huesped = r.id_huesped
     WHERE h.ci = 'DEMO-002' AND r.estado = 'CheckIn' LIMIT 1),
    400.00,
    'Efectivo',
    NOW()
WHERE EXISTS (
    SELECT 1 FROM reserva r
    JOIN huesped h ON h.id_huesped = r.id_huesped
    WHERE h.ci = 'DEMO-002' AND r.estado = 'CheckIn'
);

-- -----------------------------------------------------------------------
-- PASO 3: Caso de fechas cruzadas — para demostrar HU-03 (integridad)
-- NO se puede insertar directamente porque viola la regla de negocio.
-- La demo se hace desde la UI: intentar reservar hab 201 con fechas
-- que se solapan con la reserva de Luis → el sistema rechaza.
-- Este SELECT muestra qué reserva activa colisionaría:
-- -----------------------------------------------------------------------
SELECT
    'CONFLICTO DEMO (hab 201)' AS escenario,
    r.id_reserva,
    h.ci,
    CONCAT(h.nombres,' ',h.apellidos) AS huesped,
    r.fecha_entrada,
    r.fecha_salida,
    r.estado
FROM reserva r
JOIN huesped h ON h.id_huesped = r.id_huesped
WHERE r.id_habitacion = (SELECT id_habitacion FROM habitacion WHERE numero = '201')
  AND r.estado <> 'Cancelada'
  AND r.fecha_entrada < DATE_ADD(CURDATE(), INTERVAL 2 DAY)
  AND r.fecha_salida  > CURDATE();

-- -----------------------------------------------------------------------
-- PASO 4: Verificar estado final para la demo
-- -----------------------------------------------------------------------
SELECT 'ESTADO LISTO PARA DEMO' AS info;
SELECT * FROM vw_reservas_detalle
WHERE fecha_entrada >= CURDATE()
  AND estado IN ('Confirmada','CheckIn')
ORDER BY fecha_entrada
LIMIT 10;

-- -----------------------------------------------------------------------
-- LIMPIEZA OPCIONAL (ejecutar SOLO después de la demo si quieres revertir)
-- Descomenta las líneas siguientes SOLO si quieres limpiar los datos demo.
-- -----------------------------------------------------------------------
-- DELETE FROM pago    WHERE id_reserva IN (SELECT id_reserva FROM reserva r JOIN huesped h ON h.id_huesped=r.id_huesped WHERE h.ci IN ('DEMO-001','DEMO-002'));
-- DELETE FROM reserva WHERE id_huesped IN (SELECT id_huesped FROM huesped WHERE ci IN ('DEMO-001','DEMO-002'));
-- DELETE FROM huesped WHERE ci IN ('DEMO-001','DEMO-002');
