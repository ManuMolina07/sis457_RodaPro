using ClnRodaPro;
using CadRodapro;
using System;
using System.Linq;
using System.Windows.Forms;

namespace CpRodapro
{
    public partial class FrmEmpleados : Form
    {
        private int id = 0;
        private bool modoEdicion = false;

        public FrmEmpleados()
        {
            InitializeComponent();

            Load += FrmEmpleados_Load;

            btnNuevo.Click += btnNuevo_Click;
            btnGuardar.Click += btnGuardar_Click;
            btnModificar.Click += btnModificar_Click;
            btnEliminar.Click += btnEliminar_Click;
            btnCancelar.Click += btnCancelar_Click;
            btnVolver.Click += btnVolver_Click;
            btnBuscar.Click += btnBuscar_Click;

            dgvListaEmpleados.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvListaEmpleados.MultiSelect = false;
            dgvListaEmpleados.ReadOnly = true;
            dgvListaEmpleados.AllowUserToAddRows = false;
            dgvListaEmpleados.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvListaEmpleados.DataBindingComplete +=
                dgvListaEmpleados_DataBindingComplete;
        }

        private void FrmEmpleados_Load(object sender, EventArgs e)
        {
            listar();
            restablecerEstado();
        }

        // LISTAR EMPLEADOS
        private void listar()
        {
            string parametro = txtParametro.Text.Trim().ToLower();

            var datos = EmpleadoCln.listar();

            if (!string.IsNullOrEmpty(parametro))
            {
                datos = datos.Where(x =>
                    (x.cedulaIdentidad ?? "").ToLower().Contains(parametro) ||
                    (x.nombres ?? "").ToLower().Contains(parametro) ||
                    (x.primerApellido ?? "").ToLower().Contains(parametro) ||
                    (x.segundoApellido ?? "").ToLower().Contains(parametro) ||
                    (x.cargo ?? "").ToLower().Contains(parametro)
                ).ToList();
            }

            dgvListaEmpleados.DataSource = null;
            dgvListaEmpleados.DataSource = datos;

            configurarColumnas();
        }

        // CONFIGURAR Y OCULTAR COLUMNAS
        private void configurarColumnas()
        {
            foreach (DataGridViewColumn columna in dgvListaEmpleados.Columns)
            {
                string nombre = columna.Name.ToLower();
                string propiedad = (columna.DataPropertyName ?? "").ToLower();
                string encabezado = (columna.HeaderText ?? "").ToLower();

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

        private void cambiarEncabezado(string nombre, string encabezado)
        {
            foreach (DataGridViewColumn columna in dgvListaEmpleados.Columns)
            {
                if (columna.Name.Equals(nombre,
                    StringComparison.OrdinalIgnoreCase) ||
                    columna.DataPropertyName.Equals(nombre,
                    StringComparison.OrdinalIgnoreCase))
                {
                    columna.HeaderText = encabezado;
                    break;
                }
            }
        }

        // APLICAR CONFIGURACIÓN DESPUÉS DE CARGAR LOS DATOS
        private void dgvListaEmpleados_DataBindingComplete(
            object sender, DataGridViewBindingCompleteEventArgs e)
        {
            configurarColumnas();
        }

        // NUEVO
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            restablecerEstado();
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
                return;
            }

            if (txtCedula.Text.Trim().Length > 12)
            {
                MessageBox.Show("La cédula no puede superar los 12 caracteres.");
                return;
            }

            if (dtpFechaNacimiento.Value.Date >= DateTime.Today)
            {
                MessageBox.Show("La fecha de nacimiento debe ser anterior a hoy.");
                return;
            }

            var empleado = new Empleado();

            empleado.id = id;
            empleado.cedulaIdentidad = txtCedula.Text.Trim();
            empleado.nombres = txtNombres.Text.Trim();

            empleado.primerApellido =
                string.IsNullOrWhiteSpace(txtPrimerApellido.Text)
                ? null : txtPrimerApellido.Text.Trim();

            empleado.segundoApellido =
                string.IsNullOrWhiteSpace(txtSegundoApellido.Text)
                ? null : txtSegundoApellido.Text.Trim();

            empleado.fechaNacimiento = dtpFechaNacimiento.Value.Date;
            empleado.direccion = txtDireccion.Text.Trim();
            empleado.celular = celular;
            empleado.cargo = txtCargo.Text.Trim();

            string usuarioRegistro =
                CpRodaPro.Util.usuario?.usuario1 ?? "admin";

            try
            {
                if (modoEdicion)
                {
                    EmpleadoCln.actualizar(empleado, usuarioRegistro);
                    MessageBox.Show("Empleado modificado correctamente.");
                }
                else
                {
                    EmpleadoCln.insertar(empleado, usuarioRegistro);
                    MessageBox.Show("Empleado registrado correctamente.");
                }

                listar();
                restablecerEstado();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el empleado: " + ex.Message);
            }
        }

        // MODIFICAR
        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvListaEmpleados.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un empleado.");
                return;
            }

            var empleado =
                dgvListaEmpleados.CurrentRow.DataBoundItem as Empleado;

            if (empleado == null)
                return;

            id = empleado.id;
            modoEdicion = true;

            txtCedula.Text = empleado.cedulaIdentidad;
            txtNombres.Text = empleado.nombres;
            txtPrimerApellido.Text = empleado.primerApellido;
            txtSegundoApellido.Text = empleado.segundoApellido;
            dtpFechaNacimiento.Value = empleado.fechaNacimiento;
            txtDireccion.Text = empleado.direccion;
            txtCelular.Text = empleado.celular.ToString();
            txtCargo.Text = empleado.cargo;

            btnGuardar.Enabled = true;
        }

        // ELIMINAR
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvListaEmpleados.CurrentRow == null)
            {
                MessageBox.Show("Seleccione un empleado.");
                return;
            }

            var empleado =
                dgvListaEmpleados.CurrentRow.DataBoundItem as Empleado;

            if (empleado == null)
                return;

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar este empleado?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta == DialogResult.Yes)
            {
                string usuarioRegistro =
                    CpRodaPro.Util.usuario?.usuario1 ?? "admin";

                try
                {
                    EmpleadoCln.eliminar(empleado.id, usuarioRegistro);

                    MessageBox.Show("Empleado eliminado correctamente.");

                    listar();
                    restablecerEstado();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar: " + ex.Message);
                }
            }
        }

        // BUSCAR
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            listar();
        }

        // CANCELAR
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            restablecerEstado();
        }

        // VOLVER
        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // RESTABLECER FORMULARIO
        private void restablecerEstado()
        {
            id = 0;
            modoEdicion = false;

            txtCedula.Clear();
            txtNombres.Clear();
            txtPrimerApellido.Clear();
            txtSegundoApellido.Clear();
            txtDireccion.Clear();
            txtCelular.Clear();
            txtCargo.Clear();

            txtCedula.Enabled = true;
            txtNombres.Enabled = true;
            btnGuardar.Enabled = true;

            dtpFechaNacimiento.Value =
                DateTime.Today.AddYears(-18);

            txtCedula.Focus();
        }
    }
}
