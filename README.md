Sis457_RodaPro
Aplicación de escritorio para la tienda "RodaPro"

Integrantes: Choque Yucra Valentin, Molina Cors Manuel

Descripción del negocio

RODAPRO es una tienda ubicada en la ciudad de Sucre, dedicada a la comercialización de productos y 
repuestos para diferentes aplicaciones mecánicas. Entre los principales productos que ofrece se encuentran 
rodamientos, crucetas, chumaceras, maceteros, retenes y descansos.

El negocio realiza la venta de estos productos a diferentes clientes, por lo que es necesario contar con un 
sistema que permita organizar y controlar la información relacionada con los productos, clientes y ventas. La aplicación 
permitirá gestionar de manera más ordenada los productos disponibles y registrar las ventas realizadas en el negocio.

Entidades y campos del proyecto de la Base de Datos

##Producto

- IdProducto
- IdCategoria
- IdUnidadMedida
- Codigo
- Nombre
- Descripcion
- Marca
- PrecioCompra
- PrecioVenta
- Stock
- StockMinimo
- Estado

##Categoria

- IdCategoria
- Nombre
- Descripcion
- Estado

##UnidadMedida

- IdUnidadMedida
- Nombre
- Abreviatura
- Descripcion
- Estado

##Cliente

- IdCliente
- Nombre
- Apellido
- CiNit
- Telefono
- Direccion
- Estado

##Proveedor

- IdProveedor
- NombreEmpresa
- Nit
- Telefono
- Direccion
- Email
- Estado

##Usuario

- IdUsuario
- NombreCompleto
- NombreUsuario
- PasswordHash
- Rol
- Estado

##Empleado

- IdEmpleado
- Usuario
- Clave
- UsuarioRegistro
- FechaRegistro
- Estado

##Compra

- IdCompra
- IdProveedor
- IdUsuario
- Fecha
- NumeroDocumento
- TipoDocumento
- Subtotal
- Descuento
- Total
- Estado

##CompraDetalle

- IdDetalleCompra
- IdCompra
- IdProducto
- Cantidad
- CostoUnitario
- Descuento
- Subtotal

##Venta

- IdVenta
- IdCliente
- IdUsuario
- Fecha
- NumeroDocumento
- TipoDocumento
- Subtotal
- Descuento
- Total
- Estado

##VentaDetalle

- IdDetalleVenta
- IdVenta
- IdProducto
- Cantidad
- PrecioUnitario
- Descuento
- Subtotal

##Pago

- IdPago
- IdVenta
- IdCompra
- IdUsuario
- Fecha
- Monto
- MetodoPago
- Referencia

##MovimientoInventario

- IdMovimiento
- IdProducto
- IdUsuario
- Fecha
- TipoMovimiento
- Cantidad
- ReferenciaTipo
- ReferenciaId
- Observacion
