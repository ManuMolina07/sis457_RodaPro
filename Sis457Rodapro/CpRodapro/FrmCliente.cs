using CadRodapro;
using ClnRodaPro;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CpRodaPro
{
    public partial class FrmCliente : Form
    {
        private int id = 0;
        private bool modoEdicion = false;

        private const int ANCHO = 900;
        private const int ALTO_INICIAL = 357;
        private const int ALTO_REGISTRO = 600;

        public FrmCliente()
        {
            InitializeComponent();
        }

        private void FrmCliente_Load(object sender, EventArgs e)
        {
            Size = new Size(ANCHO, ALTO_INICIAL);
            pnlRegistro.Visible = false;
            cargarLista();
            limpiarFormulario();
            restablecerEstado();
        }

        private void cargarLista(string parametro = "")
        {
            dgvLista.DataSource = ClienteCln.listar(parametro);

            if (dgvLista.Columns.Contains("id"))
                dgvLista.Columns["id"].Visible = false;

            if (dgvLista.Columns.Contains("estado"))
                dgvLista.Columns["estado"].Visible = false;

            dgvLista.Columns["nombre"].HeaderText = "Nombre";
            dgvLista.Columns["apellido"].HeaderText = "Apellido";
            dgvLista.Columns["ciNit"].HeaderText = "CI / NIT";
            dgvLista.Columns["telefono"].HeaderText = "Teléfono";
            dgvLista.Columns["direccion"].HeaderText = "Dirección";
            dgvLista.Columns["usuarioRegistro"].HeaderText = "Usuario Registro";
            dgvLista.Columns["fechaRegistro"].HeaderText = "Fecha Registro";

            dgvLista.ClearSelection();
        }

        private void limpiarFormulario()
        {
            id = 0;

            txtNombre.Clear();
            txtApellido.Clear();
            txtCiNit.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();

            dgvLista.ClearSelection();
        }

        private void restablecerEstado()
        {
            modoEdicion = false;

            pnlRegistro.Visible = false;
            Size = new Size(ANCHO, ALTO_INICIAL);

            dgvLista.Enabled = true;

            btnNuevo.Enabled = true;
            btnModificar.Enabled = true;
            btnEliminar.Enabled = true;

            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;
        }

        private void habilitarRegistro()
        {
            pnlRegistro.Visible = true;
            Size = new Size(ANCHO, ALTO_REGISTRO);

            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

            txtNombre.Focus();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limpiarFormulario();

            modoEdicion = false;

            dgvLista.Enabled = false;

            btnNuevo.Enabled = false;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;

            habilitarRegistro();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                MessageBox.Show("Seleccione un cliente para modificar.");
                return;
            }

            modoEdicion = true;

            dgvLista.Enabled = false;

            btnNuevo.Enabled = false;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;

            habilitarRegistro();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingrese el nombre del cliente.");
                txtNombre.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtApellido.Text))
            {
                MessageBox.Show("Ingrese el apellido del cliente.");
                txtApellido.Focus();
                return;
            }

            var cliente = new Cliente
            {
                nombre = txtNombre.Text.Trim(),
                apellido = txtApellido.Text.Trim(),
                ciNit = string.IsNullOrWhiteSpace(txtCiNit.Text)
                    ? null : txtCiNit.Text.Trim(),
                telefono = string.IsNullOrWhiteSpace(txtTelefono.Text)
                    ? null : txtTelefono.Text.Trim(),
                direccion = string.IsNullOrWhiteSpace(txtDireccion.Text)
                    ? null : txtDireccion.Text.Trim()
            };

            int resultado;

            if (modoEdicion)
            {
                cliente.id = id;
                resultado = ClienteCln.modificar(cliente);
            }
            else
            {
                resultado = ClienteCln.crear(cliente);
            }

            if (resultado == -1)
            {
                MessageBox.Show("El CI / NIT ya se encuentra registrado.");
                txtCiNit.Focus();
                return;
            }

            if (resultado == 0)
            {
                MessageBox.Show("No se encontró el cliente.");
                return;
            }

            MessageBox.Show(
                modoEdicion
                    ? "Cliente modificado correctamente."
                    : "Cliente registrado correctamente.");

            cargarLista();
            limpiarFormulario();
            restablecerEstado();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            limpiarFormulario();
            restablecerEstado();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                MessageBox.Show("Seleccione un cliente para eliminar.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar este cliente?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta == DialogResult.Yes)
            {
                int resultado = ClienteCln.eliminar(id, "admin");

                if (resultado > 0)
                {
                    MessageBox.Show("Cliente eliminado correctamente.");

                    cargarLista();
                    limpiarFormulario();
                    restablecerEstado();
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar el cliente.");
                }
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            cargarLista(txtParametro.Text.Trim());
        }

        private void txtParametro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnBuscar.PerformClick();
            }
        }

        private void dgvLista_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvLista.Rows[e.RowIndex];

                id = Convert.ToInt32(fila.Cells["id"].Value);

                txtNombre.Text = fila.Cells["nombre"].Value?.ToString();
                txtApellido.Text = fila.Cells["apellido"].Value?.ToString();
                txtCiNit.Text = fila.Cells["ciNit"].Value?.ToString();
                txtTelefono.Text = fila.Cells["telefono"].Value?.ToString();
                txtDireccion.Text = fila.Cells["direccion"].Value?.ToString();
            }
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

