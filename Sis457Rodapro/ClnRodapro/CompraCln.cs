using CadRodapro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;

namespace ClnRodaPro
{
    public class CompraCln
    {
        // REGISTRAR COMPRA
        public static long registrar(
            Compra compra,
            List<CompraDetalle> detalles,
            string usuario)
        {
            if (compra == null)
                throw new Exception("La compra no puede estar vacía.");

            if (detalles == null || detalles.Count == 0)
                throw new Exception("Debe agregar al menos un producto.");

            using (var context = new LabRodaproEntities())
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    decimal subtotalGeneral = 0;
                    decimal descuentoGeneral = 0;

                    foreach (var detalle in detalles)
                    {
                        if (detalle.cantidad <= 0)
                            throw new Exception("La cantidad debe ser mayor a cero.");

                        if (detalle.costoUnitario < 0)
                            throw new Exception("El costo unitario no puede ser negativo.");

                        var producto = context.Producto.FirstOrDefault(
                            x => x.id == detalle.idProducto &&
                                 x.estado == 1
                        );

                        if (producto == null)
                            throw new Exception(
                                "Uno de los productos no existe o está inactivo."
                            );

                        detalle.subtotal =
                            detalle.cantidad * detalle.costoUnitario;

                        if (detalle.descuento < 0 ||
                            detalle.descuento > detalle.subtotal)
                        {
                            throw new Exception(
                                "El descuento del detalle no es válido."
                            );
                        }

                        subtotalGeneral += detalle.subtotal;
                        descuentoGeneral += detalle.descuento;
                    }

                    compra.fecha = DateTime.Now;
                    compra.subtotal = subtotalGeneral;
                    compra.descuento = descuentoGeneral;
                    compra.total = subtotalGeneral - descuentoGeneral;
                    compra.usuarioRegistro = usuario;
                    compra.fechaRegistro = DateTime.Now;
                    compra.estado = 1;

                    context.Compra.Add(compra);
                    context.SaveChanges();

                    foreach (var detalle in detalles)
                    {
                        var producto = context.Producto.First(
                            x => x.id == detalle.idProducto
                        );

                        detalle.idCompra = compra.id;
                        detalle.usuarioRegistro = usuario;
                        detalle.fechaRegistro = DateTime.Now;
                        detalle.estado = 1;

                        producto.stock += detalle.cantidad;

                        context.CompraDetalle.Add(detalle);
                    }

                    context.SaveChanges();

                    transaccion.Commit();

                    return compra.id;
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        // LISTAR COMPRAS
        public static List<CompraListarDto> listar(string parametro)
        {
            using (var context = new LabRodaproEntities())
            {
                return context.Database.SqlQuery<CompraListarDto>(
                    "EXEC paCompraListar @parametro",
                    new SqlParameter("@parametro", parametro ?? "")
                ).ToList();
            }
        }

        // MODIFICAR COMPRA
        public static void modificar(
            Compra compra,
            List<CompraDetalle> nuevosDetalles,
            string usuario)
        {
            if (compra == null)
                throw new Exception("La compra no puede estar vacía.");

            if (nuevosDetalles == null || nuevosDetalles.Count == 0)
                throw new Exception("Debe agregar al menos un producto.");

            using (var context = new LabRodaproEntities())
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var compraActual = context.Compra.FirstOrDefault(
                        x => x.id == compra.id && x.estado == 1
                    );

                    if (compraActual == null)
                        throw new Exception("La compra no existe o está anulada.");

                    decimal subtotalGeneral = 0;
                    decimal descuentoGeneral = 0;

                    // Validar nuevos detalles
                    foreach (var detalle in nuevosDetalles)
                    {
                        if (detalle.cantidad <= 0)
                            throw new Exception("La cantidad debe ser mayor a cero.");

                        if (detalle.costoUnitario < 0)
                            throw new Exception("El costo no puede ser negativo.");

                        if (detalle.descuento < 0)
                            throw new Exception("El descuento no puede ser negativo.");

                        var producto = context.Producto.FirstOrDefault(
                            x => x.id == detalle.idProducto &&
                                 x.estado == 1
                        );

                        if (producto == null)
                            throw new Exception(
                                "Uno de los productos no existe o está inactivo."
                            );

                        detalle.subtotal =
                            detalle.cantidad * detalle.costoUnitario;

                        if (detalle.descuento > detalle.subtotal)
                            throw new Exception("El descuento supera el subtotal.");

                        subtotalGeneral += detalle.subtotal;
                        descuentoGeneral += detalle.descuento;
                    }

                    // Recuperar detalles anteriores
                    var detallesAnteriores = context.CompraDetalle
                        .Where(x => x.idCompra == compra.id && x.estado == 1)
                        .ToList();

                    // Revertir stock anterior
                    foreach (var detalle in detallesAnteriores)
                    {
                        var producto = context.Producto.FirstOrDefault(
                            x => x.id == detalle.idProducto
                        );

                        if (producto == null)
                            throw new Exception(
                                "No se encontró un producto anterior."
                            );

                        if (producto.stock < detalle.cantidad)
                            throw new Exception(
                                "No hay stock suficiente para revertir la compra anterior: "
                                + producto.nombre
                            );

                        producto.stock -= detalle.cantidad;
                        detalle.estado = -1;
                    }

                    // Actualizar encabezado
                    compraActual.idProveedor = compra.idProveedor;
                    compraActual.idUsuario = compra.idUsuario;
                    compraActual.fecha = compra.fecha;
                    compraActual.numeroDocumento = compra.numeroDocumento;
                    compraActual.tipoDocumento = compra.tipoDocumento;
                    compraActual.subtotal = subtotalGeneral;
                    compraActual.descuento = descuentoGeneral;
                    compraActual.total = subtotalGeneral - descuentoGeneral;

                    // Guardar nuevos detalles
                    foreach (var detalle in nuevosDetalles)
                    {
                        var producto = context.Producto.First(
                            x => x.id == detalle.idProducto
                        );

                        detalle.id = 0;
                        detalle.idCompra = compra.id;
                        detalle.usuarioRegistro = usuario;
                        detalle.fechaRegistro = DateTime.Now;
                        detalle.estado = 1;

                        producto.stock += detalle.cantidad;

                        context.CompraDetalle.Add(detalle);
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

        // ANULAR COMPRA
        public static void anular(long id, string usuario)
        {
            using (var context = new LabRodaproEntities())
            using (var transaccion = context.Database.BeginTransaction())
            {
                try
                {
                    var compra = context.Compra.FirstOrDefault(
                        x => x.id == id && x.estado == 1
                    );

                    if (compra == null)
                        throw new Exception(
                            "La compra no existe o ya está anulada."
                        );

                    var detalles = context.CompraDetalle
                        .Where(x => x.idCompra == id && x.estado == 1)
                        .ToList();

                    // Revertir stock
                    foreach (var detalle in detalles)
                    {
                        var producto = context.Producto.FirstOrDefault(
                            x => x.id == detalle.idProducto
                        );

                        if (producto == null)
                            throw new Exception(
                                "No se encontró uno de los productos."
                            );

                        if (producto.stock < detalle.cantidad)
                            throw new Exception(
                                "No hay stock suficiente para anular la compra. Producto: "
                                + producto.nombre
                            );

                        producto.stock -= detalle.cantidad;

                        detalle.estado = -1;
                    }

                    // Anulación lógica
                    compra.estado = -1;

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

