using CadRodapro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClnRodaPro
{
    public class EmpleadoCln
    {
        // LISTAR EMPLEADOS ACTIVOS
        public static List<Empleado> listar()
        {
            using (var context = new LabRodaproEntities())
            {
                return context.Empleado
                    .Where(x => x.estado == (short)Estado.Activo)
                    .OrderBy(x => x.nombres)
                    .ThenBy(x => x.primerApellido)
                    .ToList();
            }
        }

        // INSERTAR EMPLEADO
        public static int insertar(Empleado empleado, string usuarioRegistro)
        {
            using (var context = new LabRodaproEntities())
            {
                empleado.usuarioRegistro = usuarioRegistro;
                empleado.fechaRegistro = DateTime.Now;
                empleado.estado = (short)Estado.Activo;

                context.Empleado.Add(empleado);
                return context.SaveChanges();
            }
        }

        // ACTUALIZAR EMPLEADO
        public static int actualizar(Empleado empleado, string usuarioRegistro)
        {
            using (var context = new LabRodaproEntities())
            {
                var registro = context.Empleado
                    .FirstOrDefault(x => x.id == empleado.id);

                if (registro == null)
                    return 0;

                registro.cedulaIdentidad = empleado.cedulaIdentidad;
                registro.nombres = empleado.nombres;
                registro.primerApellido = empleado.primerApellido;
                registro.segundoApellido = empleado.segundoApellido;
                registro.fechaNacimiento = empleado.fechaNacimiento;
                registro.direccion = empleado.direccion;
                registro.celular = empleado.celular;
                registro.cargo = empleado.cargo;
                registro.usuarioRegistro = usuarioRegistro;

                return context.SaveChanges();
            }
        }

        // ELIMINACIÓN LÓGICA
        public static int eliminar(int id, string usuarioRegistro)
        {
            using (var context = new LabRodaproEntities())
            {
                var registro = context.Empleado
                    .FirstOrDefault(x => x.id == id);

                if (registro == null)
                    return 0;

                registro.estado = (short)Estado.Eliminado;
                registro.usuarioRegistro = usuarioRegistro;

                return context.SaveChanges();
            }
        }
    }
}
