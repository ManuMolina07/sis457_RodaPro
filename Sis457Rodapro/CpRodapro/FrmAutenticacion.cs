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

            // OCULTAR CONTRASEÑA
            txtClave.UseSystemPasswordChar = true;

            // PERMITIR INGRESAR AL PRESIONAR ENTER
            this.AcceptButton = btnIngresar;
        }

        // CREAR ADMINISTRADOR
        private void crearAdministrador()
        {
            using (var context = new LabRodaproEntities())
            {
                var existe = context.Usuario
                    .Any(x => x.usuario1 == "admin");

                if (existe)
                {
                    MessageBox.Show("El administrador ya existe.");
                    return;
                }

                //PARA CREAR EL ADMINISTRADOR, PRIMERO DEBE EXISTIR EL EMPLEADO GABRIEL(DUEÑO) CON CÉDULA 10329801
                var empleado = context.Empleado.FirstOrDefault(x => x.cedulaIdentidad == "10329801");

                if (empleado == null)
                {
                    MessageBox.Show("No existe el empleado Gabriel.");
                    return;
                }

                var nuevo = new Usuario
                {
                    idEmpleado = empleado.id,
                    usuario1 = "admin",
                    passwordHash = CadRodapro.Util.Encrypt("admin123"),
                    rol = "Administrador",
                    usuarioRegistro = "admin",
                    fechaRegistro = DateTime.Now,
                    estado = 1
                };

                context.Usuario.Add(nuevo);
                context.SaveChanges();
            }

            MessageBox.Show("Administrador creado correctamente.");
        }

        // INICIAR SESIÓN
        private void btnIngresar_Click(object sender, EventArgs e)
        {
            if (!validarCampos())
                return;

            try
            {
                string nombreUsuario = txtUsuario.Text.Trim();
                string clave = txtClave.Text;

                var usuario = UsuarioCln.validar(
                    nombreUsuario,
                    clave
                );

                if (usuario == null)
                {
                    MessageBox.Show(
                        "Usuario o contraseña incorrectos.",
                        "Acceso denegado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtClave.Clear();
                    txtClave.Focus();
                    return;
                }

                if (usuario.estado != 1)
                {
                    MessageBox.Show(
                        "Usuario inactivo.",
                        "Acceso denegado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtClave.Clear();
                    txtClave.Focus();
                    return;
                }

                // GUARDAR USUARIO AUTENTICADO
                CpRodaPro.Util.usuario = usuario;

                MessageBox.Show("¡Bienvenido a RodaPro!");

                FrmPrincipal principal = new FrmPrincipal();
                principal.Show();

                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al iniciar sesión: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // SALIR
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
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
            erpUsuario.Clear();
            erpClave.Clear();

            bool esValido = true;

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

        // LIMPIAR ERROR AL HACER CLIC EN USUARIO
        private void txtUsuario_Click(object sender, EventArgs e)
        {
            erpUsuario.SetError(txtUsuario, "");
        }

        // LIMPIAR ERROR AL HACER CLIC EN CLAVE
        private void txtClave_Click(object sender, EventArgs e)
        {
            erpClave.SetError(txtClave, "");
        }

        // LIMPIAR ERROR AL ESCRIBIR USUARIO
        private void txtUsuario_TextChanged(object sender, EventArgs e)
        {
            erpUsuario.SetError(txtUsuario, "");
        }

        // LIMPIAR ERROR AL ESCRIBIR CLAVE
        private void txtClave_TextChanged(object sender, EventArgs e)
        {
            erpClave.SetError(txtClave, "");
        }

        // LIMPIAR CAMPOS
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

