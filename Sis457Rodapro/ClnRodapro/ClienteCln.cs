using CadRodapro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClnRodaPro
{
    public class ClienteCln
    {
        public static List<Cliente> listar(string parametro)
        {
            using (var context = new LabRodaproEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                context.Configuration.ProxyCreationEnabled = false;

                parametro = parametro ?? "";

                return context.Cliente
                    .Where(c => c.estado == 1 &&
                        (c.nombre.Contains(parametro) ||
                         c.apellido.Contains(parametro) ||
                         (c.ciNit != null && c.ciNit.Contains(parametro)) ||
                         (c.telefono != null && c.telefono.Contains(parametro)) ||
                         (c.direccion != null && c.direccion.Contains(parametro))))
                    .OrderBy(c => c.nombre)
                    .ThenBy(c => c.apellido)
                    .ToList();
            }
        }

        public static int crear(Cliente cliente)
        {
            using (var context = new LabRodaproEntities())
            {
                if (!string.IsNullOrWhiteSpace(cliente.ciNit))
                {
                    bool existe = context.Cliente.Any(c =>
                        c.ciNit == cliente.ciNit && c.estado != -1);

                    if (existe)
                        return -1;
                }

                cliente.usuarioRegistro = "admin";
                cliente.fechaRegistro = DateTime.Now;
                cliente.estado = 1;

                context.Cliente.Add(cliente);

                return context.SaveChanges();
            }
        }

        public static int modificar(Cliente cliente)
        {
            using (var context = new LabRodaproEntities())
            {
                if (!string.IsNullOrWhiteSpace(cliente.ciNit))
                {
                    bool existe = context.Cliente.Any(c =>
                        c.ciNit == cliente.ciNit &&
                        c.id != cliente.id &&
                        c.estado != -1);

                    if (existe)
                        return -1;
                }

                var existente = context.Cliente
                    .FirstOrDefault(c => c.id == cliente.id);

                if (existente == null)
                    return 0;

                existente.nombre = cliente.nombre;
                existente.apellido = cliente.apellido;
                existente.ciNit = cliente.ciNit;
                existente.telefono = cliente.telefono;
                existente.direccion = cliente.direccion;

                return context.SaveChanges();
            }
        }

        public static int eliminar(int id, string usuario)
        {
            using (var context = new LabRodaproEntities())
            {
                var cliente = context.Cliente
                    .FirstOrDefault(c => c.id == id);

                if (cliente == null)
                    return 0;

                cliente.estado = -1;
                cliente.usuarioRegistro = usuario;

                return context.SaveChanges();
            }
        }
    }
}
