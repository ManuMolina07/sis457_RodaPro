
using CadRodapro;
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
    }
}