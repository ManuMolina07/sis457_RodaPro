
using CadRodapro;
using ClnRodaPro;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CpRodapro
{
    public partial class FrmProducto : Form
    {
        // VARIABLES
        private int id = 0;
        private bool modoEdicion = false;

        // CONSTANTES
        private const int ANCHO = 900;
        private const int ALTO_INICIAL = 338;
        private const int ALTO_REGISTRO = 600;

        public FrmProducto()
        {
            InitializeComponent();
        }

        // CARGAR FORMULARIO
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
            restablecerEstado();
        }

        // CARGAR LISTA DE PRODUCTOS
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

        // LIMPIAR FORMULARIO
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
        }

        // RESTABLECER ESTADO
        private void restablecerEstado()
        {
            modoEdicion = false;

            Size = new Size(ANCHO, ALTO_INICIAL);

            dgvLista.Enabled = true;

            btnNuevo.Enabled = true;
            btnModificar.Enabled = true;
            btnEliminar.Enabled = true;

            btnGuardar.Enabled = false;
            btnCancelar.Enabled = false;

            pnlRegistro.Enabled = false;

            habilitarCampos(false);
        }

        // HABILITAR O DESHABILITAR CAMPOS
        private void habilitarCampos(bool habilitar)
        {
            txtCodigo.Enabled = habilitar;
            txtNombre.Enabled = habilitar;
            txtDescripcion.Enabled = habilitar;
            txtMarca.Enabled = habilitar;
            txtPrecioCompra.Enabled = habilitar;
            txtPrecioVenta.Enabled = habilitar;
            txtStock.Enabled = habilitar;
            txtStockMinimo.Enabled = habilitar;

            cbxCategoria.Enabled = habilitar;
            cbxUnidad.Enabled = habilitar;
        }

        // ASIGNAR DATOS DEL FORMULARIO AL PRODUCTO
        private void asignarDatos(Producto producto)
        {
            producto.idCategoria =
                Convert.ToInt32(cbxCategoria.SelectedValue);

            producto.idUnidadMedida =
                Convert.ToInt32(cbxUnidad.SelectedValue);

            producto.codigo = txtCodigo.Text.Trim();
            producto.nombre = txtNombre.Text.Trim();
            producto.descripcion = txtDescripcion.Text.Trim();
            producto.marca = txtMarca.Text.Trim();

            producto.precioCompra =
                Convert.ToDecimal(txtPrecioCompra.Text);

            producto.precioVenta =
                Convert.ToDecimal(txtPrecioVenta.Text);

            producto.stock =
                Convert.ToDecimal(txtStock.Text);

            producto.stockMinimo =
                Convert.ToDecimal(txtStockMinimo.Text);

            producto.usuarioRegistro =
                CpRodaPro.Util.usuario.usuario1;
        }

        // VALIDAR DATOS
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
                MessageBox.Show(
                    "Ingrese un precio de compra válido y mayor a cero."
                );

                txtPrecioCompra.Focus();
                return false;
            }

            decimal precioVenta;

            if (!decimal.TryParse(txtPrecioVenta.Text, out precioVenta)
                || precioVenta <= 0)
            {
                MessageBox.Show(
                    "Ingrese un precio de venta válido y mayor a cero."
                );

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

        // NUEVO PRODUCTO
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            limpiarFormulario();

            modoEdicion = false;

            Size = new Size(ANCHO, ALTO_REGISTRO);

            dgvLista.Enabled = false;

            btnNuevo.Enabled = false;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;

            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

            pnlRegistro.Enabled = true;

            habilitarCampos(true);

            txtCodigo.Focus();
        }

        // GUARDAR O ACTUALIZAR PRODUCTO
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!validarDatos())
                return;

            if (CpRodaPro.Util.usuario == null)
            {
                MessageBox.Show(
                    "No existe un usuario autenticado.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            bool estabaEditando = modoEdicion;

            try
            {
                if (modoEdicion)
                {
                    // ACTUALIZAR
                    Producto producto = ProductoCln.obtenerUno(id);

                    if (producto == null)
                    {
                        MessageBox.Show("No se encontró el producto.");
                        return;
                    }

                    asignarDatos(producto);

                    ProductoCln.actualizar(producto);

                    MessageBox.Show(
                        "Producto modificado correctamente."
                    );
                }
                else
                {
                    // REGISTRAR
                    Producto producto = new Producto();

                    asignarDatos(producto);

                    producto.fechaRegistro = DateTime.Now;
                    producto.estado = (short)Estado.Activo;

                    ProductoCln.crear(producto);

                    MessageBox.Show(
                        "Producto registrado correctamente."
                    );
                }

                cargarLista();
                limpiarFormulario();
                restablecerEstado();
            }
            catch (Exception ex)
            {
                string operacion = estabaEditando
                    ? "modificar"
                    : "registrar";

                MessageBox.Show(
                    "Error al " + operacion + " el producto:\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // MODIFICAR PRODUCTO
        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                MessageBox.Show(
                    "Primero debes seleccionar un producto."
                );

                return;
            }

            modoEdicion = true;

            Size = new Size(ANCHO, ALTO_REGISTRO);

            dgvLista.Enabled = false;

            btnNuevo.Enabled = false;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;

            btnGuardar.Enabled = true;
            btnCancelar.Enabled = true;

            pnlRegistro.Enabled = true;

            habilitarCampos(true);

            txtCodigo.Focus();
        }

        // ELIMINAR PRODUCTO
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (id == 0)
            {
                MessageBox.Show(
                    "Seleccione un producto de la lista."
                );

                return;
            }

            if (CpRodaPro.Util.usuario == null)
            {
                MessageBox.Show(
                    "No existe un usuario autenticado."
                );

                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar este producto?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                ProductoCln.eliminar(
                    id,
                    CpRodaPro.Util.usuario.usuario1
                );

                MessageBox.Show(
                    "Producto eliminado correctamente."
                );

                cargarLista();
                limpiarFormulario();
                restablecerEstado();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al eliminar el producto: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // CANCELAR
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            limpiarFormulario();
            restablecerEstado();
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
                btnBuscar.PerformClick();
                e.SuppressKeyPress = true;
            }
        }

        // SELECCIONAR PRODUCTO
        private void dgvLista_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || modoEdicion)
                return;

            DataGridViewRow fila = dgvLista.Rows[e.RowIndex];

            id = Convert.ToInt32(fila.Cells["id"].Value);

            txtCodigo.Text =
                Convert.ToString(fila.Cells["codigo"].Value);

            txtNombre.Text =
                Convert.ToString(fila.Cells["nombre"].Value);

            txtDescripcion.Text =
                Convert.ToString(fila.Cells["descripcion"].Value);

            txtMarca.Text =
                Convert.ToString(fila.Cells["marca"].Value);

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

        // VOLVER
        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
