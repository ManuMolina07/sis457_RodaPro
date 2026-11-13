using CadRodapro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClnRodaPro
{
    public class CategoriaCln
    {
        public static int crear(Categoria categoria)
        {
            using (var context = new LabRodaproEntities())
            {
                var existe = context.Categoria
                    .Where(x => x.nombre == categoria.nombre)
                    .FirstOrDefault();

                if (existe != null)
                    throw new Exception($"La categoria {categoria.nombre} ya existe");

                context.Categoria.Add(categoria);
                context.SaveChanges();

                return categoria.id;
            }
        }

        public static int actualizar(Categoria categoria)
        {
            using (var context = new LabRodaproEntities())
            {
                var existe = context.Categoria.Find(categoria.id);

                if (existe == null)
                    throw new Exception($"La categoria con id {categoria.id} no existe");

                existe.nombre = categoria.nombre;
                existe.descripcion = categoria.descripcion;
                existe.usuarioRegistro = categoria.usuarioRegistro;

                return context.SaveChanges();
            }
        }

        public static int eliminar(int id, string usuario)
        {
            using (var context = new LabRodaproEntities())
            {
                var existe = context.Categoria.Find(id);

                if (existe == null)
                    throw new Exception($"La categoria con id {id} no existe");

                existe.estado = (short)Estado.Eliminado;
                existe.usuarioRegistro = usuario;

                return context.SaveChanges();
            }
        }

        public static Categoria obtenerUno(int id)
        {
            using (var context = new LabRodaproEntities())
            {
                var categoria = context.Categoria.Find(id);

                if (categoria == null)
                    throw new Exception($"La categoria con id {id} no existe");

                return categoria;
            }
        }

        public static List<Categoria> listar()
        {
            using (var context = new LabRodaproEntities())
            {
                return context.Categoria
                    .Where(x => x.estado == (short)Estado.Activo)
                    .OrderBy(x => x.nombre)
                    .ToList();
            }
        }
    }
}
