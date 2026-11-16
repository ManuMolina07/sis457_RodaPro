using CadRodapro;
using ClnRodaPro;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CpRodapro
{
    public partial class FrmProveedor : Form
    {
        private int id = 0;
        private bool modoEdicion = false;

        private const int ANCHO = 900;
        private const int ALTO_INICIAL = 357;
        private const int ALTO_REGISTRO = 600;

        public FrmProveedor()
        {
            InitializeComponent();

            // ASOCIAR EVENTOS
            Load -= FrmProveedor_Load;
            Load += FrmProveedor_Load;

            btnNuevo.Click -= btnNuevo_Click;
            btnNuevo.Click += btnNuevo_Click;

            btnModificar.Click -= btnModificar_Click;
            btnModificar.Click += btnModificar_Click;

            btnGuardar.Click -= btnGuardar_Click;
            btnGuardar.Click += btnGuardar_Click;

            btnCancelar.Click -= btnCancelar_Click;
            btnCancelar.Click += btnCancelar_Click;

            btnEliminar.Click -= btnEliminar_Click;
            btnEliminar.Click += btnEliminar_Click;

            btnBuscar.Click -= btnBuscar_Click;
            btnBuscar.Click += btnBuscar_Click;

            btnVolver.Click -= btnVolver_Click;
            btnVolver.Click += btnVolver_Click;

            txtParametro.KeyDown -= txtParametro_KeyDown;
            txtParametro.KeyDown += txtParametro_KeyDown;

            dgvLista.CellClick -= dgvLista_CellClick;
            dgvLista.CellClick += dgvLista_CellClick;

            // ESTADO INICIAL
            pnlRegistro.Visible = false;
            Size = new Size(ANCHO, ALTO_INICIAL);
        }

        // CARGAR FORMULARIO
        private void FrmProveedor_Load(object sender, EventArgs e)
        {
            Size = new Size(ANCHO, ALTO_INICIAL);

            pnlRegistro.Visible = false;

            cargarLista();

            limpiarFormulario();

            restablecerEstado();
        }

        // LISTAR Y BUSCAR
        private void cargarLista(string parametro = "")
        {
            try
            {
                dgvLista.DataSource = null;
                dgvLista.DataSource = ProveedorCln.listar(parametro);

                if (dgvLista.Columns.Contains("id"))
                    dgvLista.Columns["id"].Visible = false;

                if (dgvLista.Columns.Contains("estado"))
                    dgvLista.Columns["estado"].Visible = false;

                if (dgvLista.Columns.Contains("nombreEmpresa"))
                    dgvLista.Columns["nombreEmpresa"].HeaderText = "Empresa";

                if (dgvLista.Columns.Contains("nit"))
                    dgvLista.Columns["nit"].HeaderText = "NIT";

                if (dgvLista.Columns.Contains("telefono"))
                    dgvLista.Columns["telefono"].HeaderText = "Teléfono";

                if (dgvLista.Columns.Contains("direccion"))
                    dgvLista.Columns["direccion"].HeaderText = "Dirección";

                if (dgvLista.Columns.Contains("email"))
                    dgvLista.Columns["email"].HeaderText = "Correo";

                if (dgvLista.Columns.Contains("usuarioRegistro"))
                    dgvLista.Columns["usuarioRegistro"].HeaderText = "Usuario Registro";

                if (dgvLista.Columns.Contains("fechaRegistro"))
                {
                    dgvLista.Columns["fechaRegistro"].HeaderText = "Fecha Registro";
                    dgvLista.Columns["fechaRegistro"].DefaultCellStyle.Format =
                        "dd/MM/yyyy HH:mm";
                }

                if (dgvLista.Columns.Contains("Compra"))
                    dgvLista.Columns["Compra"].Visible = false;

                dgvLista.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;

                dgvLista.ReadOnly = true;
                dgvLista.AllowUserToAddRows = false;
                dgvLista.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;
                dgvLista.MultiSelect = false;
                dgvLista.RowHeadersVisible = false;

                dgvLista.ClearSelection();

                id = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar los proveedores: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // LIMPIAR FORMULARIO
        private void limpiarFormulario()
        {
            id = 0;

            txtNombre.Clear();
            txtNit.Clear();
            txtTelefono.Clear();
            txtDireccion.Clear();
            txtCorreo.Clear();

            dgvLista.ClearSelection();
        }

        // RESTABLECER ESTADO
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

        // HABILITAR REGISTRO
        private void habilitarRegistro()
        {
            Size = new Size(ANCHO, ALTO_REGISTRO);

            pnlRegistro.Visible = true;
            pnlRegistro.BringToFront();

            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

            txtNombre.Focus();
        }

        // ASIGNAR DATOS
        private Proveedor asignarDatos()
        {
            return new Proveedor
            {
                nombreEmpresa = txtNombre.Text.Trim(),
                nit = txtNit.Text.Trim(),
                telefono = txtTelefono.Text.Trim(),
                direccion = txtDireccion.Text.Trim(),
                email = txtCorreo.Text.Trim()
            };
        }

        // VALIDAR DATOS
        private bool validarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingrese el nombre de la empresa.");
                txtNombre.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNit.Text))
            {
                MessageBox.Show("Ingrese el NIT o CI.");
                txtNit.Focus();
                return false;
            }

            return true;
        }

        // NUEVO
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

        // MODIFICAR
        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                MessageBox.Show("Seleccione un proveedor de la lista.");
                return;
            }

            modoEdicion = true;

            dgvLista.Enabled = false;

            btnNuevo.Enabled = false;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;

            habilitarRegistro();
        }

        // GUARDAR
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!validarDatos())
                return;

            try
            {
                Proveedor proveedor = asignarDatos();

                int resultado;

                if (modoEdicion)
                {
                    proveedor.id = id;

                    resultado = ProveedorCln.modificar(proveedor);

                    if (resultado == -1)
                    {
                        MessageBox.Show("El NIT ya pertenece a otro proveedor.");
                        txtNit.Focus();
                        return;
                    }

                    if (resultado == 0)
                    {
                        MessageBox.Show("No se encontró el proveedor.");
                        return;
                    }

                    MessageBox.Show("Proveedor modificado correctamente.");
                }
                else
                {
                    resultado = ProveedorCln.crear(proveedor);

                    if (resultado == -1)
                    {
                        MessageBox.Show("El NIT ya está registrado.");
                        txtNit.Focus();
                        return;
                    }

                    if (resultado == 0)
                    {
                        MessageBox.Show("No se pudo registrar el proveedor.");
                        return;
                    }

                    MessageBox.Show("Proveedor registrado correctamente.");
                }

                cargarLista();

                limpiarFormulario();

                restablecerEstado();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al guardar proveedor: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // CANCELAR
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            limpiarFormulario();

            restablecerEstado();
        }

        // ELIMINAR
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                MessageBox.Show("Seleccione un proveedor de la lista.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar este proveedor?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                int resultado = ProveedorCln.eliminar(id, "admin");

                if (resultado == 0)
                {
                    MessageBox.Show("No se encontró el proveedor.");
                    return;
                }

                MessageBox.Show("Proveedor eliminado correctamente.");

                cargarLista();

                limpiarFormulario();

                restablecerEstado();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al eliminar proveedor: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // BUSCAR
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            cargarLista(txtParametro.Text.Trim());
        }

        // BUSCAR AL PRESIONAR ENTER
        private void txtParametro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;

                btnBuscar.PerformClick();
            }
        }

        // SELECCIONAR PROVEEDOR
        private void dgvLista_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || modoEdicion)
                return;

            DataGridViewRow fila = dgvLista.Rows[e.RowIndex];

            id = Convert.ToInt32(fila.Cells["id"].Value);

            txtNombre.Text =
                Convert.ToString(fila.Cells["nombreEmpresa"].Value);

            txtNit.Text =
                Convert.ToString(fila.Cells["nit"].Value);

            txtTelefono.Text =
                Convert.ToString(fila.Cells["telefono"].Value);

            txtDireccion.Text =
                Convert.ToString(fila.Cells["direccion"].Value);

            txtCorreo.Text =
                Convert.ToString(fila.Cells["email"].Value);
        }

        // VOLVER
        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
