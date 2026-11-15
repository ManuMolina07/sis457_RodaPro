using CadRodapro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace ClnRodaPro
{
    public class VentaCln
    {
        // REGISTRAR VENTA
        public static long registrar(
            Venta venta,
            List<VentaDetalle> detalles,
            string usuario)
        {
            if (venta == null)
                throw new Exception("La venta no puede estar vacía.");

            if (detalles == null || detalles.Count == 0)
                throw new Exception("Debe agregar al menos un producto.");

            using (var context = new LabRodaproEntities())
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    decimal subtotalGeneral = 0;

                    // VALIDAR STOCK AGRUPANDO PRODUCTOS REPETIDOS
                    var cantidadesPorProducto = detalles
                        .GroupBy(x => x.idProducto)
                        .Select(g => new
                        {
                            idProducto = g.Key,
                            cantidad = g.Sum(x => x.cantidad)
                        })
                        .ToList();

                    foreach (var item in cantidadesPorProducto)
                    {
                        var producto = context.Producto.FirstOrDefault(
                            x => x.id == item.idProducto &&
                                 x.estado == 1);

                        if (producto == null)
                            throw new Exception(
                                "Uno de los productos no existe o está inactivo.");

                        if (producto.stock < item.cantidad)
                            throw new Exception(
                                "Stock insuficiente para: " + producto.nombre);
                    }

                    // VALIDAR Y CALCULAR DETALLES
                    foreach (var detalle in detalles)
                    {
                        if (detalle.cantidad <= 0)
                            throw new Exception(
                                "La cantidad debe ser mayor a cero.");

                        if (detalle.precioUnitario < 0)
                            throw new Exception(
                                "El precio unitario no puede ser negativo.");

                        decimal importe =
                            detalle.cantidad * detalle.precioUnitario;

                        if (detalle.descuento < 0 ||
                            detalle.descuento > importe)
                        {
                            throw new Exception(
                                "El descuento del producto no es válido.");
                        }

                        detalle.subtotal = importe - detalle.descuento;

                        subtotalGeneral += detalle.subtotal;
                    }

                    // VALIDAR DESCUENTO GENERAL
                    decimal descuentoGeneral = venta.descuento;

                    if (descuentoGeneral < 0 ||
                        descuentoGeneral > subtotalGeneral)
                    {
                        throw new Exception(
                            "El descuento general no es válido.");
                    }

                    // CONFIGURAR VENTA
                    venta.fecha = DateTime.Now;
                    venta.subtotal = subtotalGeneral;
                    venta.descuento = descuentoGeneral;
                    venta.total = subtotalGeneral - descuentoGeneral;
                    venta.usuarioRegistro = usuario;
                    venta.fechaRegistro = DateTime.Now;
                    venta.estado = 1;

                    context.Venta.Add(venta);
                    context.SaveChanges();

                    // REGISTRAR DETALLES Y DESCONTAR STOCK
                    foreach (var detalle in detalles)
                    {
                        var producto = context.Producto.First(
                            x => x.id == detalle.idProducto);

                        detalle.idVenta = venta.id;
                        detalle.usuarioRegistro = usuario;
                        detalle.fechaRegistro = DateTime.Now;
                        detalle.estado = 1;

                        producto.stock -= detalle.cantidad;

                        context.VentaDetalle.Add(detalle);
                    }

                    context.SaveChanges();

                    transaccion.Commit();

                    return venta.id;
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        // LISTAR VENTAS
        public static List<VentaListarDto> listar(string parametro)
        {
            using (var context = new LabRodaproEntities())
            {
                return context.Database.SqlQuery<VentaListarDto>(
                    "EXEC paVentaListar @parametro",
                    new SqlParameter("@parametro", parametro ?? "")
                ).ToList();
            }
        }

        // OBTENER UNA VENTA POR SU ID
        public static Venta obtenerPorId(long id)
        {
            using (var context = new LabRodaproEntities())
            {
                return context.Venta.FirstOrDefault(
                    x => x.id == id && x.estado == 1
                );
            }
        }

        // LISTAR DETALLES ACTIVOS DE UNA VENTA
        public static List<VentaDetalle> listarDetalles(long idVenta)
        {
            using (var context = new LabRodaproEntities())
            {
                return context.VentaDetalle
                    .Where(x => x.idVenta == idVenta && x.estado == 1)
                    .ToList();
            }
        }

        // MODIFICAR VENTA
        public static void modificar(
            Venta venta,
            List<VentaDetalle> nuevosDetalles,
            string usuario)
        {
            if (venta == null)
                throw new Exception("La venta no puede estar vacía.");

            if (nuevosDetalles == null || nuevosDetalles.Count == 0)
                throw new Exception("Debe agregar al menos un producto.");

            using (var context = new LabRodaproEntities())
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var ventaActual = context.Venta.FirstOrDefault(
                        x => x.id == venta.id && x.estado == 1
                    );

                    if (ventaActual == null)
                        throw new Exception(
                            "La venta no existe o está anulada.");

                    // NO MODIFICAR VENTAS CON PAGOS REGISTRADOS
                    bool tienePagos = context.Pago.Any(
                        x => x.idVenta == venta.id && x.estado == 1
                    );

                    if (tienePagos)
                        throw new Exception(
                            "No se puede modificar esta venta porque tiene pagos registrados.");

                    // RECUPERAR DETALLES ANTERIORES
                    var detallesAnteriores = context.VentaDetalle
                        .Where(x => x.idVenta == venta.id && x.estado == 1)
                        .ToList();

                    // DEVOLVER AL INVENTARIO EL STOCK ANTERIOR
                    foreach (var detalle in detallesAnteriores)
                    {
                        var producto = context.Producto.FirstOrDefault(
                            x => x.id == detalle.idProducto
                        );

                        if (producto == null)
                            throw new Exception(
                                "No se encontró uno de los productos anteriores.");

                        producto.stock += detalle.cantidad;

                        detalle.estado = -1;
                        detalle.usuarioRegistro = usuario;
                    }

                    // VALIDAR NUEVOS DETALLES
                    decimal subtotalGeneral = 0;

                    foreach (var detalle in nuevosDetalles)
                    {
                        if (detalle.cantidad <= 0)
                            throw new Exception(
                                "La cantidad debe ser mayor a cero.");

                        if (detalle.precioUnitario < 0)
                            throw new Exception(
                                "El precio unitario no puede ser negativo.");

                        if (detalle.descuento < 0)
                            throw new Exception(
                                "El descuento no puede ser negativo.");

                        var producto = context.Producto.FirstOrDefault(
                            x => x.id == detalle.idProducto &&
                                 x.estado == 1
                        );

                        if (producto == null)
                            throw new Exception(
                                "Uno de los productos no existe o está inactivo.");

                        decimal importe =
                            detalle.cantidad * detalle.precioUnitario;

                        if (detalle.descuento > importe)
                            throw new Exception(
                                "El descuento supera el importe del producto.");

                        detalle.subtotal = importe - detalle.descuento;

                        subtotalGeneral += detalle.subtotal;
                    }

                    // COMPROBAR STOCK AGRUPANDO PRODUCTOS REPETIDOS
                    var cantidadesPorProducto = nuevosDetalles
                        .GroupBy(x => x.idProducto)
                        .Select(g => new
                        {
                            idProducto = g.Key,
                            cantidad = g.Sum(x => x.cantidad)
                        })
                        .ToList();

                    foreach (var item in cantidadesPorProducto)
                    {
                        var producto = context.Producto.First(
                            x => x.id == item.idProducto
                        );

                        if (producto.stock < item.cantidad)
                            throw new Exception(
                                "Stock insuficiente para: " + producto.nombre);
                    }

                    // VALIDAR DESCUENTO GENERAL
                    decimal descuentoGeneral = venta.descuento;

                    if (descuentoGeneral < 0 ||
                        descuentoGeneral > subtotalGeneral)
                    {
                        throw new Exception(
                            "El descuento general no es válido.");
                    }

                    // ACTUALIZAR ENCABEZADO
                    ventaActual.idCliente = venta.idCliente;
                    ventaActual.idUsuario = venta.idUsuario;
                    ventaActual.fecha = venta.fecha;
                    ventaActual.numeroDocumento = venta.numeroDocumento;
                    ventaActual.tipoDocumento = venta.tipoDocumento;
                    ventaActual.subtotal = subtotalGeneral;
                    ventaActual.descuento = descuentoGeneral;
                    ventaActual.total = subtotalGeneral - descuentoGeneral;
                    ventaActual.usuarioRegistro = usuario;

                    // REGISTRAR NUEVOS DETALLES Y DESCONTAR STOCK
                    foreach (var detalle in nuevosDetalles)
                    {
                        var producto = context.Producto.First(
                            x => x.id == detalle.idProducto
                        );

                        detalle.id = 0;
                        detalle.idVenta = venta.id;
                        detalle.usuarioRegistro = usuario;
                        detalle.fechaRegistro = DateTime.Now;
                        detalle.estado = 1;

                        producto.stock -= detalle.cantidad;

                        context.VentaDetalle.Add(detalle);
                    }

                    context.SaveChanges();
                    transaccion.Commit();
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        // ANULAR VENTA
        public static void anular(long id, string usuario)
        {
            using (var context = new LabRodaproEntities())
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var venta = context.Venta.FirstOrDefault(
                        x => x.id == id && x.estado == 1
                    );

                    if (venta == null)
                        throw new Exception(
                            "La venta no existe o ya está anulada.");

                    // VERIFICAR PAGOS REGISTRADOS
                    bool tienePagos = context.Pago.Any(
                        x => x.idVenta == id && x.estado == 1
                    );

                    if (tienePagos)
                        throw new Exception(
                            "No se puede anular esta venta porque tiene pagos registrados. Primero debe gestionar los pagos.");

                    // RECUPERAR DETALLES ACTIVOS
                    var detalles = context.VentaDetalle
                        .Where(x => x.idVenta == id && x.estado == 1)
                        .ToList();

                    // DEVOLVER PRODUCTOS AL INVENTARIO
                    foreach (var detalle in detalles)
                    {
                        var producto = context.Producto.FirstOrDefault(
                            x => x.id == detalle.idProducto
                        );

                        if (producto == null)
                            throw new Exception(
                                "No se encontró uno de los productos.");

                        producto.stock += detalle.cantidad;

                        detalle.estado = -1;
                        detalle.usuarioRegistro = usuario;
                    }

                    // ANULACIÓN LÓGICA
                    venta.estado = -1;
                    venta.usuarioRegistro = usuario;

                    context.SaveChanges();
                    transaccion.Commit();
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }
    }
}
