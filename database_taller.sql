IF DB_ID(N'DBtaller') IS NULL
BEGIN
  CREATE DATABASE DBtaller;
END;
GO

USE DBtaller;
GO

CREATE TABLE dbo.rol (
  id_rol INT IDENTITY(1,1) NOT NULL,
  nombre NVARCHAR(50) NOT NULL,
  descripcion NVARCHAR(255) NULL,
  creado_en DATETIME2 NOT NULL CONSTRAINT df_rol_creado_en DEFAULT SYSDATETIME(),
  CONSTRAINT pk_rol PRIMARY KEY (id_rol),
  CONSTRAINT uq_rol_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.usuario (
  id_usuario INT IDENTITY(1,1) NOT NULL,
  nombre NVARCHAR(120) NOT NULL,
  username NVARCHAR(50) NOT NULL,
  correo NVARCHAR(120) NOT NULL,
  password_hash NVARCHAR(255) NOT NULL,
  activo BIT NOT NULL CONSTRAINT df_usuario_activo DEFAULT 1,
  id_rol INT NOT NULL,
  creado_en DATETIME2 NOT NULL CONSTRAINT df_usuario_creado_en DEFAULT SYSDATETIME(),
  actualizado_en DATETIME2 NOT NULL CONSTRAINT df_usuario_actualizado_en DEFAULT SYSDATETIME(),
  CONSTRAINT pk_usuario PRIMARY KEY (id_usuario),
  CONSTRAINT uq_usuario_username UNIQUE (username),
  CONSTRAINT uq_usuario_correo UNIQUE (correo),
  CONSTRAINT fk_usuario_rol FOREIGN KEY (id_rol)
    REFERENCES dbo.rol(id_rol)
    ON UPDATE CASCADE
);
GO

CREATE TABLE dbo.vehiculo (
  id_vehiculo INT IDENTITY(1,1) NOT NULL,
  placa NVARCHAR(20) NOT NULL,
  marca NVARCHAR(80) NOT NULL,
  modelo NVARCHAR(80) NOT NULL,
  anio SMALLINT NOT NULL,
  kilometraje INT NOT NULL CONSTRAINT df_vehiculo_kilometraje DEFAULT 0,
  activo BIT NOT NULL CONSTRAINT df_vehiculo_activo DEFAULT 1,
  creado_en DATETIME2 NOT NULL CONSTRAINT df_vehiculo_creado_en DEFAULT SYSDATETIME(),
  actualizado_en DATETIME2 NOT NULL CONSTRAINT df_vehiculo_actualizado_en DEFAULT SYSDATETIME(),
  CONSTRAINT pk_vehiculo PRIMARY KEY (id_vehiculo),
  CONSTRAINT uq_vehiculo_placa UNIQUE (placa),
  CONSTRAINT chk_vehiculo_anio CHECK (anio BETWEEN 1900 AND 2100),
  CONSTRAINT chk_vehiculo_kilometraje CHECK (kilometraje >= 0)
);
GO

CREATE TABLE dbo.proveedor (
  id_proveedor INT IDENTITY(1,1) NOT NULL,
  nombre NVARCHAR(120) NOT NULL,
  telefono NVARCHAR(25) NULL,
  direccion NVARCHAR(255) NULL,
  correo NVARCHAR(120) NULL,
  activo BIT NOT NULL CONSTRAINT df_proveedor_activo DEFAULT 1,
  creado_en DATETIME2 NOT NULL CONSTRAINT df_proveedor_creado_en DEFAULT SYSDATETIME(),
  CONSTRAINT pk_proveedor PRIMARY KEY (id_proveedor)
);
GO

CREATE TABLE dbo.producto (
  id_producto INT IDENTITY(1,1) NOT NULL,
  nombre NVARCHAR(120) NOT NULL,
  descripcion NVARCHAR(255) NULL,
  stock INT NOT NULL CONSTRAINT df_producto_stock DEFAULT 0,
  precio_unitario DECIMAL(10,2) NOT NULL CONSTRAINT df_producto_precio DEFAULT 0.00,
  id_proveedor INT NULL,
  activo BIT NOT NULL CONSTRAINT df_producto_activo DEFAULT 1,
  creado_en DATETIME2 NOT NULL CONSTRAINT df_producto_creado_en DEFAULT SYSDATETIME(),
  actualizado_en DATETIME2 NOT NULL CONSTRAINT df_producto_actualizado_en DEFAULT SYSDATETIME(),
  CONSTRAINT pk_producto PRIMARY KEY (id_producto),
  CONSTRAINT fk_producto_proveedor FOREIGN KEY (id_proveedor)
    REFERENCES dbo.proveedor(id_proveedor)
    ON UPDATE CASCADE
    ON DELETE SET NULL,
  CONSTRAINT chk_producto_stock CHECK (stock >= 0),
  CONSTRAINT chk_producto_precio CHECK (precio_unitario >= 0)
);
GO

CREATE TABLE dbo.registro_trabajo (
  id_registro INT IDENTITY(1,1) NOT NULL,
  id_vehiculo INT NOT NULL,
  id_usuario INT NOT NULL,
  fecha DATE NOT NULL,
  tipo NVARCHAR(20) NOT NULL,
  descripcion NVARCHAR(MAX) NOT NULL,
  costo_mano_obra DECIMAL(10,2) NOT NULL CONSTRAINT df_registro_mano_obra DEFAULT 0.00,
  costo_total DECIMAL(10,2) NOT NULL CONSTRAINT df_registro_total DEFAULT 0.00,
  estado NVARCHAR(20) NOT NULL CONSTRAINT df_registro_estado DEFAULT N'PENDIENTE',
  creado_en DATETIME2 NOT NULL CONSTRAINT df_registro_creado_en DEFAULT SYSDATETIME(),
  actualizado_en DATETIME2 NOT NULL CONSTRAINT df_registro_actualizado_en DEFAULT SYSDATETIME(),
  CONSTRAINT pk_registro_trabajo PRIMARY KEY (id_registro),
  CONSTRAINT fk_registro_vehiculo FOREIGN KEY (id_vehiculo)
    REFERENCES dbo.vehiculo(id_vehiculo)
    ON UPDATE CASCADE,
  CONSTRAINT fk_registro_usuario FOREIGN KEY (id_usuario)
    REFERENCES dbo.usuario(id_usuario)
    ON UPDATE CASCADE,
  CONSTRAINT chk_registro_tipo CHECK (tipo IN (N'MANTENIMIENTO', N'REPARACION')),
  CONSTRAINT chk_registro_estado CHECK (estado IN (N'PENDIENTE', N'EN_PROCESO', N'FINALIZADO', N'CANCELADO')),
  CONSTRAINT chk_registro_mano_obra CHECK (costo_mano_obra >= 0),
  CONSTRAINT chk_registro_total CHECK (costo_total >= 0)
);
GO

CREATE TABLE dbo.detalle_repuesto (
  id_detalle INT IDENTITY(1,1) NOT NULL,
  id_registro INT NOT NULL,
  id_producto INT NOT NULL,
  cantidad INT NOT NULL,
  precio_unitario DECIMAL(10,2) NOT NULL,
  subtotal AS (cantidad * precio_unitario) PERSISTED,
  CONSTRAINT pk_detalle_repuesto PRIMARY KEY (id_detalle),
  CONSTRAINT fk_detalle_registro FOREIGN KEY (id_registro)
    REFERENCES dbo.registro_trabajo(id_registro)
    ON UPDATE CASCADE
    ON DELETE CASCADE,
  CONSTRAINT fk_detalle_producto FOREIGN KEY (id_producto)
    REFERENCES dbo.producto(id_producto)
    ON UPDATE CASCADE,
  CONSTRAINT chk_detalle_cantidad CHECK (cantidad > 0),
  CONSTRAINT chk_detalle_precio CHECK (precio_unitario >= 0)
);
GO

CREATE INDEX idx_usuario_rol ON dbo.usuario(id_rol);
CREATE INDEX idx_producto_proveedor ON dbo.producto(id_proveedor);
CREATE INDEX idx_registro_vehiculo_fecha ON dbo.registro_trabajo(id_vehiculo, fecha);
CREATE INDEX idx_registro_usuario ON dbo.registro_trabajo(id_usuario);
CREATE INDEX idx_registro_fecha ON dbo.registro_trabajo(fecha);
CREATE INDEX idx_detalle_registro ON dbo.detalle_repuesto(id_registro);
CREATE INDEX idx_detalle_producto ON dbo.detalle_repuesto(id_producto);
GO

CREATE TRIGGER dbo.trg_usuario_actualizado_en
ON dbo.usuario
AFTER UPDATE
AS
BEGIN
  SET NOCOUNT ON;

  UPDATE u
  SET actualizado_en = SYSDATETIME()
  FROM dbo.usuario u
  INNER JOIN inserted i ON i.id_usuario = u.id_usuario;
END;
GO

CREATE TRIGGER dbo.trg_vehiculo_actualizado_en
ON dbo.vehiculo
AFTER UPDATE
AS
BEGIN
  SET NOCOUNT ON;

  UPDATE v
  SET actualizado_en = SYSDATETIME()
  FROM dbo.vehiculo v
  INNER JOIN inserted i ON i.id_vehiculo = v.id_vehiculo;
END;
GO

CREATE TRIGGER dbo.trg_producto_actualizado_en
ON dbo.producto
AFTER UPDATE
AS
BEGIN
  SET NOCOUNT ON;

  UPDATE p
  SET actualizado_en = SYSDATETIME()
  FROM dbo.producto p
  INNER JOIN inserted i ON i.id_producto = p.id_producto;
END;
GO

CREATE TRIGGER dbo.trg_registro_actualizado_total
ON dbo.registro_trabajo
AFTER INSERT, UPDATE
AS
BEGIN
  SET NOCOUNT ON;

  UPDATE r
  SET
    costo_total = r.costo_mano_obra + COALESCE(t.total_repuestos, 0),
    actualizado_en = SYSDATETIME()
  FROM dbo.registro_trabajo r
  INNER JOIN inserted i ON i.id_registro = r.id_registro
  OUTER APPLY (
    SELECT SUM(d.subtotal) AS total_repuestos
    FROM dbo.detalle_repuesto d
    WHERE d.id_registro = r.id_registro
  ) t;
END;
GO

CREATE TRIGGER dbo.trg_detalle_repuesto_stock_insert
ON dbo.detalle_repuesto
AFTER INSERT
AS
BEGIN
  SET NOCOUNT ON;

  IF EXISTS (
    SELECT 1
    FROM inserted i
    INNER JOIN dbo.producto p ON p.id_producto = i.id_producto
    GROUP BY i.id_producto, p.stock
    HAVING SUM(i.cantidad) > p.stock
  )
  BEGIN
    THROW 50001, 'Stock insuficiente para asignar el repuesto.', 1;
  END;

  UPDATE p
  SET stock = p.stock - x.cantidad
  FROM dbo.producto p
  INNER JOIN (
    SELECT id_producto, SUM(cantidad) AS cantidad
    FROM inserted
    GROUP BY id_producto
  ) x ON x.id_producto = p.id_producto;

  UPDATE r
  SET costo_total = r.costo_mano_obra + COALESCE(t.total_repuestos, 0)
  FROM dbo.registro_trabajo r
  INNER JOIN (
    SELECT DISTINCT id_registro
    FROM inserted
  ) i ON i.id_registro = r.id_registro
  OUTER APPLY (
    SELECT SUM(d.subtotal) AS total_repuestos
    FROM dbo.detalle_repuesto d
    WHERE d.id_registro = r.id_registro
  ) t;
END;
GO

CREATE TRIGGER dbo.trg_detalle_repuesto_stock_update
ON dbo.detalle_repuesto
AFTER UPDATE
AS
BEGIN
  SET NOCOUNT ON;

  IF EXISTS (
    SELECT 1
    FROM (
      SELECT id_producto, SUM(cantidad) AS cantidad_nueva
      FROM inserted
      GROUP BY id_producto
    ) i
    INNER JOIN dbo.producto p ON p.id_producto = i.id_producto
    OUTER APPLY (
      SELECT SUM(d.cantidad) AS cantidad_anterior
      FROM deleted d
      WHERE d.id_producto = i.id_producto
    ) old_qty
    WHERE i.cantidad_nueva > p.stock + COALESCE(old_qty.cantidad_anterior, 0)
  )
  BEGIN
    THROW 50002, 'Stock insuficiente para actualizar el repuesto.', 1;
  END;

  UPDATE p
  SET stock = p.stock + COALESCE(d.cantidad_anterior, 0) - COALESCE(i.cantidad_nueva, 0)
  FROM dbo.producto p
  LEFT JOIN (
    SELECT id_producto, SUM(cantidad) AS cantidad_anterior
    FROM deleted
    GROUP BY id_producto
  ) d ON d.id_producto = p.id_producto
  LEFT JOIN (
    SELECT id_producto, SUM(cantidad) AS cantidad_nueva
    FROM inserted
    GROUP BY id_producto
  ) i ON i.id_producto = p.id_producto
  WHERE d.id_producto IS NOT NULL OR i.id_producto IS NOT NULL;

  UPDATE r
  SET costo_total = r.costo_mano_obra + COALESCE(t.total_repuestos, 0)
  FROM dbo.registro_trabajo r
  INNER JOIN (
    SELECT id_registro FROM inserted
    UNION
    SELECT id_registro FROM deleted
  ) x ON x.id_registro = r.id_registro
  OUTER APPLY (
    SELECT SUM(dr.subtotal) AS total_repuestos
    FROM dbo.detalle_repuesto dr
    WHERE dr.id_registro = r.id_registro
  ) t;
END;
GO

CREATE TRIGGER dbo.trg_detalle_repuesto_stock_delete
ON dbo.detalle_repuesto
AFTER DELETE
AS
BEGIN
  SET NOCOUNT ON;

  UPDATE p
  SET stock = p.stock + x.cantidad
  FROM dbo.producto p
  INNER JOIN (
    SELECT id_producto, SUM(cantidad) AS cantidad
    FROM deleted
    GROUP BY id_producto
  ) x ON x.id_producto = p.id_producto;

  UPDATE r
  SET costo_total = r.costo_mano_obra + COALESCE(t.total_repuestos, 0)
  FROM dbo.registro_trabajo r
  INNER JOIN (
    SELECT DISTINCT id_registro
    FROM deleted
  ) d ON d.id_registro = r.id_registro
  OUTER APPLY (
    SELECT SUM(dr.subtotal) AS total_repuestos
    FROM dbo.detalle_repuesto dr
    WHERE dr.id_registro = r.id_registro
  ) t;
END;
GO

CREATE OR ALTER VIEW dbo.vw_historial_vehiculo AS
SELECT
  v.id_vehiculo,
  v.placa,
  v.marca,
  v.modelo,
  r.id_registro,
  r.fecha,
  r.tipo,
  r.descripcion,
  r.costo_mano_obra,
  r.costo_total,
  r.estado,
  u.nombre AS usuario_registro
FROM dbo.registro_trabajo r
INNER JOIN dbo.vehiculo v ON v.id_vehiculo = r.id_vehiculo
INNER JOIN dbo.usuario u ON u.id_usuario = r.id_usuario;
GO

CREATE OR ALTER VIEW dbo.vw_reporte_trabajos AS
SELECT
  r.id_registro,
  r.fecha,
  r.tipo,
  r.estado,
  v.placa,
  v.marca,
  v.modelo,
  u.nombre AS usuario_registro,
  r.costo_mano_obra,
  COALESCE(SUM(d.subtotal), 0) AS costo_repuestos,
  r.costo_total
FROM dbo.registro_trabajo r
INNER JOIN dbo.vehiculo v ON v.id_vehiculo = r.id_vehiculo
INNER JOIN dbo.usuario u ON u.id_usuario = r.id_usuario
LEFT JOIN dbo.detalle_repuesto d ON d.id_registro = r.id_registro
GROUP BY
  r.id_registro,
  r.fecha,
  r.tipo,
  r.estado,
  v.placa,
  v.marca,
  v.modelo,
  u.nombre,
  r.costo_mano_obra,
  r.costo_total;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.rol WHERE nombre = N'Administrador')
BEGIN
  INSERT INTO dbo.rol (nombre, descripcion) VALUES
    (N'Administrador', N'Gestiona usuarios, roles, vehiculos, registros e inventario'),
    (N'Supervisor', N'Consulta vehiculos, historial y reportes; puede editar registros segun politica'),
    (N'Operador', N'Registra mantenimientos y reparaciones y consulta historial');
END;
GO
