using CadRodapro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClnRodaPro
{
    public class PagoCln
    {
        public static List<Pago> listarPorVenta(long idVenta)
        {
            using (var context = new LabRodaproEntities())
            {
                return context.Pago
                    .Where(x =>
                        x.idVenta == idVenta &&
                        x.estado == (short)Estado.Activo)
                    .OrderBy(x => x.fecha)
                    .ToList();
            }
        }

        public static void registrarVenta(
            long idVenta,
            int idUsuario,
            decimal monto,
            string metodoPago,
            string referencia,
            string usuarioRegistro)
        {
            using (var context = new LabRodaproEntities())
            {
                if (monto <= 0)
                    throw new Exception("El monto debe ser mayor a cero.");

                if (string.IsNullOrWhiteSpace(metodoPago))
                    throw new Exception("Debe seleccionar un método de pago.");

                var venta = context.Venta.FirstOrDefault(x =>
                    x.id == idVenta &&
                    x.estado == (short)Estado.Activo);

                if (venta == null)
                    throw new Exception("La venta no existe o está inactiva.");

                Pago pago = new Pago();

                pago.idVenta = idVenta;
                pago.idCompra = null;
                pago.idUsuario = idUsuario;
                pago.fecha = DateTime.Now;
                pago.monto = monto;
                pago.metodoPago = metodoPago;
                pago.referencia = referencia;
                pago.usuarioRegistro = usuarioRegistro;
                pago.fechaRegistro = DateTime.Now;
                pago.estado = (short)Estado.Activo;

                context.Pago.Add(pago);
                context.SaveChanges();
            }
        }

        public static void eliminar(int id)
        {
            using (var context = new LabRodaproEntities())
            {
                var pago = context.Pago.FirstOrDefault(x => x.id == id);

                if (pago == null)
                    throw new Exception("El pago no existe.");

                pago.estado = (short)Estado.Eliminado;

                context.SaveChanges();
            }
        }
    }
}
