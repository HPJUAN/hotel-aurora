-- =====================================================================
-- HOTEL AURORA - Sistema de Reservas
-- 3.4 DISEÑO FÍSICO: creación de la base de datos (MySQL 8 / MariaDB 10.5+)
-- =====================================================================
-- NOTA alwaysdata / hosting compartido: la base se crea desde el panel
-- (Bases de datos > MySQL > Añadir). En ese caso OMITE las 2 primeras
-- sentencias y ejecuta solo desde USE <tu_base>.
-- =====================================================================
-- ⚠ ATENCIÓN: el nombre 'hotel_reservas' es el nombre LOCAL de desarrollo.
-- En alwaysdata tu base real se llama 'hpjuan_hotel' (creada desde el panel).
-- Si ejecutas desde alwaysdata, OMITE las dos líneas siguientes y
-- selecciona tu base directamente en phpMyAdmin antes de ejecutar.
CREATE DATABASE IF NOT EXISTS hotel_reservas
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_spanish_ci;
USE hotel_reservas;

-- ---------------------------------------------------------------------
-- Ubicación física de archivos de datos y LOG (ejecutar y capturar)
-- ---------------------------------------------------------------------
SHOW VARIABLES LIKE 'datadir';                  -- carpeta de archivos de datos (.ibd por tabla)
SHOW VARIABLES LIKE 'innodb_data_file_path';    -- tablespace del sistema (ibdata1)
SHOW VARIABLES LIKE 'innodb_log_group_home_dir';-- redo log (ib_logfile* / #innodb_redo)
SHOW VARIABLES LIKE 'log_bin_basename';         -- binary log (si está habilitado)
SHOW VARIABLES LIKE 'log_error';                -- log de errores del servidor
SHOW VARIABLES LIKE 'innodb_file_per_table';    -- ON = un archivo .ibd por tabla
