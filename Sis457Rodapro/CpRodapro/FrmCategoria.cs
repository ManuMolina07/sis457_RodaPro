using CadRodapro;
using ClnRodaPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CpRodapro
{
    public partial class FrmCategoria : Form
    {
        private int idCategoria = 0;
        private bool modoEdicion = false;

        private List<Categoria> listaCategorias =
            new List<Categoria>();

        public FrmCategoria()
        {
            InitializeComponent();

            // Configuración inicial
            pnlRegistro.Visible = false;
            this.Size = new System.Drawing.Size(900, 357);

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

            pnlRegistro.Visible = false;

            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;

            this.Size = new System.Drawing.Size(900, 357);
        }

        // Cargar formulario
        private void FrmCategoria_Load(object sender, EventArgs e)
        {
            configurarGrid();
            listar();
            limpiar();
        }

        // Configurar DataGridView
        private void configurarGrid()
        {
            dgvLista.AutoGenerateColumns = false;
            dgvLista.AllowUserToAddRows = false;
            dgvLista.ReadOnly = true;
            dgvLista.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;
            dgvLista.MultiSelect = false;
            dgvLista.Columns.Clear();

            dgvLista.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "id",
                DataPropertyName = "id",
                HeaderText = "ID",
                Visible = false
            });

            dgvLista.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "nombre",
                DataPropertyName = "nombre",
                HeaderText = "Nombre",
                Width = 200
            });

            dgvLista.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "descripcion",
                DataPropertyName = "descripcion",
                HeaderText = "Descripción",
                AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.Fill
            });
        }

        // Listar categorías
        private void listar()
        {
            try
            {
                listaCategorias = CategoriaCln.listar();

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
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al listar categorías: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Limpiar controles
        private void limpiar()
        {
            idCategoria = 0;
            modoEdicion = false;

            txtNombre.Clear();
            txtDescripcion.Clear();

            dgvLista.ClearSelection();

            btnGuardar.Text = "Guardar";

            txtNombre.Focus();
        }

        // Nuevo
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            idCategoria = 0;
            modoEdicion = false;

            txtNombre.Clear();
            txtDescripcion.Clear();

            btnGuardar.Text = "Guardar";

            habilitarRegistro();
        }

        // Guardar o actualizar
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = txtNombre.Text.Trim();
                string descripcion = txtDescripcion.Text.Trim();

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

                if (CpRodaPro.Util.usuario == null)
                {
                    MessageBox.Show("No existe un usuario autenticado.");
                    return;
                }

                string usuario =
                    CpRodaPro.Util.usuario.usuario1;

                if (modoEdicion)
                {
                    Categoria categoria = new Categoria
                    {
                        id = idCategoria,
                        nombre = nombre,
                        descripcion = descripcion,
                        usuarioRegistro = usuario
                    };

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
                    Categoria categoria = new Categoria
                    {
                        nombre = nombre,
                        descripcion = descripcion,
                        usuarioRegistro = usuario,
                        estado = 1
                    };

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
                    "Error al guardar la categoría:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // Seleccionar categoría para modificar
        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (dgvLista.SelectedRows.Count == 0 ||
                dgvLista.SelectedRows[0].DataBoundItem == null)
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
                (Categoria)dgvLista.SelectedRows[0].DataBoundItem;

            idCategoria = categoria.id;
            txtNombre.Text = categoria.nombre;
            txtDescripcion.Text = categoria.descripcion;

            modoEdicion = true;
            btnGuardar.Text = "Actualizar";

            habilitarRegistro();
        }

        // Eliminación lógica
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvLista.SelectedRows.Count == 0 ||
                dgvLista.SelectedRows[0].DataBoundItem == null)
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
                (Categoria)dgvLista.SelectedRows[0].DataBoundItem;

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar la categoría " +
                categoria.nombre + "?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta == DialogResult.Yes)
            {
                try
                {
                    if (CpRodaPro.Util.usuario == null)
                    {
                        MessageBox.Show("No existe un usuario autenticado.");
                        return;
                    }

                    CategoriaCln.eliminar(
                        categoria.id,
                        CpRodaPro.Util.usuario.usuario1
                    );

                    MessageBox.Show(
                        "Categoría eliminada correctamente.",
                        "RodaPro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    listar();
                    limpiar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error al eliminar la categoría:\n" + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        // Buscar
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            listar();
        }

        // Cancelar
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            restablecerEstado();
        }

        // Volver
        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Mostrar panel de registro
        private void habilitarRegistro()
        {
            pnlRegistro.Visible = true;
            this.Size = new System.Drawing.Size(900, 600);

            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

            pnlRegistro.BringToFront();
            txtNombre.Focus();
        }

        // Ocultar panel y restablecer formulario
        private void restablecerEstado()
        {
            pnlRegistro.Visible = false;

            this.Size = new System.Drawing.Size(900, 357);

            idCategoria = 0;
            modoEdicion = false;

            txtNombre.Clear();
            txtDescripcion.Clear();

            btnGuardar.Text = "Guardar";

            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;

            dgvLista.ClearSelection();
        }
    }
}

