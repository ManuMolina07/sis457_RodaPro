
using CadRodapro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClnRodaPro
{
    public class CategoriaCln
    {
        // CREAR CATEGORÍA
        public static int crear(Categoria categoria)
        {
            if (categoria == null)
                throw new ArgumentNullException(nameof(categoria));

            validarNombre(categoria.nombre);

            using (var context = new LabRodaproEntities())
            {
                validarNombreDisponible(
                    context,
                    categoria.nombre
                );

                categoria.estado = (short)Estado.Activo;
                categoria.fechaRegistro = DateTime.Now;

                context.Categoria.Add(categoria);

                context.SaveChanges();

                return categoria.id;
            }
        }

        // ACTUALIZAR CATEGORÍA
        public static int actualizar(Categoria categoria)
        {
            if (categoria == null)
                throw new ArgumentNullException(nameof(categoria));

            validarNombre(categoria.nombre);

            using (var context = new LabRodaproEntities())
            {
                var existe = context.Categoria.Find(categoria.id);

                if (existe == null)
                    throw new Exception(
                        $"La categoría con ID {categoria.id} no existe."
                    );

                if (existe.estado != (short)Estado.Activo)
                    throw new Exception(
                        "No se puede modificar una categoría inactiva o eliminada."
                    );

                validarNombreDisponible(
                    context,
                    categoria.nombre,
                    categoria.id
                );

                existe.nombre = categoria.nombre.Trim();
                existe.descripcion = categoria.descripcion;
                existe.usuarioRegistro = categoria.usuarioRegistro;

                return context.SaveChanges();
            }
        }

        // ELIMINACIÓN LÓGICA
        public static int eliminar(int id, string usuario)
        {
            using (var context = new LabRodaproEntities())
            {
                var existe = context.Categoria.Find(id);

                if (existe == null)
                    throw new Exception(
                        $"La categoría con ID {id} no existe."
                    );

                if (existe.estado != (short)Estado.Activo)
                    throw new Exception(
                        "La categoría ya no se encuentra activa."
                    );

                existe.estado = (short)Estado.Eliminado;
                existe.usuarioRegistro = usuario;

                return context.SaveChanges();
            }
        }

        // OBTENER UNA CATEGORÍA
        public static Categoria obtenerUno(int id)
        {
            using (var context = new LabRodaproEntities())
            {
                var categoria = context.Categoria.Find(id);

                if (categoria == null)
                    throw new Exception(
                        $"La categoría con ID {id} no existe."
                    );

                return categoria;
            }
        }

        // LISTAR CATEGORÍAS ACTIVAS
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

        // VALIDAR NOMBRE
        private static void validarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException(
                    "El nombre de la categoría es obligatorio."
                );
        }

        // VALIDAR QUE EL NOMBRE NO ESTÉ DUPLICADO
        private static void validarNombreDisponible(
            LabRodaproEntities context,
            string nombre,
            int? idExcluir = null)
        {
            nombre = nombre.Trim();

            bool existe = context.Categoria.Any(x =>
                x.estado == (short)Estado.Activo &&
                x.nombre == nombre &&
                (!idExcluir.HasValue || x.id != idExcluir.Value)
            );

            if (existe)
                throw new Exception(
                    $"La categoría '{nombre}' ya existe."
                );
        }
    }
}
