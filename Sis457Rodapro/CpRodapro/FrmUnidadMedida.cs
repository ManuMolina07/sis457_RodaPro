using CadRodapro;
using ClnRodaPro;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CpRodapro
{
    public partial class FrmUnidadMedida : Form
    {
        private int id = 0;
        private bool modoEdicion = false;

        private const int ANCHO = 900;
        private const int ALTO_INICIAL = 357;
        private const int ALTO_REGISTRO = 600;

        public FrmUnidadMedida()
        {
            InitializeComponent();

            Load += FrmUnidadMedida_Load;

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
        }

        // CARGAR FORMULARIO
        private void FrmUnidadMedida_Load(object sender, EventArgs e)
        {
            Size = new Size(ANCHO, ALTO_INICIAL);

            listar();
            restablecerEstado();
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

        // RESTABLECER ESTADO INICIAL
        private void restablecerEstado()
        {
            pnlRegistro.Visible = false;

            Size = new Size(ANCHO, ALTO_INICIAL);

            txtDescripcion.Clear();

            id = 0;
            modoEdicion = false;

            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;

            dgvLista.ClearSelection();
        }

        // LISTAR UNIDADES DE MEDIDA
        private void listar()
        {
            try
            {
                string parametro = txtParametro.Text.Trim();

                var lista = UnidadMedidaCln.listar();

                if (!string.IsNullOrEmpty(parametro))
                {
                    lista = lista
                        .Where(x => x.descripcion != null &&
                            x.descripcion.IndexOf(
                                parametro,
                                StringComparison.OrdinalIgnoreCase) >= 0)
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

                dgvLista.Columns["id"].Visible = false;
                dgvLista.Columns["estado"].Visible = false;
                dgvLista.Columns["Producto"].Visible = false;

                dgvLista.Columns["descripcion"].HeaderText = "Descripción";

                dgvLista.Columns["usuarioRegistro"].HeaderText =
                    "Registrado por";

                dgvLista.Columns["fechaRegistro"].HeaderText =
                    "Fecha de registro";

                dgvLista.Columns["fechaRegistro"].DefaultCellStyle.Format =
                    "dd/MM/yyyy HH:mm";

                dgvLista.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al listar las unidades de medida: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // OBTENER REGISTRO SELECCIONADO
        private UnidadMedida obtenerSeleccionado()
        {
            if (dgvLista.CurrentRow == null ||
                dgvLista.CurrentRow.DataBoundItem == null)
            {
                MessageBox.Show(
                    "Seleccione una unidad de medida.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return null;
            }

            return dgvLista.CurrentRow.DataBoundItem as UnidadMedida;
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
                listar();
            }
        }

        // BOTÓN NUEVO
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            restablecerEstado();

            habilitarRegistro();

            txtDescripcion.Focus();
        }

        // BOTÓN MODIFICAR
        private void btnModificar_Click(object sender, EventArgs e)
        {
            UnidadMedida unidad = obtenerSeleccionado();

            if (unidad == null)
                return;

            id = unidad.id;
            modoEdicion = true;

            txtDescripcion.Text = unidad.descripcion;

            habilitarRegistro();

            txtDescripcion.Focus();
        }

        // BOTÓN GUARDAR
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string descripcion = txtDescripcion.Text.Trim();

                // VALIDAR DESCRIPCIÓN
                if (string.IsNullOrWhiteSpace(descripcion))
                {
                    MessageBox.Show(
                        "Ingrese la descripción de la unidad de medida.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtDescripcion.Focus();
                    return;
                }

                if (descripcion.Length > 25)
                {
                    MessageBox.Show(
                        "La descripción no puede superar los 25 caracteres.",
                        "Validación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtDescripcion.Focus();
                    return;
                }

                // INSERTAR
                if (!modoEdicion)
                {
                    if (CpRodaPro.Util.usuario == null)
                    {
                        MessageBox.Show(
                            "No se pudo identificar al usuario autenticado.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        return;
                    }

                    UnidadMedida unidad = new UnidadMedida
                    {
                        descripcion = descripcion,
                        usuarioRegistro =
                            CpRodaPro.Util.usuario.usuario1
                    };

                    UnidadMedidaCln.insertar(unidad);

                    MessageBox.Show(
                        "La unidad de medida fue registrada correctamente.",
                        "Confirmación",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                // ACTUALIZAR
                else
                {
                    UnidadMedida unidad = new UnidadMedida
                    {
                        id = id,
                        descripcion = descripcion
                    };

                    UnidadMedidaCln.actualizar(unidad);

                    MessageBox.Show(
                        "La unidad de medida fue modificada correctamente.",
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
                    "Error al guardar la unidad de medida:\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // BOTÓN ELIMINAR
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            UnidadMedida unidad = obtenerSeleccionado();

            if (unidad == null)
                return;

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar la unidad de medida " +
                unidad.descripcion + "?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                UnidadMedidaCln.eliminar(unidad.id);

                MessageBox.Show(
                    "La unidad de medida fue eliminada correctamente.",
                    "Confirmación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                listar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al eliminar la unidad de medida: " + ex.Message,
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
