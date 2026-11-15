
using CpRodaPro;
using System;
using System.Windows.Forms;

namespace CpRodapro
{
    public partial class FrmPrincipal : Form
    {
        public FrmPrincipal()
        {
            InitializeComponent();

            Load -= FrmPrincipal_Load;
            Load += FrmPrincipal_Load;
        }

        // ACTUALIZAR USUARIO
        private void actualizarUsuario()
        {
            if (Util.usuario != null)
            {
                lblUsuario.Text =
                    "Usuario: " + Util.usuario.usuario1;
            }
            else
            {
                lblUsuario.Text = "Usuario: Modo de prueba";
            }
        }

        // CARGAR FORMULARIO PRINCIPAL
        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            if (Util.usuario == null)
            {
                lblUsuario.Text = "Usuario: Sin sesión";
            }
            else
            {
                lblUsuario.Text = "Usuario: " + Util.usuario.usuario1;
            }
        }

        // PRODUCTOS
        private void btnProductos_Click(object sender, EventArgs e)
        {
            FrmProducto formulario = new FrmProducto();
            formulario.ShowDialog();
        }

        // PROVEEDORES
        private void btnProveedores_Click(object sender, EventArgs e)
        {
            FrmProveedor formulario = new FrmProveedor();
            formulario.ShowDialog();
        }

        // CLIENTES
        private void btnClientes_Click(object sender, EventArgs e)
        {
            FrmCliente formulario = new FrmCliente();
            formulario.ShowDialog();
        }

        // CERRAR SESIÓN
        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            DialogResult respuesta = MessageBox.Show(
                "¿Estás seguro de que deseas cerrar sesión?",
                "Cerrar sesión",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta == DialogResult.Yes)
            {
                Util.usuario = null;

                FrmAutenticacion login = null;

                foreach (Form formulario in Application.OpenForms)
                {
                    if (formulario is FrmAutenticacion)
                    {
                        login = (FrmAutenticacion)formulario;
                        break;
                    }
                }

                if (login != null)
                {
                    login.limpiarCampos();
                    login.Show();
                }
                else
                {
                    FrmAutenticacion nuevoLogin =
                        new FrmAutenticacion();

                    nuevoLogin.Show();
                }

                this.Close();
            }
        }

        // COMPRAS
        private void btnCompras_Click(object sender, EventArgs e)
        {
            FrmCompra formulario = new FrmCompra();

            if (Util.usuario != null)
            {
                formulario.IdUsuarioActual = Util.usuario.id;
                formulario.UsuarioActual = Util.usuario.usuario1;
            }

            formulario.ShowDialog();
        }

        // CATEGORÍAS
        private void btnCategorias_Click(object sender, EventArgs e)
        {
            FrmCategoria formulario = new FrmCategoria();
            formulario.ShowDialog();
        }

        // UNIDADES DE MEDIDA
        private void btnUnidades_Click(object sender, EventArgs e)
        {
            FrmUnidadMedida formulario =
                new FrmUnidadMedida();

            formulario.ShowDialog();
        }

        // USUARIOS
        private void btnUsuarios_Click(object sender, EventArgs e)
        {
            FrmUsuario formulario = new FrmUsuario();
            formulario.ShowDialog();
        }

        // VENTAS
        private void btnVentas_Click(object sender, EventArgs e)
        {
            
        }

        // EMPLEADOS
        private void btnEmpleados_Click(object sender, EventArgs e)
        {
            FrmEmpleados formulario = new FrmEmpleados();
            formulario.ShowDialog();
        }
    }
}