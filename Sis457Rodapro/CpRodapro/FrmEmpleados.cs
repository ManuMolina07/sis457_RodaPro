
using ClnRodaPro;
using CadRodapro;
using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace CpRodapro
{
    public partial class FrmEmpleados : Form
    {
        private int id = 0;
        private bool modoEdicion = false;

        private const int ANCHO = 900;
        private const int ALTO_INICIAL = 350;
        private const int ALTO_REGISTRO = 600;

        public FrmEmpleados()
        {
            InitializeComponent();

            // ASOCIAR EVENTOS SIN DUPLICARLOS
            Load -= FrmEmpleados_Load;
            Load += FrmEmpleados_Load;

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

            btnBuscar.Click -= btnBuscar_Click;
            btnBuscar.Click += btnBuscar_Click;

            txtParametro.KeyDown -= txtParametro_KeyDown;
            txtParametro.KeyDown += txtParametro_KeyDown;

            // CONFIGURAR DATAGRIDVIEW
            dgvListaEmpleados.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvListaEmpleados.MultiSelect = false;
            dgvListaEmpleados.ReadOnly = true;
            dgvListaEmpleados.AllowUserToAddRows = false;

            dgvListaEmpleados.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvListaEmpleados.DataBindingComplete -=
                dgvListaEmpleados_DataBindingComplete;

            dgvListaEmpleados.DataBindingComplete +=
                dgvListaEmpleados_DataBindingComplete;

            dgvListaEmpleados.CellClick -=
                dgvListaEmpleados_CellClick;

            dgvListaEmpleados.CellClick +=
                dgvListaEmpleados_CellClick;
        }

        // CARGAR FORMULARIO
        private void FrmEmpleados_Load(object sender, EventArgs e)
        {
            Size = new Size(ANCHO, ALTO_INICIAL);

            listar();
            restablecerEstado();
        }

        // LISTAR EMPLEADOS
        private void listar()
        {
            try
            {
                string parametro = txtParametro.Text.Trim();

                var datos = EmpleadoCln.listar();

                if (!string.IsNullOrEmpty(parametro))
                {
                    datos = datos.Where(x =>
                        (x.cedulaIdentidad ?? "")
                            .IndexOf(parametro, StringComparison.OrdinalIgnoreCase) >= 0 ||

                        (x.nombres ?? "")
                            .IndexOf(parametro, StringComparison.OrdinalIgnoreCase) >= 0 ||

                        (x.primerApellido ?? "")
                            .IndexOf(parametro, StringComparison.OrdinalIgnoreCase) >= 0 ||

                        (x.segundoApellido ?? "")
                            .IndexOf(parametro, StringComparison.OrdinalIgnoreCase) >= 0 ||

                        (x.cargo ?? "")
                            .IndexOf(parametro, StringComparison.OrdinalIgnoreCase) >= 0
                    ).ToList();
                }

                dgvListaEmpleados.DataSource = null;
                dgvListaEmpleados.DataSource = datos;

                configurarColumnas();

                dgvListaEmpleados.ClearSelection();

                id = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al listar empleados: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // CONFIGURAR Y OCULTAR COLUMNAS
        private void configurarColumnas()
        {
            foreach (DataGridViewColumn columna in dgvListaEmpleados.Columns)
            {
                string nombre = columna.Name.ToLower();
                string propiedad =
                    (columna.DataPropertyName ?? "").ToLower();

                string encabezado =
                    (columna.HeaderText ?? "").ToLower();

                // OCULTAR COLUMNAS INTERNAS
                if (nombre == "id" || propiedad == "id" ||
                    nombre == "estado" || propiedad == "estado" ||
                    nombre == "usuarioregistro" ||
                    propiedad == "usuarioregistro" ||
                    nombre == "fecharegistro" ||
                    propiedad == "fecharegistro" ||
                    encabezado == "usuario" ||
                    encabezado == "usuario registro")
                {
                    columna.Visible = false;
                }
            }

            // CAMBIAR ENCABEZADOS
            cambiarEncabezado("cedulaIdentidad", "Cédula");
            cambiarEncabezado("nombres", "Nombres");
            cambiarEncabezado("primerApellido", "Primer Apellido");
            cambiarEncabezado("segundoApellido", "Segundo Apellido");
            cambiarEncabezado("fechaNacimiento", "Fecha de Nacimiento");
            cambiarEncabezado("direccion", "Dirección");
            cambiarEncabezado("celular", "Celular");
            cambiarEncabezado("cargo", "Cargo");
        }

        // CAMBIAR ENCABEZADOS
        private void cambiarEncabezado(string nombre, string encabezado)
        {
            foreach (DataGridViewColumn columna in dgvListaEmpleados.Columns)
            {
                if (columna.Name.Equals(
                        nombre, StringComparison.OrdinalIgnoreCase) ||
                    (columna.DataPropertyName ?? "").Equals(
                        nombre, StringComparison.OrdinalIgnoreCase))
                {
                    columna.HeaderText = encabezado;
                    break;
                }
            }
        }

        // CONFIGURACIÓN DESPUÉS DE CARGAR LOS DATOS
        private void dgvListaEmpleados_DataBindingComplete(
            object sender, DataGridViewBindingCompleteEventArgs e)
        {
            configurarColumnas();
            dgvListaEmpleados.ClearSelection();
        }

        // SELECCIONAR EMPLEADO
        private void dgvListaEmpleados_CellClick(
            object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var empleado =
                dgvListaEmpleados.Rows[e.RowIndex].DataBoundItem as Empleado;

            if (empleado != null)
                id = empleado.id;
        }

        // OBTENER EMPLEADO SELECCIONADO
        private Empleado obtenerSeleccionado()
        {
            if (dgvListaEmpleados.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione un empleado.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return null;
            }

            return dgvListaEmpleados.SelectedRows[0].DataBoundItem
                as Empleado;
        }

        // LIMPIAR FORMULARIO
        private void limpiarFormulario()
        {
            id = 0;

            txtCedula.Clear();
            txtNombres.Clear();
            txtPrimerApellido.Clear();
            txtSegundoApellido.Clear();
            txtDireccion.Clear();
            txtCelular.Clear();
            txtCargo.Clear();

            dtpFechaNacimiento.Value =
                DateTime.Today.AddYears(-18);

            dgvListaEmpleados.ClearSelection();
        }

        // RESTABLECER ESTADO
        private void restablecerEstado()
        {
            modoEdicion = false;

            dgvListaEmpleados.Enabled = true;
            pnlRegistro.Enabled = true;

            btnNuevo.Enabled = true;
            btnModificar.Enabled = true;
            btnEliminar.Enabled = true;

            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;

            pnlRegistro.Visible = false;

            Size = new Size(ANCHO, ALTO_INICIAL);
        }

        // NUEVO
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limpiarFormulario();

            modoEdicion = false;

            Size = new Size(ANCHO, ALTO_REGISTRO);

            dgvListaEmpleados.Enabled = false;

            btnNuevo.Enabled = false;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;

            pnlRegistro.Visible = true;
            pnlRegistro.Enabled = true;
            pnlRegistro.BringToFront();

            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

            txtCedula.Enabled = true;
            txtNombres.Enabled = true;
            txtPrimerApellido.Enabled = true;
            txtSegundoApellido.Enabled = true;
            dtpFechaNacimiento.Enabled = true;
            txtDireccion.Enabled = true;
            txtCelular.Enabled = true;
            txtCargo.Enabled = true;

            txtCedula.Focus();
        }

        // GUARDAR
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCedula.Text) ||
                string.IsNullOrWhiteSpace(txtNombres.Text) ||
                string.IsNullOrWhiteSpace(txtDireccion.Text) ||
                string.IsNullOrWhiteSpace(txtCelular.Text) ||
                string.IsNullOrWhiteSpace(txtCargo.Text))
            {
                MessageBox.Show("Complete los campos obligatorios.");
                return;
            }

            if (!long.TryParse(txtCelular.Text.Trim(), out long celular))
            {
                MessageBox.Show("Ingrese un número de celular válido.");
                txtCelular.Focus();
                return;
            }

            if (txtCedula.Text.Trim().Length > 12)
            {
                MessageBox.Show(
                    "La cédula no puede superar los 12 caracteres.");
                txtCedula.Focus();
                return;
            }

            if (dtpFechaNacimiento.Value.Date >= DateTime.Today)
            {
                MessageBox.Show(
                    "La fecha de nacimiento debe ser anterior a hoy.");
                dtpFechaNacimiento.Focus();
                return;
            }

            Empleado empleado = new Empleado();

            empleado.id = id;
            empleado.cedulaIdentidad = txtCedula.Text.Trim();
            empleado.nombres = txtNombres.Text.Trim();

            empleado.primerApellido =
                string.IsNullOrWhiteSpace(txtPrimerApellido.Text)
                ? null : txtPrimerApellido.Text.Trim();

            empleado.segundoApellido =
                string.IsNullOrWhiteSpace(txtSegundoApellido.Text)
                ? null : txtSegundoApellido.Text.Trim();

            empleado.fechaNacimiento =
                dtpFechaNacimiento.Value.Date;

            empleado.direccion = txtDireccion.Text.Trim();
            empleado.celular = celular;
            empleado.cargo = txtCargo.Text.Trim();

            string usuarioRegistro =
                CpRodaPro.Util.usuario?.usuario1 ?? "admin";

            bool estabaEditando = modoEdicion;

            try
            {
                if (modoEdicion)
                {
                    EmpleadoCln.actualizar(empleado, usuarioRegistro);

                    MessageBox.Show(
                        "Empleado modificado correctamente.",
                        "Confirmación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    EmpleadoCln.insertar(empleado, usuarioRegistro);

                    MessageBox.Show(
                        "Empleado registrado correctamente.",
                        "Confirmación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                listar();
                limpiarFormulario();
                restablecerEstado();
            }
            catch (Exception ex)
            {
                string operacion = estabaEditando
                    ? "modificar"
                    : "registrar";

                MessageBox.Show(
                    "Error al " + operacion +
                    " el empleado: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // MODIFICAR
        private void btnModificar_Click(object sender, EventArgs e)
        {
            Empleado empleado = obtenerSeleccionado();

            if (empleado == null)
                return;

            id = empleado.id;
            modoEdicion = true;

            Size = new Size(ANCHO, ALTO_REGISTRO);

            dgvListaEmpleados.Enabled = false;

            pnlRegistro.Visible = true;
            pnlRegistro.Enabled = true;
            pnlRegistro.BringToFront();

            txtCedula.Text = empleado.cedulaIdentidad;
            txtNombres.Text = empleado.nombres;
            txtPrimerApellido.Text = empleado.primerApellido;
            txtSegundoApellido.Text = empleado.segundoApellido;

            dtpFechaNacimiento.Value =
                empleado.fechaNacimiento;

            txtDireccion.Text = empleado.direccion;
            txtCelular.Text = empleado.celular.ToString();
            txtCargo.Text = empleado.cargo;

            txtCedula.Enabled = true;
            txtNombres.Enabled = true;
            txtPrimerApellido.Enabled = true;
            txtSegundoApellido.Enabled = true;
            dtpFechaNacimiento.Enabled = true;
            txtDireccion.Enabled = true;
            txtCelular.Enabled = true;
            txtCargo.Enabled = true;

            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

            txtCedula.Focus();
        }

        // ELIMINAR
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            Empleado empleado = obtenerSeleccionado();

            if (empleado == null)
                return;

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar al empleado " +
                empleado.nombres + " " + empleado.primerApellido + "?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                string usuarioRegistro =
                    CpRodaPro.Util.usuario?.usuario1 ?? "admin";

                int resultado =
                    EmpleadoCln.eliminar(empleado.id, usuarioRegistro);

                if (resultado == 0)
                {
                    MessageBox.Show(
                        "No se encontró el empleado.",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                MessageBox.Show(
                    "Empleado eliminado correctamente.",
                    "Confirmación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                listar();
                limpiarFormulario();
                restablecerEstado();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al eliminar el empleado: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // BUSCAR
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

        // CANCELAR
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            limpiarFormulario();
            restablecerEstado();
        }

        // VOLVER
        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}