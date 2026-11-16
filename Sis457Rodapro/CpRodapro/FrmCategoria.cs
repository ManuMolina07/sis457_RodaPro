
using CadRodapro;
using ClnRodaPro;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CpRodapro
{
    public partial class FrmCategoria : Form
    {
        // CONSTANTES
        private const int ANCHO_FORMULARIO = 900;
        private const int ALTO_INICIAL = 357;
        private const int ALTO_REGISTRO = 600;
        private const int ESTADO_ACTIVO = 1;

        // VARIABLES
        private int idCategoria = 0;
        private bool modoEdicion = false;

        public FrmCategoria()
        {
            InitializeComponent();

            // CONFIGURACIÓN INICIAL
            pnlRegistro.Visible = false;
            Size = new Size(ANCHO_FORMULARIO, ALTO_INICIAL);

            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;

            // EVENTOS
            Load -= FrmCategoria_Load;
            Load += FrmCategoria_Load;

            btnNuevo.Click -= btnNuevo_Click;
            btnNuevo.Click += btnNuevo_Click;

            btnGuardar.Click -= btnGuardar_Click;
            btnGuardar.Click += btnGuardar_Click;

            btnModificar.Click -= btnModificar_Click;
            btnModificar.Click += btnModificar_Click;

            btnEliminar.Click -= btnEliminar_Click;
            btnEliminar.Click += btnEliminar_Click;

            btnBuscar.Click -= btnBuscar_Click;
            btnBuscar.Click += btnBuscar_Click;

            btnCancelar.Click -= btnCancelar_Click;
            btnCancelar.Click += btnCancelar_Click;

            btnVolver.Click -= btnVolver_Click;
            btnVolver.Click += btnVolver_Click;

            txtParametro.KeyDown -= txtParametro_KeyDown;
            txtParametro.KeyDown += txtParametro_KeyDown;
        }

        // CARGAR FORMULARIO
        private void FrmCategoria_Load(object sender, EventArgs e)
        {
            configurarGrid();
            listar();
            limpiarFormulario();
        }

        // CONFIGURAR DATAGRIDVIEW
        private void configurarGrid()
        {
            dgvLista.AutoGenerateColumns = false;
            dgvLista.AllowUserToAddRows = false;
            dgvLista.ReadOnly = true;

            dgvLista.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvLista.MultiSelect = false;
            dgvLista.Columns.Clear();

            // ID
            dgvLista.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "id",
                DataPropertyName = "id",
                HeaderText = "ID",
                Visible = false
            });

            // NOMBRE
            dgvLista.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "nombre",
                DataPropertyName = "nombre",
                HeaderText = "Nombre",
                Width = 200
            });

            // DESCRIPCIÓN
            dgvLista.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "descripcion",
                DataPropertyName = "descripcion",
                HeaderText = "Descripción",
                AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.Fill
            });
        }

        // LISTAR Y BUSCAR CATEGORÍAS
        private void listar()
        {
            try
            {
                var listaCategorias = CategoriaCln.listar();

                string parametro = txtParametro.Text.Trim();

                var resultado = listaCategorias
                    .Where(x =>
                        string.IsNullOrEmpty(parametro) ||
                        (x.nombre ?? "").IndexOf(
                            parametro,
                            StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (x.descripcion ?? "").IndexOf(
                            parametro,
                            StringComparison.OrdinalIgnoreCase) >= 0
                    )
                    .ToList();

                dgvLista.DataSource = null;
                dgvLista.DataSource = resultado;

                dgvLista.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al listar categorías:\n" +
                    obtenerDetalleError(ex),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // LIMPIAR FORMULARIO
        private void limpiarFormulario()
        {
            idCategoria = 0;
            modoEdicion = false;

            txtNombre.Clear();
            txtDescripcion.Clear();

            btnGuardar.Text = "Guardar";

            dgvLista.ClearSelection();
        }

        // VALIDAR USUARIO AUTENTICADO
        private bool validarUsuario(out string usuario)
        {
            usuario = null;

            if (CpRodaPro.Util.usuario == null)
            {
                MessageBox.Show(
                    "No existe un usuario autenticado.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return false;
            }

            usuario = CpRodaPro.Util.usuario.usuario1;

            return true;
        }

        // NUEVO
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limpiarFormulario();
            habilitarRegistro();
        }

        // GUARDAR O ACTUALIZAR
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string nombre = txtNombre.Text.Trim();
            string descripcion = txtDescripcion.Text.Trim();

            // VALIDAR NOMBRE
            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show(
                    "Ingrese el nombre de la categoría.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtNombre.Focus();
                return;
            }

            // VALIDAR USUARIO
            string usuario;

            if (!validarUsuario(out usuario))
                return;

            try
            {
                // CREAR OBJETO CATEGORÍA
                Categoria categoria = new Categoria
                {
                    id = idCategoria,
                    nombre = nombre,
                    descripcion = descripcion,
                    usuarioRegistro = usuario
                };

                if (modoEdicion)
                {
                    // ACTUALIZAR
                    CategoriaCln.actualizar(categoria);

                    MessageBox.Show(
                        "Categoría modificada correctamente.",
                        "RodaPro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    // REGISTRAR
                    categoria.estado = ESTADO_ACTIVO;

                    CategoriaCln.crear(categoria);

                    MessageBox.Show(
                        "Categoría registrada correctamente.",
                        "RodaPro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                listar();
                restablecerEstado();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al guardar la categoría:\n\n" +
                    obtenerDetalleError(ex),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // MODIFICAR
        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvLista.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione una categoría para modificar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            Categoria categoria =
                dgvLista.SelectedRows[0].DataBoundItem as Categoria;

            if (categoria == null)
            {
                MessageBox.Show(
                    "No se pudo obtener la categoría seleccionada.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            // CARGAR DATOS
            idCategoria = categoria.id;

            txtNombre.Text = categoria.nombre;
            txtDescripcion.Text = categoria.descripcion;

            modoEdicion = true;
            btnGuardar.Text = "Actualizar";

            habilitarRegistro();
        }

        // ELIMINACIÓN LÓGICA
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvLista.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione una categoría para eliminar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            Categoria categoria =
                dgvLista.SelectedRows[0].DataBoundItem as Categoria;

            if (categoria == null)
            {
                MessageBox.Show(
                    "No se pudo obtener la categoría seleccionada.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            // CONFIRMAR ELIMINACIÓN
            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar la categoría " +
                categoria.nombre + "?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta != DialogResult.Yes)
                return;

            // VALIDAR USUARIO
            string usuario;

            if (!validarUsuario(out usuario))
                return;

            try
            {
                CategoriaCln.eliminar(categoria.id, usuario);

                MessageBox.Show(
                    "Categoría eliminada correctamente.",
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                listar();
                restablecerEstado();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al eliminar la categoría:\n" +
                    obtenerDetalleError(ex),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
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
            Close();
        }

        // MOSTRAR PANEL DE REGISTRO
        private void habilitarRegistro()
        {
            pnlRegistro.Visible = true;

            Size = new Size(
                ANCHO_FORMULARIO,
                ALTO_REGISTRO
            );

            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

            pnlRegistro.BringToFront();

            txtNombre.Focus();
        }

        // RESTABLECER FORMULARIO
        private void restablecerEstado()
        {
            pnlRegistro.Visible = false;

            Size = new Size(
                ANCHO_FORMULARIO,
                ALTO_INICIAL
            );

            limpiarFormulario();

            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;
        }

        // OBTENER DETALLE REAL DEL ERROR
        private string obtenerDetalleError(Exception ex)
        {
            Exception error = ex;

            while (error.InnerException != null)
            {
                error = error.InnerException;
            }

            return error.Message;
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
    }
}

