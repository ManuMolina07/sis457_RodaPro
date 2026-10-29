CREATE DATABASE LabRodapro
GO
USE master

GO
CREATE LOGIN usrrodapro WITH PASSWORD =N'123456',
	DEFAULT_DATABASE = LabRodapro, 
	CHECK_EXPIRATION = OFF, 
	CHECK_POLICY = ON
GO
USE LabRodapro
GO
CREATE USER usrrodapro FOR LOGIN usrrodapro
GO
ALTER ROLE [db_owner] ADD MEMBER usrrodapro
GO

DROP TABLE IF EXISTS Pago;
DROP TABLE IF EXISTS MovimientoInventario;
DROP TABLE IF EXISTS VentaDetalle;
DROP TABLE IF EXISTS CompraDetalle;
DROP TABLE IF EXISTS Venta;
DROP TABLE IF EXISTS Compra;
DROP TABLE IF EXISTS Usuario;
DROP TABLE IF EXISTS Producto;
DROP TABLE IF EXISTS Empleado;
DROP TABLE IF EXISTS Cliente;
DROP TABLE IF EXISTS Proveedor;
DROP TABLE IF EXISTS Categoria;
DROP TABLE IF EXISTS UnidadMedida;
GO

CREATE TABLE UnidadMedida (
	id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	descripcion VARCHAR(25) NOT NULL
);

CREATE TABLE Categoria (
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100) NOT NULL,
    descripcion VARCHAR(250) NULL
);

CREATE TABLE Empleado (
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    cedulaIdentidad VARCHAR(12) NOT NULL,
    nombres VARCHAR(50) NOT NULL,
    primerApellido VARCHAR(30) NULL,
    segundoApellido VARCHAR(30) NULL,
    fechaNacimiento DATE NOT NULL,
    direccion VARCHAR(150) NOT NULL,
    celular BIGINT NOT NULL,
    cargo VARCHAR(50) NOT NULL
);

CREATE TABLE Usuario (
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    idEmpleado INT NOT NULL,
    usuario VARCHAR(50) NOT NULL,
    passwordHash VARCHAR(255) NOT NULL,
    rol VARCHAR(30) NOT NULL,

    CONSTRAINT fk_Usuario_Empleado FOREIGN KEY (idEmpleado) REFERENCES Empleado(id),
);

CREATE TABLE Proveedor (
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    nombreEmpresa VARCHAR(150) NOT NULL,
    nit VARCHAR(20) NOT NULL,
    telefono VARCHAR(20) NULL,
    direccion VARCHAR(150) NULL,
    email VARCHAR(100) NULL
);

CREATE TABLE Cliente (
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    nombre VARCHAR(100) NOT NULL,
    apellido VARCHAR(100) NOT NULL,
    ciNit VARCHAR(20) NULL,
    telefono VARCHAR(20) NULL,
    direccion VARCHAR(150) NULL
);

CREATE TABLE Producto (
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    idCategoria INT NOT NULL,
    idUnidadMedida INT NOT NULL,
    codigo VARCHAR(30) NOT NULL,
    nombre VARCHAR(150) NOT NULL,
    descripcion VARCHAR(250) NULL,
    marca VARCHAR(100) NULL,
    precioCompra DECIMAL(12,2) NOT NULL CHECK (precioCompra > 0),
    precioVenta DECIMAL(12,2) NOT NULL CHECK (precioVenta > 0),
    stock DECIMAL(12,2) NOT NULL DEFAULT 0,
    stockMinimo DECIMAL(12,2) NOT NULL DEFAULT 0,

    CONSTRAINT fk_Producto_Categoria FOREIGN KEY (idCategoria) REFERENCES Categoria(id),
    CONSTRAINT fk_Producto_UnidadMedida FOREIGN KEY (idUnidadMedida) REFERENCES UnidadMedida(id),
);

CREATE TABLE Compra (
    id BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    idProveedor INT NOT NULL,
    idUsuario INT NOT NULL,
    fecha DATETIME NOT NULL DEFAULT GETDATE(),
    numeroDocumento VARCHAR(50) NULL,
    tipoDocumento VARCHAR(30) NOT NULL,
    subtotal DECIMAL(12,2) NOT NULL DEFAULT 0 CHECK (subtotal >= 0),
    descuento DECIMAL(12,2) NOT NULL DEFAULT 0,
    total DECIMAL(12,2) NOT NULL DEFAULT 0 CHECK (total >= 0),

    CONSTRAINT fk_Compra_Proveedor FOREIGN KEY (idProveedor) REFERENCES Proveedor(id),
    CONSTRAINT fk_Compra_Usuario FOREIGN KEY (idUsuario) REFERENCES Usuario(id),
    CONSTRAINT ck_Compra_Descuento CHECK (descuento >= 0 AND descuento <= subtotal)
);

CREATE TABLE CompraDetalle (
    id BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    idCompra BIGINT NOT NULL,
    idProducto INT NOT NULL,
    cantidad DECIMAL(12,2) NOT NULL CHECK (cantidad > 0),
    costoUnitario DECIMAL(12,2) NOT NULL CHECK (costoUnitario >= 0),
    descuento DECIMAL(12,2) NOT NULL DEFAULT 0,
    subtotal DECIMAL(12,2) NOT NULL,

    CONSTRAINT fk_CompraDetalle_Compra FOREIGN KEY (idCompra) REFERENCES Compra(id),
    CONSTRAINT fk_CompraDetalle_Producto FOREIGN KEY (idProducto) REFERENCES Producto(id),
    CONSTRAINT ck_CompraDetalle_Descuento CHECK (descuento >= 0 AND descuento <= subtotal)
);

CREATE TABLE Venta (
    id BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    idCliente INT NULL,
    idUsuario INT NOT NULL,
    fecha DATETIME NOT NULL DEFAULT GETDATE(),
    numeroDocumento VARCHAR(50) NULL,
    tipoDocumento VARCHAR(30) NOT NULL,
    subtotal DECIMAL(12,2) NOT NULL DEFAULT 0 CHECK (subtotal >= 0),
    descuento DECIMAL(12,2) NOT NULL DEFAULT 0,
    total DECIMAL(12,2) NOT NULL DEFAULT 0 CHECK (total >= 0),

    CONSTRAINT ck_Venta_Descuento CHECK (descuento >= 0 AND descuento <= subtotal),
    CONSTRAINT fk_Venta_Cliente FOREIGN KEY (idCliente) REFERENCES Cliente(id),
    CONSTRAINT fk_Venta_Usuario FOREIGN KEY (idUsuario) REFERENCES Usuario(id),
);

CREATE TABLE VentaDetalle (
    id BIGINT NOT NULL PRIMARY KEY IDENTITY(1,1),
    idVenta BIGINT NOT NULL,
    idProducto INT NOT NULL,
    cantidad DECIMAL(12,2) NOT NULL CHECK (cantidad > 0),
    precioUnitario DECIMAL(12,2) NOT NULL CHECK (precioUnitario >= 0),
    descuento DECIMAL(12,2) NOT NULL DEFAULT 0,
    subtotal DECIMAL(12,2) NOT NULL,

    CONSTRAINT fk_VentaDetalle_Venta FOREIGN KEY (idVenta) REFERENCES Venta(id),
    CONSTRAINT fk_VentaDetalle_Producto FOREIGN KEY (idProducto) REFERENCES Producto(id),
    CONSTRAINT ck_VentaDetalle_Descuento CHECK (descuento >= 0 AND descuento <= subtotal)
);

CREATE TABLE Pago (
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    idVenta BIGINT NULL,
    idCompra BIGINT NULL,
    idUsuario INT NOT NULL,
    fecha DATETIME NOT NULL DEFAULT GETDATE(),
    monto DECIMAL(12,2) NOT NULL CHECK (monto > 0),
    metodoPago VARCHAR(40) NOT NULL,
    referencia VARCHAR(100) NULL,

    CONSTRAINT fk_Pago_Venta FOREIGN KEY (idVenta) REFERENCES Venta(id),
    CONSTRAINT fk_Pago_Compra FOREIGN KEY (idCompra) REFERENCES Compra(id),
    CONSTRAINT fk_Pago_Usuario FOREIGN KEY (idUsuario) REFERENCES Usuario(id),

    CONSTRAINT ck_Pago_Operacion
    CHECK (
            (idVenta IS NOT NULL AND idCompra IS NULL)
            OR
            (idVenta IS NULL AND idCompra IS NOT NULL)
        ),
);

CREATE TABLE MovimientoInventario (
    id INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    idProducto INT NOT NULL,
    idUsuario INT NOT NULL,
    fecha DATETIME NOT NULL DEFAULT GETDATE(),
    tipoMovimiento VARCHAR(20) NOT NULL,
    cantidad DECIMAL(12,2) NOT NULL CHECK (cantidad > 0),
    referenciaTipo VARCHAR(30) NULL,
    referenciaId BIGINT NULL,
    observacion VARCHAR(250) NULL,

    CONSTRAINT fk_MovimientoInventario_Producto FOREIGN KEY (idProducto) REFERENCES Producto(id),
    CONSTRAINT fk_MovimientoInventario_Usuario FOREIGN KEY (idUsuario) REFERENCES Usuario(id),
    CONSTRAINT ck_MovimientoInventario_Tipo
        CHECK (tipoMovimiento IN
        ('ENTRADA', 'SALIDA', 'AJUSTE')),
);

--AUDITORIA Y ESTADO--

ALTER TABLE UnidadMedida ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE UnidadMedida ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE UnidadMedida ADD estado SMALLINT NOT NULL DEFAULT 1; -- -1: Eliminado, 0: Inactivo, 1: Activo

ALTER TABLE Categoria ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Categoria ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Categoria ADD estado SMALLINT NOT NULL DEFAULT 1; -- -1: Eliminado, 0: Inactivo, 1: Activo

ALTER TABLE Empleado ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Empleado ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Empleado ADD estado SMALLINT NOT NULL DEFAULT 1; -- -1: Eliminado, 0: Inactivo, 1: Activo

ALTER TABLE Usuario ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Usuario ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Usuario ADD estado SMALLINT NOT NULL DEFAULT 1; -- -1: Eliminado, 0: Inactivo, 1: Activo

ALTER TABLE Proveedor ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Proveedor ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Proveedor ADD estado SMALLINT NOT NULL DEFAULT 1; -- -1: Eliminado, 0: Inactivo, 1: Activo

ALTER TABLE Cliente ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Cliente ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Cliente ADD estado SMALLINT NOT NULL DEFAULT 1; -- -1: Eliminado, 0: Inactivo, 1: Activo

ALTER TABLE Producto ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Producto ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Producto ADD estado SMALLINT NOT NULL DEFAULT 1; -- -1: Eliminado, 0: Inactivo, 1: Activo

ALTER TABLE Compra ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Compra ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Compra ADD estado SMALLINT NOT NULL DEFAULT 1; -- -1: Eliminado, 0: Inactivo, 1: Activo

ALTER TABLE Venta ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Venta ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Venta ADD estado SMALLINT NOT NULL DEFAULT 1; -- -1: Eliminado, 0: Inactivo, 1: Activo

ALTER TABLE CompraDetalle ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE CompraDetalle ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE CompraDetalle ADD estado SMALLINT NOT NULL DEFAULT 1; -- -1: Eliminado, 0: Inactivo, 1: Activo

ALTER TABLE VentaDetalle ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE VentaDetalle ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE VentaDetalle ADD estado SMALLINT NOT NULL DEFAULT 1; -- -1: Eliminado, 0: Inactivo, 1: Activo

ALTER TABLE Pago ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE Pago ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE Pago ADD estado SMALLINT NOT NULL DEFAULT 1; -- -1: Eliminado, 0: Inactivo, 1: Activo

ALTER TABLE MovimientoInventario ADD usuarioRegistro VARCHAR(50) NOT NULL DEFAULT SUSER_NAME();
ALTER TABLE MovimientoInventario ADD fechaRegistro DATETIME NOT NULL DEFAULT GETDATE();
ALTER TABLE MovimientoInventario ADD estado SMALLINT NOT NULL DEFAULT 1; -- -1: Eliminado, 0: Inactivo, 1: Activo

GO
GO
DROP PROC IF EXISTS paProductoListar;
GO

CREATE PROC paProductoListar @parametro VARCHAR(50)
AS
    SELECT
        p.id,
        p.idCategoria,
        p.idUnidadMedida,
        p.codigo,
        p.nombre,
        p.descripcion,
        p.marca,
        c.nombre AS categoria,
        um.descripcion AS unidadMedida,
        p.precioCompra,
        p.precioVenta,
        p.stock,
        p.stockMinimo,
        p.usuarioRegistro,
        p.fechaRegistro,
        p.estado
    FROM Producto p
    INNER JOIN UnidadMedida um ON um.id = p.idUnidadMedida
    INNER JOIN Categoria c ON c.id = p.idCategoria
    WHERE p.estado = 1 AND ( p.codigo + p.nombre + ISNULL(p.descripcion, '') +
            ISNULL(p.marca, '') + c.nombre + um.descripcion
        ) LIKE '%' + REPLACE(@parametro, ' ', '%') + '%'
    ORDER BY p.nombre;
GO

EXEC paProductoListar 'chumacera';
GO
--DML--

INSERT INTO UnidadMedida (descripcion)
VALUES ('Unidad'),    --1
       ('Caja'),      --2
       ('Docena'),    --3
       ('Kilogramo'), --4
       ('Juego'),     --5
       ('Metro');     --6

INSERT INTO Categoria (nombre, descripcion)
VALUES
('Rodamientos', 'Rodamientos industriales y mecánicos'),--1
('Crucetas', 'Crucetas para sistemas de transmisión'),  --2
('Chumaceras', 'Soportes para rodamientos'),            --3
('Maceteros', 'Maceteros y soportes comercializados'),  --4
('Retenes', 'Retenes y sellos de aceite'),              --5
('Descansos', 'Descansos para ejes y maquinaria');      --6

INSERT INTO Producto (idCategoria, idUnidadMedida, codigo, nombre, descripcion, marca,precioCompra, precioVenta, stock, stockMinimo)
VALUES
(1, 1, 'ROD001', 'Rodamiento 6205', 'Rodamiento rígido de bolas', 'SKF', 25, 35, 20, 5),
(2, 1, 'CRU001', 'Cruceta universal', 'Cruceta para transmisión mecánica', 'Genérica', 40, 55, 10, 3),
(3, 1, 'CHU001', 'Chumacera UCP205', 'Chumacera con rodamiento', 'SKF', 60, 85, 8, 2),
(4, 1, 'MAC001', 'Macetero metálico', 'Macetero de metal', 'Genérica', 30, 45, 12, 3),
(5, 1, 'RET001', 'Retén 35x52x7', 'Retén de aceite', 'Genérica', 8, 15, 30, 5),
(6, 1, 'DES001', 'Descanso para eje', 'Soporte para eje rotativo', 'Genérica', 50, 70, 6, 2);

SELECT * FROM UnidadMedida;
SELECT * FROM Categoria;
SELECT * FROM Producto;