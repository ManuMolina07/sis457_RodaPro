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

Entidades y campos tentativos

Producto

- IdProducto
- Nombre
- Descripción
- Marca
- Precio
- Stock
- Estado

Categoría

- IdCategoria
- Nombre
- Descripción
- Estado

Cliente

- IdCliente
- Nombre
- Apellido
- CI
- Teléfono
- Dirección
- Estado

Venta

- IdVenta
- Fecha
- IdCliente
- Total
- Estado

DetalleVenta

- IdDetalleVenta
- IdVenta
- IdProducto
- Cantidad
- PrecioUnitario
- Subtotal

Usuario

- IdUsuario
- NombreUsuario
- Contraseña
- Rol
- Estado