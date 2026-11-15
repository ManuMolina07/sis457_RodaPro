using CadRodapro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClnRodaPro
{
    public class ProveedorCln
    {
        public static List<Proveedor> listar(string parametro = "")
        {
            using (var context = new LabRodaproEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                context.Configuration.ProxyCreationEnabled = false;

                parametro = parametro ?? "";

                return context.Proveedor
                    .Where(p => p.estado == 1 &&
                        (p.nombreEmpresa.Contains(parametro) ||
                         p.nit.Contains(parametro) ||
                         (p.telefono != null && p.telefono.Contains(parametro)) ||
                         (p.direccion != null && p.direccion.Contains(parametro)) ||
                         (p.email != null && p.email.Contains(parametro))))
                    .OrderBy(p => p.nombreEmpresa)
                    .ToList();
            }
        }


        public static int crear(Proveedor proveedor)
        {
            using (var context = new LabRodaproEntities())
            {
                bool existe = context.Proveedor.Any(p =>
                    p.nit == proveedor.nit && p.estado != -1);

                if (existe)
                    return -1;

                proveedor.usuarioRegistro = "admin";
                proveedor.fechaRegistro = DateTime.Now;
                proveedor.estado = 1;

                context.Proveedor.Add(proveedor);

                return context.SaveChanges();
            }
        }


        public static int modificar(Proveedor proveedor)
        {
            using (var context = new LabRodaproEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                context.Configuration.ProxyCreationEnabled = false;

                bool existe = context.Proveedor.Any(p =>
                    p.nit == proveedor.nit &&
                    p.id != proveedor.id &&
                    p.estado != -1);

                if (existe)
                    return -1;

                var existente = context.Proveedor
                    .FirstOrDefault(p => p.id == proveedor.id);

                if (existente == null)
                    return 0;

                existente.nombreEmpresa = proveedor.nombreEmpresa;
                existente.nit = proveedor.nit;
                existente.telefono = proveedor.telefono;
                existente.direccion = proveedor.direccion;
                existente.email = proveedor.email;

                return context.SaveChanges();
            }
        }


        public static int eliminar(int id, string usuario)
        {
            using (var context = new LabRodaproEntities())
            {
                var proveedor = context.Proveedor
                    .FirstOrDefault(p => p.id == id);

                if (proveedor == null)
                    return 0;

                proveedor.estado = -1;
                proveedor.usuarioRegistro = usuario;

                return context.SaveChanges();
            }
        }
    }
}