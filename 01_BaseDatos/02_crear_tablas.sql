-- =====================================================================
-- 3.3 DISEÑO LÓGICO ESPECÍFICO: creación de tablas (3FN)
-- =====================================================================
SET FOREIGN_KEY_CHECKS = 0;
DROP VIEW  IF EXISTS vw_reservas_detalle;
DROP TABLE IF EXISTS pago, reserva_servicio, servicio, reserva, huesped, habitacion, tipo_habitacion;
SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE tipo_habitacion (
    id_tipo       INT AUTO_INCREMENT PRIMARY KEY,
    nombre        VARCHAR(40)   NOT NULL,
    descripcion   VARCHAR(200)  NULL,
    capacidad     TINYINT       NOT NULL,
    precio_noche  DECIMAL(10,2) NOT NULL,
    CONSTRAINT uq_tipo_nombre UNIQUE (nombre),
    CONSTRAINT ck_tipo_capacidad CHECK (capacidad BETWEEN 1 AND 10),
    CONSTRAINT ck_tipo_precio    CHECK (precio_noche > 0)
) ENGINE=InnoDB;

CREATE TABLE habitacion (
    id_habitacion INT AUTO_INCREMENT PRIMARY KEY,
    numero        VARCHAR(10) NOT NULL,
    piso          TINYINT     NOT NULL,
    id_tipo       INT         NOT NULL,
    estado        ENUM('Disponible','Mantenimiento') NOT NULL DEFAULT 'Disponible',
    CONSTRAINT uq_habitacion_numero UNIQUE (numero),
    CONSTRAINT fk_habitacion_tipo FOREIGN KEY (id_tipo) REFERENCES tipo_habitacion(id_tipo)
) ENGINE=InnoDB;

CREATE TABLE huesped (
    id_huesped     INT AUTO_INCREMENT PRIMARY KEY,
    ci             VARCHAR(20)  NOT NULL,
    nombres        VARCHAR(60)  NOT NULL,
    apellidos      VARCHAR(60)  NOT NULL,
    email          VARCHAR(100) NULL,
    telefono       VARCHAR(20)  NULL,
    nacionalidad   VARCHAR(40)  NOT NULL DEFAULT 'Boliviana',
    fecha_registro DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT uq_huesped_ci UNIQUE (ci)
) ENGINE=InnoDB;

CREATE TABLE reserva (
    id_reserva     INT AUTO_INCREMENT PRIMARY KEY,
    id_huesped     INT           NOT NULL,
    id_habitacion  INT           NOT NULL,
    fecha_entrada  DATE          NOT NULL,
    fecha_salida   DATE          NOT NULL,
    personas       TINYINT       NOT NULL DEFAULT 1,
    total          DECIMAL(10,2) NOT NULL,
    estado         ENUM('Confirmada','CheckIn','CheckOut','Cancelada') NOT NULL DEFAULT 'Confirmada',
    observaciones  VARCHAR(250)  NULL,
    fecha_creacion DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_reserva_huesped    FOREIGN KEY (id_huesped)    REFERENCES huesped(id_huesped),
    CONSTRAINT fk_reserva_habitacion FOREIGN KEY (id_habitacion) REFERENCES habitacion(id_habitacion),
    CONSTRAINT ck_reserva_fechas   CHECK (fecha_salida > fecha_entrada),
    CONSTRAINT ck_reserva_personas CHECK (personas >= 1),
    CONSTRAINT ck_reserva_total    CHECK (total >= 0),
    INDEX ix_reserva_fechas (id_habitacion, fecha_entrada, fecha_salida),
    INDEX ix_reserva_estado (estado)
) ENGINE=InnoDB;

CREATE TABLE servicio (
    id_servicio INT AUTO_INCREMENT PRIMARY KEY,
    nombre      VARCHAR(60)   NOT NULL,
    precio      DECIMAL(10,2) NOT NULL,
    CONSTRAINT uq_servicio_nombre UNIQUE (nombre),
    CONSTRAINT ck_servicio_precio CHECK (precio >= 0)
) ENGINE=InnoDB;

CREATE TABLE reserva_servicio (
    id_reserva       INT           NOT NULL,
    id_servicio      INT           NOT NULL,
    cantidad         SMALLINT      NOT NULL DEFAULT 1,
    precio_unitario  DECIMAL(10,2) NOT NULL,
    PRIMARY KEY (id_reserva, id_servicio),
    CONSTRAINT fk_rs_reserva  FOREIGN KEY (id_reserva)  REFERENCES reserva(id_reserva) ON DELETE CASCADE,
    CONSTRAINT fk_rs_servicio FOREIGN KEY (id_servicio) REFERENCES servicio(id_servicio),
    CONSTRAINT ck_rs_cantidad CHECK (cantidad > 0)
) ENGINE=InnoDB;

CREATE TABLE pago (
    id_pago     INT AUTO_INCREMENT PRIMARY KEY,
    id_reserva  INT           NOT NULL,
    monto       DECIMAL(10,2) NOT NULL,
    metodo      ENUM('Efectivo','Tarjeta','Transferencia','QR') NOT NULL,
    fecha_pago  DATETIME      NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_pago_reserva FOREIGN KEY (id_reserva) REFERENCES reserva(id_reserva),
    CONSTRAINT ck_pago_monto   CHECK (monto > 0)
) ENGINE=InnoDB;

-- Vista usada por el sistema y por los reportes
CREATE VIEW vw_reservas_detalle AS
SELECT r.id_reserva, r.id_huesped, CONCAT(h.nombres,' ',h.apellidos) AS huesped, h.ci,
       r.id_habitacion, hb.numero, t.nombre AS tipo,
       r.fecha_entrada, r.fecha_salida,
       DATEDIFF(r.fecha_salida, r.fecha_entrada) AS noches,
       r.personas, r.total, r.estado, r.fecha_creacion
FROM reserva r
JOIN huesped h          ON h.id_huesped = r.id_huesped
JOIN habitacion hb      ON hb.id_habitacion = r.id_habitacion
JOIN tipo_habitacion t  ON t.id_tipo = hb.id_tipo;
