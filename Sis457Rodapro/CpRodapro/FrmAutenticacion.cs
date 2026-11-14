using ClnRodaPro;
using CadRodapro;
using System;
using System.Linq;
using System.Windows.Forms;

namespace CpRodapro
{
    public partial class FrmAutenticacion : Form
    {
        public FrmAutenticacion()
        {
            InitializeComponent();

            // EJECUTAR SOLO UNA VEZ:
            //crearAdministrador();
        }

        // CREAR ADMINISTRADOR
        private void crearAdministrador()
        {
            using (var context = new LabRodaproEntities())
            {
                // Verificar si ya existe el administrador
                var existe = context.Usuario
                    .Any(x => x.usuario1 == "admin");

                if (existe)
                {
                    MessageBox.Show("El administrador ya existe.");
                    return;
                }

                // Buscar empleado Gabriel
                var empleado = context.Empleado
                    .FirstOrDefault(x => x.id == 4);

                if (empleado == null)
                {
                    MessageBox.Show("No existe el empleado Gabriel.");
                    return;
                }

                // Crear administrador
                var nuevo = new Usuario
                {
                    idEmpleado = empleado.id,
                    usuario1 = "admin",
                    passwordHash =
                        CadRodapro.Util.Encrypt("admin123"),
                    rol = "Administrador",
                    usuarioRegistro = "admin",
                    fechaRegistro = DateTime.Now,
                    estado = 1
                };

                context.Usuario.Add(nuevo);
                context.SaveChanges();
            }

            MessageBox.Show(
                "Administrador creado correctamente."
            );
        }

        // INICIAR SESIÓN
        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if (!validarCampos())
                return;

            string nombreUsuario = txtUsuario.Text.Trim();
            string clave = txtClave.Text;

            var usuario = UsuarioCln.validar(
                nombreUsuario,
                clave
            );

            if (usuario == null)
            {
                MessageBox.Show(
                    "Usuario o contraseña incorrectos."
                );
                return;
            }

            if (usuario.estado != 1)
            {
                MessageBox.Show("Usuario inactivo.");
                return;
            }

            // Guardar usuario autenticado
            CpRodaPro.Util.usuario = usuario;

            MessageBox.Show("¡Bienvenido a RodaPro!");

            FrmPrincipal principal = new FrmPrincipal();
            principal.Show();

            this.Hide();
        }

        // SALIR
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // ENTER PARA INGRESAR
        private void txtClave_KeyPress(
            object sender,
            KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                btnIngresar.PerformClick();
            }
        }

        // CARGAR FORMULARIO
        private void FrmAutenticacion_Load(
            object sender,
            EventArgs e)
        {
            txtUsuario.Focus();
            txtClave.UseSystemPasswordChar = true;
        }

        // VALIDAR CAMPOS
        private bool validarCampos()
        {
            bool esValido = true;

            erpUsuario.Clear();
            erpClave.Clear();

            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                erpUsuario.SetError(
                    txtUsuario,
                    "El usuario es obligatorio"
                );

                esValido = false;
            }

            if (string.IsNullOrWhiteSpace(txtClave.Text))
            {
                erpClave.SetError(
                    txtClave,
                    "La contraseña es obligatoria"
                );

                esValido = false;
            }

            return esValido;
        }

        public void limpiarCampos()
        {
            txtUsuario.Clear();
            txtClave.Clear();

            erpUsuario.Clear();
            erpClave.Clear();

            txtUsuario.Focus();
        }
    }
}