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
        private Dictionary<int, decimal> stockOriginalVenta =
            new Dictionary<int, decimal>();

        public FrmVenta()
        {
            InitializeComponent();

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

        private void FrmVenta_Load(object sender, EventArgs e)
        {
            // TIPO DE DOCUMENTO
            cbxTipoDocumento.Items.Clear();
            cbxTipoDocumento.Items.Add("Factura");
            cbxTipoDocumento.Items.Add("Nota de venta");
            cbxTipoDocumento.Items.Add("Recibo");
            cbxTipoDocumento.SelectedIndex = -1;
            cbxTipoDocumento.DropDownStyle = ComboBoxStyle.DropDownList;

            // COMBOBOX
            cbxCliente.DropDownStyle = ComboBoxStyle.DropDownList;
            cbxProducto.DropDownStyle = ComboBoxStyle.DropDownList;

            // FECHA
            dtpFecha.Value = DateTime.Now;

            // CANTIDAD
            nudCantidad.DecimalPlaces = 2;
            nudCantidad.Minimum = 0.01m;
            nudCantidad.Maximum = 999999;
            nudCantidad.Value = 1;

            // PRECIO
            txtPrecio.ReadOnly = true;
            txtPrecio.Text = "0.00";

            // DESCUENTOS
            txtDescuentoDetalle.Text = "0";
            txtDescuento.Text = "0";

            // TOTALES
            lblSubtotal.Text = "Bs. 0,00";
            lblDescuento.Text = "Bs. 0,00";
            lblTotal.Text = "Bs. 0,00";

            // CONFIGURAR GRID
            ConfigurarDetalle();

            // CARGAR INFORMACIÓN
            CargarClientes();
            CargarProductos();

            // COMPROBAR SI ES MODIFICACIÓN
            if (ModoEdicion)
            {
                btnGuardar.Text = "Actualizar";
                btnVerVentas.Enabled = false;

                CargarVentaEditar();
            }
            else
            {
                btnGuardar.Text = "Guardar";
            }

            CalcularTotales();
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
                    .Select(p => new
                    {
                        id = p.id,
                        descripcion = p.codigo + " - " + p.nombre,
                        precio = p.precioVenta,
                        stock = p.stock
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
        }

        // CARGAR VENTA PARA MODIFICAR
        private void CargarVentaEditar()
        {
            try
            {
                Venta venta = VentaCln.obtenerPorId(IdVentaEditar);

                if (venta == null)
                {
                    MessageBox.Show("La venta no existe o está anulada.");
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

                // RECUPERAR DETALLES
                var detalles = VentaCln.listarDetalles(IdVentaEditar);

                if (detalles.Count == 0)
                {
                    MessageBox.Show(
                        "Esta venta no tiene productos registrados.");
                    Close();
                    return;
                }

                var productos = ProductoCln.listar();

                stockOriginalVenta.Clear();

                foreach (var detalle in detalles)
                {
                    var producto = productos.FirstOrDefault(
                        p => p.id == detalle.idProducto);

                    string nombreProducto;

                    if (producto != null)
                    {
                        nombreProducto =
                            producto.codigo + " - " + producto.nombre;

                        decimal stock = Convert.ToDecimal(producto.stock);

                        if (stockOriginalVenta.ContainsKey(detalle.idProducto))
                            stockOriginalVenta[detalle.idProducto] += detalle.cantidad;
                        else
                            stockOriginalVenta.Add(detalle.idProducto, detalle.cantidad);
                    }
                    else
                    {
                        nombreProducto = "Producto ID: " + detalle.idProducto;
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
                        subtotalDetalle
                    );
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

        // MOSTRAR PRECIO DEL PRODUCTO
        private void cbxProducto_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            if (cbxProducto.SelectedItem == null)
            {
                txtPrecio.Text = "0.00";
                return;
            }

            dynamic producto = cbxProducto.SelectedItem;

            txtPrecio.Text =
                Convert.ToDecimal(producto.precio).ToString("N2");
        }

        // AGREGAR PRODUCTO AL DETALLE
        private void btnAgregarProducto_Click(
            object sender, EventArgs e)
        {
            if (cbxProducto.SelectedIndex == -1)
            {
                MessageBox.Show("Seleccione un producto.");
                return;
            }

            dynamic producto = cbxProducto.SelectedItem;

            int idProducto = Convert.ToInt32(producto.id);
            string nombreProducto = producto.descripcion;
            decimal precio = Convert.ToDecimal(producto.precio);
            decimal stock = Convert.ToDecimal(producto.stock);
            decimal cantidad = nudCantidad.Value;

            decimal descuento = 0;

            if (!decimal.TryParse(
                txtDescuentoDetalle.Text, out descuento))
            {
                MessageBox.Show("Ingrese un descuento válido.");
                return;
            }

            if (descuento < 0)
            {
                MessageBox.Show(
                    "El descuento no puede ser negativo.");
                return;
            }

            // EN MODIFICACIÓN SE SUMA EL STOCK ORIGINAL
            decimal stockDisponible = stock;

            if (ModoEdicion &&
                stockOriginalVenta.ContainsKey(idProducto))
            {
                stockDisponible += stockOriginalVenta[idProducto];
            }

            if (cantidad > stockDisponible)
            {
                MessageBox.Show(
                    "La cantidad supera el stock disponible. Stock: " +
                    stockDisponible);
                return;
            }

            decimal importe = cantidad * precio;

            if (descuento > importe)
            {
                MessageBox.Show(
                    "El descuento no puede superar el importe del producto.");
                return;
            }

            // EVITAR PRODUCTOS DUPLICADOS
            foreach (DataGridViewRow fila in dgvDetalle.Rows)
            {
                if (Convert.ToInt32(
                    fila.Cells["idProducto"].Value) == idProducto)
                {
                    MessageBox.Show(
                        "Este producto ya está agregado al detalle.");
                    return;
                }
            }

            decimal subtotal = importe - descuento;

            dgvDetalle.Rows.Add(
                idProducto,
                nombreProducto,
                cantidad,
                precio,
                descuento,
                subtotal
            );

            CalcularTotales();

            // LIMPIAR CAMPOS
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

        // CALCULAR TOTALES
        private void CalcularTotales()
        {
            decimal subtotal = 0;
            decimal descuentoGeneral = 0;

            foreach (DataGridViewRow fila in dgvDetalle.Rows)
            {
                if (fila.Cells["subtotal"].Value != null)
                {
                    subtotal += Convert.ToDecimal(
                        fila.Cells["subtotal"].Value);
                }
            }

            decimal.TryParse(
                txtDescuento.Text, out descuentoGeneral);

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

        // GUARDAR O ACTUALIZAR VENTA
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                // VALIDAR USUARIO
                if (IdUsuarioActual <= 0 ||
                    string.IsNullOrWhiteSpace(UsuarioActual))
                {
                    MessageBox.Show(
                        "Debe iniciar sesión para registrar una venta.");
                    return;
                }

                // VALIDAR TIPO DE DOCUMENTO
                if (cbxTipoDocumento.SelectedIndex == -1)
                {
                    MessageBox.Show("Seleccione el tipo de documento.");
                    cbxTipoDocumento.Focus();
                    return;
                }

                // VALIDAR NÚMERO DE DOCUMENTO
                if (string.IsNullOrWhiteSpace(txtNumeroDocumento.Text))
                {
                    MessageBox.Show("Ingrese el número de documento.");
                    txtNumeroDocumento.Focus();
                    return;
                }

                // VALIDAR DETALLE
                if (dgvDetalle.Rows.Count == 0)
                {
                    MessageBox.Show("Agregue al menos un producto.");
                    return;
                }

                // VALIDAR DESCUENTO GENERAL
                decimal descuento;

                if (!decimal.TryParse(
                    txtDescuento.Text, out descuento) ||
                    descuento < 0)
                {
                    MessageBox.Show(
                        "Ingrese un descuento general válido.");
                    txtDescuento.Focus();
                    return;
                }

                // CALCULAR SUBTOTAL
                decimal subtotal = 0;

                foreach (DataGridViewRow fila in dgvDetalle.Rows)
                {
                    subtotal += Convert.ToDecimal(
                        fila.Cells["subtotal"].Value);
                }

                if (descuento > subtotal)
                {
                    MessageBox.Show(
                        "El descuento no puede superar el subtotal.");
                    return;
                }

                // OBTENER CLIENTE
                int? idCliente = null;

                if (cbxCliente.SelectedIndex >= 0 &&
                    cbxCliente.SelectedValue != null)
                {
                    idCliente = Convert.ToInt32(
                        cbxCliente.SelectedValue);
                }

                // CREAR CABECERA DE VENTA
                Venta venta = new Venta
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

                // CREAR DETALLES
                List<VentaDetalle> detalles =
                    new List<VentaDetalle>();

                foreach (DataGridViewRow fila in dgvDetalle.Rows)
                {
                    VentaDetalle detalle = new VentaDetalle
                    {
                        idProducto = Convert.ToInt32(
                            fila.Cells["idProducto"].Value),

                        cantidad = Convert.ToDecimal(
                            fila.Cells["cantidad"].Value),

                        precioUnitario = Convert.ToDecimal(
                            fila.Cells["precio"].Value),

                        descuento = Convert.ToDecimal(
                            fila.Cells["descuento"].Value)
                    };

                    detalles.Add(detalle);
                }

                // DECIDIR ENTRE REGISTRAR Y MODIFICAR
                if (ModoEdicion)
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
                else
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

                    // LIMPIAR FORMULARIO
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

                    // ACTUALIZAR STOCK DISPONIBLE
                    CargarProductos();
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

        // ABRIR LISTA DE VENTAS
        private void btnVerVentas_Click(object sender, EventArgs e)
        {
            using (FrmListaVentas formulario = new FrmListaVentas())
            {
                formulario.IdUsuarioActual = IdUsuarioActual;
                formulario.UsuarioActual = UsuarioActual;
                formulario.ShowDialog(this);
            }
        }

        // VOLVER
        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}



