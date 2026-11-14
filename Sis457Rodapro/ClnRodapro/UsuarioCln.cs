using CadRodapro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClnRodaPro
{
    public class UsuarioCln
    {
        // BUSCAR USUARIO POR NOMBRE
        public static Usuario buscarPorUsuario(string usuario)
        {
            using (var context = new LabRodaproEntities())
            {
                return context.Usuario
                    .FirstOrDefault(x => x.usuario1 == usuario);
            }
        }

        // VALIDAR USUARIO Y CONTRASEÑA
        public static Usuario validar(string usuario, string clave)
        {
            string hashGenerado = Util.Encrypt(clave);

            using (var context = new LabRodaproEntities())
            {
                return context.Usuario
                    .FirstOrDefault(x =>
                        x.usuario1 == usuario &&
                        x.passwordHash == hashGenerado &&
                        x.estado == (short)Estado.Activo);
            }
        }

        // LISTAR USUARIOS ACTIVOS
        public static List<Usuario> listar()
        {
            using (var context = new LabRodaproEntities())
            {
                return context.Usuario
                    .Where(x => x.estado == (short)Estado.Activo)
                    .OrderBy(x => x.usuario1)
                    .ToList();
            }
        }

        // REGISTRAR USUARIO
        public static int insertar(
            Usuario usuario,
            string clave,
            string usuarioRegistro)
        {
            using (var context = new LabRodaproEntities())
            {
                bool existe = context.Usuario.Any(x =>
                    x.usuario1 == usuario.usuario1 &&
                    x.estado != (short)Estado.Eliminado);

                if (existe)
                    throw new Exception("El nombre de usuario ya está registrado.");

                usuario.passwordHash = Util.Encrypt(clave);
                usuario.usuarioRegistro = usuarioRegistro;
                usuario.fechaRegistro = DateTime.Now;
                usuario.estado = (short)Estado.Activo;

                context.Usuario.Add(usuario);
                context.SaveChanges();

                return usuario.id;
            }
        }

        // MODIFICAR USUARIO
        public static void actualizar(
            Usuario usuario,
            string clave,
            string usuarioRegistro)
        {
            using (var context = new LabRodaproEntities())
            {
                var registro = context.Usuario
                    .FirstOrDefault(x => x.id == usuario.id);

                if (registro == null)
                    throw new Exception("El usuario no existe.");

                bool existe = context.Usuario.Any(x =>
                    x.usuario1 == usuario.usuario1 &&
                    x.id != usuario.id &&
                    x.estado != (short)Estado.Eliminado);

                if (existe)
                    throw new Exception("El nombre de usuario ya está registrado.");

                registro.idEmpleado = usuario.idEmpleado;
                registro.usuario1 = usuario.usuario1;
                registro.rol = usuario.rol;
                registro.usuarioRegistro = usuarioRegistro;

                // Si se ingresó una nueva contraseña, actualizarla.
                // Si está vacía, conservar la contraseña anterior.
                if (!string.IsNullOrWhiteSpace(clave))
                {
                    registro.passwordHash = Util.Encrypt(clave);
                }

                context.SaveChanges();
            }
        }

        // ELIMINACIÓN LÓGICA
        public static void eliminar(int id)
        {
            using (var context = new LabRodaproEntities())
            {
                var registro = context.Usuario
                    .FirstOrDefault(x =>
                        x.id == id &&
                        x.estado == (short)Estado.Activo);

                if (registro == null)
                    throw new Exception("El usuario no existe.");

                registro.estado = (short)Estado.Eliminado;

                context.SaveChanges();
            }
        }
    }
}