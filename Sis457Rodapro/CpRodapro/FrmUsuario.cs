using CadRodapro;
using ClnRodaPro;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CpRodapro
{
    public partial class FrmUsuario : Form
    {
        private int id = 0;
        private bool modoEdicion = false;

        private const int ANCHO = 900;
        private const int ALTO_INICIAL = 357;
        private const int ALTO_REGISTRO = 600;

        public FrmUsuario()
        {
            InitializeComponent();

            // ASOCIAR EVENTOS
            Load -= FrmUsuario_Load;
            Load += FrmUsuario_Load;

            btnBuscar.Click -= btnBuscar_Click;
            btnBuscar.Click += btnBuscar_Click;

            btnNuevo.Click -= btnNuevo_Click;
            btnNuevo.Click += btnNuevo_Click;

            btnGuardar.Click -= btnGuardar_Click;
            btnGuardar.Click += btnGuardar_Click;

            btnModificar.Click -= btnModificar_Click;
            btnModificar.Click += btnModificar_Click;

            btnEliminar.Click -= btnEliminar_Click;
            btnEliminar.Click += btnEliminar_Click;

            btnCancelar.Click -= btnCancelar_Click;
            btnCancelar.Click += btnCancelar_Click;

            btnVolver.Click -= btnVolver_Click;
            btnVolver.Click += btnVolver_Click;

            txtParametro.KeyDown -= txtParametro_KeyDown;
            txtParametro.KeyDown += txtParametro_KeyDown;

            // CONFIGURAR ROLES
            cbxRol.Items.Clear();
            cbxRol.Items.Add("Administrador");
            cbxRol.Items.Add("Vendedor");
            cbxRol.DropDownStyle = ComboBoxStyle.DropDownList;

            // CONFIGURAR CONTRASEÑA
            txtClave.PasswordChar = '●';
        }

        // CARGAR FORMULARIO
        private void FrmUsuario_Load(object sender, EventArgs e)
        {
            Size = new Size(ANCHO, ALTO_INICIAL);

            cargarEmpleados();
            listar();

            restablecerEstado();
        }

        // CARGAR EMPLEADOS
        private void cargarEmpleados()
        {
            try
            {
                var empleados = EmpleadoCln.listar()
                    .Select(x => new
                    {
                        id = x.id,
                        nombreCompleto = x.nombres + " " +
                            x.primerApellido + " " +
                            x.segundoApellido
                    })
                    .ToList();

                cbxEmpleado.DataSource = null;
                cbxEmpleado.DataSource = empleados;
                cbxEmpleado.DisplayMember = "nombreCompleto";
                cbxEmpleado.ValueMember = "id";
                cbxEmpleado.SelectedIndex = -1;
                cbxEmpleado.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar empleados: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // HABILITAR REGISTRO
        private void habilitarRegistro()
        {
            pnlRegistro.Visible = true;

            Size = new Size(ANCHO, ALTO_REGISTRO);

            pnlRegistro.BringToFront();

            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;
        }

        // RESTABLECER ESTADO
        private void restablecerEstado()
        {
            modoEdicion = false;

            pnlRegistro.Visible = false;

            Size = new Size(ANCHO, ALTO_INICIAL);

            limpiarFormulario();

            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;
        }

        // LIMPIAR FORMULARIO
        private void limpiarFormulario()
        {
            id = 0;
            modoEdicion = false;

            cbxEmpleado.SelectedIndex = -1;
            cbxRol.SelectedIndex = -1;

            txtUsuario.Clear();
            txtClave.Clear();
        }

        // LISTAR USUARIOS
        private void listar()
        {
            try
            {
                string parametro = txtParametro.Text.Trim();

                var lista = UsuarioCln.listar();

                if (!string.IsNullOrEmpty(parametro))
                {
                    lista = lista
                        .Where(x =>
                            (x.usuario1 != null &&
                             x.usuario1.IndexOf(
                                 parametro,
                                 StringComparison.OrdinalIgnoreCase) >= 0)
                            ||
                            (x.rol != null &&
                             x.rol.IndexOf(
                                 parametro,
                                 StringComparison.OrdinalIgnoreCase) >= 0))
                        .ToList();
                }

                dgvLista.DataSource = null;
                dgvLista.DataSource = lista;

                dgvLista.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;

                dgvLista.ReadOnly = true;
                dgvLista.AllowUserToAddRows = false;
                dgvLista.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;

                dgvLista.MultiSelect = false;
                dgvLista.RowHeadersVisible = false;

                // OCULTAR COLUMNAS
                string[] columnasOcultas =
                {
                    "id",
                    "estado",
                    "passwordHash",
                    "Empleado",
                    "Compra",
                    "MovimientoInventario",
                    "Pago",
                    "Venta"
                };

                foreach (string columna in columnasOcultas)
                {
                    if (dgvLista.Columns.Contains(columna))
                        dgvLista.Columns[columna].Visible = false;
                }

                // CAMBIAR ENCABEZADOS
                if (dgvLista.Columns.Contains("idEmpleado"))
                    dgvLista.Columns["idEmpleado"].HeaderText = "ID Empleado";

                if (dgvLista.Columns.Contains("usuario1"))
                    dgvLista.Columns["usuario1"].HeaderText = "Usuario";

                if (dgvLista.Columns.Contains("rol"))
                    dgvLista.Columns["rol"].HeaderText = "Rol";

                if (dgvLista.Columns.Contains("usuarioRegistro"))
                    dgvLista.Columns["usuarioRegistro"].HeaderText =
                        "Registrado por";

                if (dgvLista.Columns.Contains("fechaRegistro"))
                {
                    dgvLista.Columns["fechaRegistro"].HeaderText =
                        "Fecha de registro";

                    dgvLista.Columns["fechaRegistro"].DefaultCellStyle.Format =
                        "dd/MM/yyyy HH:mm";
                }

                dgvLista.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al listar usuarios: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // OBTENER USUARIO SELECCIONADO
        private Usuario obtenerSeleccionado()
        {
            if (dgvLista.CurrentRow == null ||
                dgvLista.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show(
                    "Seleccione un usuario.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return null;
            }

            return dgvLista.CurrentRow.DataBoundItem as Usuario;
        }

        // BOTÓN BUSCAR
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            listar();
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

        // BOTÓN NUEVO
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limpiarFormulario();

            habilitarRegistro();

            txtUsuario.Focus();
        }

        // BOTÓN MODIFICAR
        private void btnModificar_Click(object sender, EventArgs e)
        {
            Usuario usuario = obtenerSeleccionado();

            if (usuario == null)
                return;

            id = usuario.id;
            modoEdicion = true;

            cbxEmpleado.SelectedValue = usuario.idEmpleado;
            txtUsuario.Text = usuario.usuario1;
            cbxRol.SelectedItem = usuario.rol;

            // La contraseña queda vacía.
            // Si no se escribe una nueva, se conserva la anterior.
            txtClave.Clear();

            habilitarRegistro();

            txtUsuario.Focus();
        }

        // BOTÓN GUARDAR
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbxEmpleado.SelectedIndex == -1)
                {
                    MessageBox.Show(
                        "Seleccione un empleado.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    cbxEmpleado.Focus();
                    return;
                }

                string nombreUsuario = txtUsuario.Text.Trim();
                string clave = txtClave.Text;
                string rol = cbxRol.Text;

                if (string.IsNullOrWhiteSpace(nombreUsuario))
                {
                    MessageBox.Show(
                        "Ingrese el nombre de usuario.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtUsuario.Focus();
                    return;
                }

                if (nombreUsuario.Length > 50)
                {
                    MessageBox.Show(
                        "El nombre de usuario no puede superar los 50 caracteres.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtUsuario.Focus();
                    return;
                }

                if (!modoEdicion && string.IsNullOrWhiteSpace(clave))
                {
                    MessageBox.Show(
                        "Ingrese una contraseña.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtClave.Focus();
                    return;
                }

                if (!string.IsNullOrWhiteSpace(clave) && clave.Length < 6)
                {
                    MessageBox.Show(
                        "La contraseña debe tener al menos 6 caracteres.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtClave.Focus();
                    return;
                }

                if (cbxRol.SelectedIndex == -1)
                {
                    MessageBox.Show(
                        "Seleccione un rol.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    cbxRol.Focus();
                    return;
                }

                int idEmpleado = Convert.ToInt32(cbxEmpleado.SelectedValue);

                Usuario usuario = new Usuario
                {
                    id = id,
                    idEmpleado = idEmpleado,
                    usuario1 = nombreUsuario,
                    rol = rol
                };

                // TEMPORALMENTE SE REGISTRA CON ADMIN
                string usuarioRegistro = "admin";

                if (modoEdicion)
                {
                    UsuarioCln.actualizar(
                        usuario,
                        clave,
                        usuarioRegistro);

                    MessageBox.Show(
                        "El usuario fue modificado correctamente.",
                        "Confirmación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    UsuarioCln.insertar(
                        usuario,
                        clave,
                        usuarioRegistro);

                    MessageBox.Show(
                        "El usuario fue registrado correctamente.",
                        "Confirmación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                restablecerEstado();

                listar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al guardar el usuario: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // BOTÓN ELIMINAR
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Usuario usuario = obtenerSeleccionado();

            if (usuario == null)
                return;

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar al usuario " +
                usuario.usuario1 + "?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                UsuarioCln.eliminar(usuario.id);

                MessageBox.Show(
                    "El usuario fue eliminado correctamente.",
                    "Confirmación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                listar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al eliminar el usuario: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // BOTÓN CANCELAR
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            restablecerEstado();
        }

        // BOTÓN VOLVER
        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
