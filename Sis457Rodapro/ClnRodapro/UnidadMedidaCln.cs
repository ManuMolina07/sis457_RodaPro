using CadRodapro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClnRodaPro
{
    public class UnidadMedidaCln
    {
        public static List<UnidadMedida> listar()
        {
            using (var context = new LabRodaproEntities())
            {
                return context.UnidadMedida
                    .Where(x => x.estado == (short)Estado.Activo)
                    .OrderBy(x => x.descripcion)
                    .ToList();
            }
        }

        public static int insertar(UnidadMedida unidadMedida)
        {
            using (var context = new LabRodaproEntities())
            {
                unidadMedida.estado = (short)Estado.Activo;
                unidadMedida.fechaRegistro = DateTime.Now;

                context.UnidadMedida.Add(unidadMedida);
                context.SaveChanges();

                return unidadMedida.id;
            }
        }

        public static void actualizar(UnidadMedida unidadMedida)
        {
            using (var context = new LabRodaproEntities())
            {
                var registro = context.UnidadMedida
                    .FirstOrDefault(x => x.id == unidadMedida.id);

                if (registro == null)
                    throw new Exception("La unidad de medida no existe.");

                registro.descripcion = unidadMedida.descripcion;

                context.SaveChanges();
            }
        }

        public static void eliminar(int id)
        {
            using (var context = new LabRodaproEntities())
            {
                var registro = context.UnidadMedida
                    .FirstOrDefault(x => x.id == id &&
                        x.estado == (short)Estado.Activo);

                if (registro == null)
                    throw new Exception("La unidad de medida no existe.");

                registro.estado = (short)Estado.Eliminado;

                context.SaveChanges();
            }
        }
    }
}
