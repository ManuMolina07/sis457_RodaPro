using CadRodapro;
using ClnRodaPro;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CpRodapro
{
    public partial class FrmProducto : Form
    {
        private int id = 0;
        private bool modoEdicion = false;

        private const int ANCHO = 900;
        private const int ALTO_INICIAL = 338;
        private const int ALTO_REGISTRO = 600;

        public FrmProducto()
        {
            InitializeComponent();
        }

        private void FrmProducto_Load(object sender, EventArgs e)
        {
            cbxCategoria.DataSource = CategoriaCln.listar();
            cbxCategoria.DisplayMember = "nombre";
            cbxCategoria.ValueMember = "id";
            cbxCategoria.SelectedIndex = -1;

            cbxUnidad.DataSource = UnidadMedidaCln.listar();
            cbxUnidad.DisplayMember = "descripcion";
            cbxUnidad.ValueMember = "id";
            cbxUnidad.SelectedIndex = -1;

            cargarLista();
            limpiarFormulario();

            Size = new Size(ANCHO, ALTO_INICIAL);
            restablecerEstado();
        }

        private void cargarLista(string parametro = "")
        {
            dgvLista.DataSource = ProductoCln.listarPa(parametro);

            if (dgvLista.Columns.Contains("id"))
                dgvLista.Columns["id"].Visible = false;

            if (dgvLista.Columns.Contains("idCategoria"))
                dgvLista.Columns["idCategoria"].Visible = false;

            if (dgvLista.Columns.Contains("idUnidadMedida"))
                dgvLista.Columns["idUnidadMedida"].Visible = false;

            if (dgvLista.Columns.Contains("estado"))
                dgvLista.Columns["estado"].Visible = false;

            dgvLista.ClearSelection();
        }

        private void limpiarFormulario()
        {
            id = 0;

            txtCodigo.Clear();
            txtNombre.Clear();
            txtDescripcion.Clear();
            txtMarca.Clear();
            txtPrecioCompra.Clear();
            txtPrecioVenta.Clear();
            txtStock.Clear();
            txtStockMinimo.Clear();

            cbxCategoria.SelectedIndex = -1;
            cbxUnidad.SelectedIndex = -1;

            dgvLista.ClearSelection();

            txtCodigo.Focus();
        }

        private void restablecerEstado()
        {
            modoEdicion = false;

            dgvLista.Enabled = true;

            btnNuevo.Enabled = true;
            btnModificar.Enabled = true;
            btnEliminar.Enabled = true;

            pnlRegistro.Enabled = true;
        }

        private void asignarDatos(Producto producto)
        {
            producto.idCategoria = Convert.ToInt32(cbxCategoria.SelectedValue);
            producto.idUnidadMedida = Convert.ToInt32(cbxUnidad.SelectedValue);

            producto.codigo = txtCodigo.Text.Trim();
            producto.nombre = txtNombre.Text.Trim();
            producto.descripcion = txtDescripcion.Text.Trim();
            producto.marca = txtMarca.Text.Trim();

            producto.precioCompra = Convert.ToDecimal(txtPrecioCompra.Text);
            producto.precioVenta = Convert.ToDecimal(txtPrecioVenta.Text);
            producto.stock = Convert.ToDecimal(txtStock.Text);
            producto.stockMinimo = Convert.ToDecimal(txtStockMinimo.Text);

            producto.usuarioRegistro = "admin";
        }

        private bool validarDatos()
        {
            if (string.IsNullOrWhiteSpace(txtCodigo.Text))
            {
                MessageBox.Show("Ingrese el código del producto.");
                txtCodigo.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Ingrese el nombre del producto.");
                txtNombre.Focus();
                return false;
            }

            if (cbxCategoria.SelectedValue == null)
            {
                MessageBox.Show("Seleccione una categoría.");
                cbxCategoria.Focus();
                return false;
            }

            if (cbxUnidad.SelectedValue == null)
            {
                MessageBox.Show("Seleccione una unidad de medida.");
                cbxUnidad.Focus();
                return false;
            }

            decimal precioCompra;
            if (!decimal.TryParse(txtPrecioCompra.Text, out precioCompra)
                || precioCompra <= 0)
            {
                MessageBox.Show("Ingrese un precio de compra válido y mayor a cero.");
                txtPrecioCompra.Focus();
                return false;
            }

            decimal precioVenta;
            if (!decimal.TryParse(txtPrecioVenta.Text, out precioVenta)
                || precioVenta <= 0)
            {
                MessageBox.Show("Ingrese un precio de venta válido y mayor a cero.");
                txtPrecioVenta.Focus();
                return false;
            }

            decimal stock;
            if (!decimal.TryParse(txtStock.Text, out stock) || stock < 0)
            {
                MessageBox.Show("Ingrese un stock válido.");
                txtStock.Focus();
                return false;
            }

            decimal stockMinimo;
            if (!decimal.TryParse(txtStockMinimo.Text, out stockMinimo)
                || stockMinimo < 0)
            {
                MessageBox.Show("Ingrese un stock mínimo válido.");
                txtStockMinimo.Focus();
                return false;
            }

            return true;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            restablecerEstado();
            limpiarFormulario();

            modoEdicion = false;

            Size = new Size(900, 600);

            // Habilitar los campos
            txtCodigo.Enabled = true;
            txtNombre.Enabled = true;
            txtDescripcion.Enabled = true;
            txtMarca.Enabled = true;
            txtPrecioCompra.Enabled = true;
            txtPrecioVenta.Enabled = true;
            txtStock.Enabled = true;
            txtStockMinimo.Enabled = true;

            cbxCategoria.Enabled = true;
            cbxUnidad.Enabled = true;

            txtCodigo.Focus();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!validarDatos())
                return;

            bool estabaEditando = modoEdicion;

            try
            {
                if (modoEdicion)
                {
                    Producto producto = ProductoCln.obtenerUno(id);

                    if (producto == null)
                    {
                        MessageBox.Show("No se encontró el producto.");
                        return;
                    }

                    asignarDatos(producto);

                    ProductoCln.actualizar(producto);

                    MessageBox.Show("Producto modificado correctamente.");
                }
                else
                {
                    Producto producto = new Producto();

                    asignarDatos(producto);

                    producto.fechaRegistro = DateTime.Now;
                    producto.estado = (short)Estado.Activo;

                    ProductoCln.crear(producto);

                    MessageBox.Show("Producto registrado correctamente.");
                }

                cargarLista();

                restablecerEstado();
                limpiarFormulario();

                Size = new Size(ANCHO, ALTO_INICIAL);
                pnlRegistro.Enabled = true;
            }
            catch (Exception ex)
            {
                string operacion = estabaEditando
                    ? "modificar"
                    : "registrar";

                MessageBox.Show(
                    "Error al " + operacion + " el producto: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                MessageBox.Show("Primero debes seleccionar un producto.");
                return;
            }

            modoEdicion = true;

            Size = new Size(900, 600);

            // Deshabilitar la lista y las acciones de selección
            dgvLista.Enabled = false;

            // Habilitar los campos del formulario
            txtCodigo.Enabled = true;
            txtNombre.Enabled = true;
            txtDescripcion.Enabled = true;
            txtMarca.Enabled = true;
            txtPrecioCompra.Enabled = true;
            txtPrecioVenta.Enabled = true;
            txtStock.Enabled = true;
            txtStockMinimo.Enabled = true;

            cbxCategoria.Enabled = true;
            cbxUnidad.Enabled = true;

            txtCodigo.Focus();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                MessageBox.Show("Seleccione un producto de la lista.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar este producto?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta == DialogResult.Yes)
            {
                try
                {
                    ProductoCln.eliminar(id, "admin");

                    MessageBox.Show("Producto eliminado correctamente.");

                    cargarLista();
                    limpiarFormulario();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el producto: " + ex.Message);
                }
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            restablecerEstado();
            limpiarFormulario();

            Size = new Size(900, 338);

            pnlRegistro.Enabled = true;
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            cargarLista(txtParametro.Text.Trim());
        }

        private void txtParametro_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnBuscar.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        private void dgvLista_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || modoEdicion)
                return;

            DataGridViewRow fila = dgvLista.Rows[e.RowIndex];

            id = Convert.ToInt32(fila.Cells["id"].Value);

            txtCodigo.Text = Convert.ToString(fila.Cells["codigo"].Value);
            txtNombre.Text = Convert.ToString(fila.Cells["nombre"].Value);
            txtDescripcion.Text = Convert.ToString(fila.Cells["descripcion"].Value);
            txtMarca.Text = Convert.ToString(fila.Cells["marca"].Value);

            cbxCategoria.SelectedValue =
                Convert.ToInt32(fila.Cells["idCategoria"].Value);

            cbxUnidad.SelectedValue =
                Convert.ToInt32(fila.Cells["idUnidadMedida"].Value);

            txtPrecioCompra.Text =
                Convert.ToString(fila.Cells["precioCompra"].Value);

            txtPrecioVenta.Text =
                Convert.ToString(fila.Cells["precioVenta"].Value);

            txtStock.Text =
                Convert.ToString(fila.Cells["stock"].Value);

            txtStockMinimo.Text =
                Convert.ToString(fila.Cells["stockMinimo"].Value);
        }

        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
