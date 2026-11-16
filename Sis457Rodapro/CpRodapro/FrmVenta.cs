using CadRodapro;
using ClnRodaPro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CpRodapro
{
    public partial class FrmVenta : Form
    {
        public int IdUsuarioActual { get; set; }
        public string UsuarioActual { get; set; }

        // PROPIEDADES PARA MODIFICAR VENTAS
        public bool ModoEdicion { get; set; } = false;
        public long IdVentaEditar { get; set; } = 0;

        // STOCK ORIGINAL DE LOS PRODUCTOS DE LA VENTA
        private readonly Dictionary<int, decimal> stockOriginalVenta =
            new Dictionary<int, decimal>();

        // CLASE AUXILIAR PARA LOS PRODUCTOS DEL COMBOBOX
        private class ProductoComboItem
        {
            public int id { get; set; }
            public string descripcion { get; set; }
            public decimal precio { get; set; }
            public decimal stock { get; set; }
        }

        public FrmVenta()
        {
            InitializeComponent();

            // EVENTOS
            Load -= FrmVenta_Load;
            Load += FrmVenta_Load;

            txtDescuento.TextChanged -= txtDescuento_TextChanged;
            txtDescuento.TextChanged += txtDescuento_TextChanged;

            cbxProducto.SelectedIndexChanged -= cbxProducto_SelectedIndexChanged;
            cbxProducto.SelectedIndexChanged += cbxProducto_SelectedIndexChanged;

            btnVolver.Click -= btnVolver_Click;
            btnVolver.Click += btnVolver_Click;

            btnAgregarProducto.Click -= btnAgregarProducto_Click;
            btnAgregarProducto.Click += btnAgregarProducto_Click;

            btnVerVentas.Click -= btnVerVentas_Click;
            btnVerVentas.Click += btnVerVentas_Click;

            btnGuardar.Click -= btnGuardar_Click;
            btnGuardar.Click += btnGuardar_Click;
        }

        // CARGAR FORMULARIO
        private void FrmVenta_Load(object sender, EventArgs e)
        {
            ConfigurarFormulario();
            ConfigurarDetalle();

            CargarClientes();
            CargarProductos();

            if (ModoEdicion)
            {
                btnGuardar.Text = "Actualizar";
                btnVerVentas.Enabled = false;

                CargarVentaEditar();
            }
            else
            {
                btnGuardar.Text = "Guardar";
                btnVerVentas.Enabled = true;
            }

            CalcularTotales();
        }

        // CONFIGURAR FORMULARIO
        private void ConfigurarFormulario()
        {
            cbxTipoDocumento.Items.Clear();
            cbxTipoDocumento.Items.Add("Factura");
            cbxTipoDocumento.Items.Add("Nota de venta");
            cbxTipoDocumento.Items.Add("Recibo");

            cbxTipoDocumento.SelectedIndex = -1;
            cbxTipoDocumento.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cbxCliente.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cbxProducto.DropDownStyle =
                ComboBoxStyle.DropDownList;

            dtpFecha.Value = DateTime.Now;

            nudCantidad.DecimalPlaces = 2;
            nudCantidad.Minimum = 0.01m;
            nudCantidad.Maximum = 999999;
            nudCantidad.Value = 1;

            txtPrecio.ReadOnly = true;
            txtPrecio.Text = "0.00";

            txtDescuentoDetalle.Text = "0";
            txtDescuento.Text = "0";

            lblSubtotal.Text = "Subtotal: Bs. 0,00";
            lblDescuento.Text = "Descuento: Bs. 0,00";
            lblTotal.Text = "Total a pagar: Bs. 0,00";
        }

        // CONFIGURAR GRID
        private void ConfigurarDetalle()
        {
            dgvDetalle.Columns.Clear();
            dgvDetalle.AutoGenerateColumns = false;
            dgvDetalle.AllowUserToAddRows = false;
            dgvDetalle.ReadOnly = true;

            dgvDetalle.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvDetalle.MultiSelect = false;

            dgvDetalle.Columns.Add("idProducto", "ID");
            dgvDetalle.Columns.Add("producto", "Producto");
            dgvDetalle.Columns.Add("cantidad", "Cantidad");
            dgvDetalle.Columns.Add("precio", "Precio (Bs.)");
            dgvDetalle.Columns.Add("descuento", "Descuento (Bs.)");
            dgvDetalle.Columns.Add("subtotal", "Subtotal (Bs.)");

            dgvDetalle.Columns["idProducto"].Visible = false;

            dgvDetalle.Columns["cantidad"].DefaultCellStyle.Format = "N2";
            dgvDetalle.Columns["precio"].DefaultCellStyle.Format = "N2";
            dgvDetalle.Columns["descuento"].DefaultCellStyle.Format = "N2";
            dgvDetalle.Columns["subtotal"].DefaultCellStyle.Format = "N2";
        }

        // CARGAR CLIENTES
        private void CargarClientes()
        {
            try
            {
                var clientes = ClienteCln.listar("");

                cbxCliente.DataSource = clientes
                    .Select(c => new
                    {
                        id = c.id,
                        nombreCompleto = c.nombre + " " + c.apellido
                    })
                    .ToList();

                cbxCliente.DisplayMember = "nombreCompleto";
                cbxCliente.ValueMember = "id";
                cbxCliente.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar clientes: " + ex.Message,
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // CARGAR PRODUCTOS
        private void CargarProductos()
        {
            try
            {
                var productos = ProductoCln.listar();

                cbxProducto.DataSource = productos
                    .Select(p => new ProductoComboItem
                    {
                        id = p.id,
                        descripcion = p.codigo + " - " + p.nombre,
                        precio = Convert.ToDecimal(p.precioVenta),
                        stock = Convert.ToDecimal(p.stock)
                    })
                    .ToList();

                cbxProducto.DisplayMember = "descripcion";
                cbxProducto.ValueMember = "id";
                cbxProducto.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar productos: " + ex.Message,
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // CARGAR VENTA PARA MODIFICAR
        private void CargarVentaEditar()
        {
            try
            {
                Venta venta = VentaCln.obtenerPorId(IdVentaEditar);

                if (venta == null)
                {
                    MessageBox.Show(
                        "La venta no existe o está anulada.",
                        "RodaPro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    Close();
                    return;
                }

                // CARGAR CABECERA
                if (venta.idCliente.HasValue)
                    cbxCliente.SelectedValue = venta.idCliente.Value;
                else
                    cbxCliente.SelectedIndex = -1;

                cbxTipoDocumento.SelectedItem = venta.tipoDocumento;

                txtNumeroDocumento.Text = venta.numeroDocumento;

                dtpFecha.Value = venta.fecha;

                txtDescuento.Text = venta.descuento.ToString();

                // CARGAR DETALLES
                var detalles = VentaCln.listarDetalles(IdVentaEditar);

                if (detalles == null || detalles.Count == 0)
                {
                    MessageBox.Show(
                        "Esta venta no tiene productos registrados.",
                        "RodaPro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    Close();
                    return;
                }

                stockOriginalVenta.Clear();

                foreach (var detalle in detalles)
                {
                    ProductoComboItem producto =
                        cbxProducto.Items
                            .Cast<ProductoComboItem>()
                            .FirstOrDefault(p => p.id == detalle.idProducto);

                    string nombreProducto =
                        producto != null
                            ? producto.descripcion
                            : "Producto ID: " + detalle.idProducto;

                    // GUARDAR CANTIDAD ORIGINAL
                    if (stockOriginalVenta.ContainsKey(detalle.idProducto))
                    {
                        stockOriginalVenta[detalle.idProducto] +=
                            detalle.cantidad;
                    }
                    else
                    {
                        stockOriginalVenta.Add(
                            detalle.idProducto,
                            detalle.cantidad);
                    }

                    decimal subtotalDetalle =
                        detalle.cantidad * detalle.precioUnitario
                        - detalle.descuento;

                    dgvDetalle.Rows.Add(
                        detalle.idProducto,
                        nombreProducto,
                        detalle.cantidad,
                        detalle.precioUnitario,
                        detalle.descuento,
                        subtotalDetalle);
                }

                CalcularTotales();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar la venta: " + ex.Message,
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Close();
            }
        }

        // OBTENER PRODUCTO SELECCIONADO
        private ProductoComboItem ObtenerProductoSeleccionado()
        {
            return cbxProducto.SelectedItem as ProductoComboItem;
        }

        // MOSTRAR PRECIO DEL PRODUCTO
        private void cbxProducto_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            ProductoComboItem producto = ObtenerProductoSeleccionado();

            if (producto == null)
            {
                txtPrecio.Text = "0.00";
                return;
            }

            txtPrecio.Text = producto.precio.ToString("N2");
        }

        // AGREGAR PRODUCTO AL DETALLE
        private void btnAgregarProducto_Click(
            object sender, EventArgs e)
        {
            ProductoComboItem producto = ObtenerProductoSeleccionado();

            if (producto == null)
            {
                MessageBox.Show(
                    "Seleccione un producto.",
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            decimal cantidad = nudCantidad.Value;
            decimal descuento;

            if (!decimal.TryParse(
                txtDescuentoDetalle.Text, out descuento) ||
                descuento < 0)
            {
                MessageBox.Show(
                    "Ingrese un descuento válido.",
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtDescuentoDetalle.Focus();
                return;
            }

            // VALIDAR STOCK DISPONIBLE
            decimal stockDisponible = producto.stock;

            if (ModoEdicion &&
                stockOriginalVenta.ContainsKey(producto.id))
            {
                stockDisponible += stockOriginalVenta[producto.id];
            }

            if (cantidad > stockDisponible)
            {
                MessageBox.Show(
                    "La cantidad supera el stock disponible. Stock: " +
                    stockDisponible,
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // VALIDAR DESCUENTO POR PRODUCTO
            decimal importe = cantidad * producto.precio;

            if (descuento > importe)
            {
                MessageBox.Show(
                    "El descuento no puede superar el importe del producto.",
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // EVITAR PRODUCTOS DUPLICADOS
            if (ProductoYaAgregado(producto.id))
            {
                MessageBox.Show(
                    "Este producto ya está agregado al detalle.",
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            decimal subtotal = importe - descuento;

            dgvDetalle.Rows.Add(
                producto.id,
                producto.descripcion,
                cantidad,
                producto.precio,
                descuento,
                subtotal);

            CalcularTotales();
            LimpiarProductoSeleccionado();
        }

        // COMPROBAR SI EL PRODUCTO YA ESTÁ AGREGADO
        private bool ProductoYaAgregado(int idProducto)
        {
            foreach (DataGridViewRow fila in dgvDetalle.Rows)
            {
                if (fila.Cells["idProducto"].Value != null &&
                    Convert.ToInt32(
                        fila.Cells["idProducto"].Value) == idProducto)
                {
                    return true;
                }
            }

            return false;
        }

        // LIMPIAR CAMPOS DEL PRODUCTO
        private void LimpiarProductoSeleccionado()
        {
            cbxProducto.SelectedIndex = -1;
            txtPrecio.Text = "0.00";
            nudCantidad.Value = 1;
            txtDescuentoDetalle.Text = "0";
        }

        // CAMBIAR DESCUENTO GENERAL
        private void txtDescuento_TextChanged(
            object sender, EventArgs e)
        {
            CalcularTotales();
        }

        // OBTENER SUBTOTAL DE LA VENTA
        private decimal ObtenerSubtotal()
        {
            decimal subtotal = 0;

            foreach (DataGridViewRow fila in dgvDetalle.Rows)
            {
                if (fila.Cells["subtotal"].Value != null)
                {
                    subtotal += Convert.ToDecimal(
                        fila.Cells["subtotal"].Value);
                }
            }

            return subtotal;
        }

        // CALCULAR TOTALES
        private void CalcularTotales()
        {
            decimal subtotal = ObtenerSubtotal();
            decimal descuentoGeneral = 0;

            decimal.TryParse(
                txtDescuento.Text,
                out descuentoGeneral);

            if (descuentoGeneral < 0)
                descuentoGeneral = 0;

            decimal descuentoAplicado =
                Math.Min(descuentoGeneral, subtotal);

            decimal total = subtotal - descuentoAplicado;

            lblSubtotal.Text =
                "Subtotal: Bs. " + subtotal.ToString("N2");

            lblDescuento.Text =
                "Descuento: Bs. " + descuentoAplicado.ToString("N2");

            lblTotal.Text =
                "Total a pagar: Bs. " + total.ToString("N2");
        }

        // VALIDAR DATOS DE LA VENTA
        private bool ValidarVenta(
            out decimal subtotal,
            out decimal descuento)
        {
            subtotal = 0;
            descuento = 0;

            if (IdUsuarioActual <= 0 ||
                string.IsNullOrWhiteSpace(UsuarioActual))
            {
                MessageBox.Show(
                    "Debe iniciar sesión para registrar una venta.");
                return false;
            }

            if (cbxTipoDocumento.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione el tipo de documento.");
                cbxTipoDocumento.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNumeroDocumento.Text))
            {
                MessageBox.Show("Ingrese el número de documento.");
                txtNumeroDocumento.Focus();
                return false;
            }

            if (dgvDetalle.Rows.Count == 0)
            {
                MessageBox.Show("Agregue al menos un producto.");
                return false;
            }

            if (!decimal.TryParse(txtDescuento.Text, out descuento) ||
                descuento < 0)
            {
                MessageBox.Show(
                    "Ingrese un descuento general válido.");

                txtDescuento.Focus();
                return false;
            }

            subtotal = ObtenerSubtotal();

            if (descuento > subtotal)
            {
                MessageBox.Show(
                    "El descuento no puede superar el subtotal.");
                txtDescuento.Focus();
                return false;
            }

            return true;
        }

        // CONSTRUIR CABECERA DE VENTA
        private Venta CrearVenta(decimal subtotal, decimal descuento)
        {
            int? idCliente = null;

            if (cbxCliente.SelectedIndex >= 0 &&
                cbxCliente.SelectedValue != null)
            {
                idCliente = Convert.ToInt32(
                    cbxCliente.SelectedValue);
            }

            return new Venta
            {
                id = ModoEdicion ? IdVentaEditar : 0,
                idCliente = idCliente,
                idUsuario = IdUsuarioActual,
                fecha = dtpFecha.Value,
                numeroDocumento = txtNumeroDocumento.Text.Trim(),
                tipoDocumento = cbxTipoDocumento.Text,
                subtotal = subtotal,
                descuento = descuento,
                total = subtotal - descuento
            };
        }

        // CONSTRUIR DETALLES DE VENTA
        private List<VentaDetalle> CrearDetalles()
        {
            List<VentaDetalle> detalles =
                new List<VentaDetalle>();

            foreach (DataGridViewRow fila in dgvDetalle.Rows)
            {
                detalles.Add(new VentaDetalle
                {
                    idProducto = Convert.ToInt32(
                        fila.Cells["idProducto"].Value),

                    cantidad = Convert.ToDecimal(
                        fila.Cells["cantidad"].Value),

                    precioUnitario = Convert.ToDecimal(
                        fila.Cells["precio"].Value),

                    descuento = Convert.ToDecimal(
                        fila.Cells["descuento"].Value)
                });
            }

            return detalles;
        }

        // GUARDAR O ACTUALIZAR VENTA
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            decimal subtotal;
            decimal descuento;

            if (!ValidarVenta(out subtotal, out descuento))
                return;

            try
            {
                Venta venta = CrearVenta(subtotal, descuento);
                List<VentaDetalle> detalles = CrearDetalles();

                if (ModoEdicion)
                {
                    ActualizarVenta(venta, detalles);
                }
                else
                {
                    RegistrarVenta(venta, detalles);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al guardar la venta: " + ex.Message,
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // REGISTRAR NUEVA VENTA
        private void RegistrarVenta(
            Venta venta,
            List<VentaDetalle> detalles)
        {
            long idVenta = VentaCln.registrar(
                venta,
                detalles,
                UsuarioActual);

            MessageBox.Show(
                "Venta registrada correctamente.\n" +
                "Número de venta: " + idVenta,
                "RodaPro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            LimpiarFormulario();
            CargarProductos();
        }

        // ACTUALIZAR VENTA
        private void ActualizarVenta(
            Venta venta,
            List<VentaDetalle> detalles)
        {
            VentaCln.modificar(
                venta,
                detalles,
                UsuarioActual);

            MessageBox.Show(
                "Venta modificada correctamente.",
                "RodaPro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            DialogResult = DialogResult.OK;
            Close();
        }

        // LIMPIAR FORMULARIO
        private void LimpiarFormulario()
        {
            dgvDetalle.Rows.Clear();

            txtNumeroDocumento.Clear();
            txtDescuento.Text = "0";
            txtDescuentoDetalle.Text = "0";

            cbxCliente.SelectedIndex = -1;
            cbxTipoDocumento.SelectedIndex = -1;
            cbxProducto.SelectedIndex = -1;

            nudCantidad.Value = 1;
            txtPrecio.Text = "0.00";

            dtpFecha.Value = DateTime.Now;

            CalcularTotales();
        }

        // VER VENTAS REGISTRADAS
        private void btnVerVentas_Click(object sender, EventArgs e)
        {
            btnVerVentas.Enabled = false;

            try
            {
                int idUsuario = IdUsuarioActual;
                string usuario = UsuarioActual;

                if (CpRodaPro.Util.usuario != null)
                {
                    if (idUsuario <= 0)
                        idUsuario = CpRodaPro.Util.usuario.id;

                    if (string.IsNullOrWhiteSpace(usuario))
                        usuario = CpRodaPro.Util.usuario.usuario1;
                }

                using (FrmListaVentas formulario = new FrmListaVentas())
                {
                    formulario.IdUsuarioActual = idUsuario;
                    formulario.UsuarioActual = usuario;

                    // FrmVenta es el propietario de FrmListaVentas.
                    formulario.ShowDialog(this);
                }
            }
            finally
            {
                if (!IsDisposed)
                    btnVerVentas.Enabled = true;
            }
        }

        // VOLVER
        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}