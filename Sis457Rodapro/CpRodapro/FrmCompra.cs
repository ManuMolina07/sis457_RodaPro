using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using CadRodapro;
using ClnRodaPro;

namespace CpRodapro
{
    public partial class FrmCompra : Form
    {
        private bool modoEdicion = false;
        private long idCompraEdicion = 0;

        public FrmCompra() : this(0)
        {
        }

        // Constructor para modificar una compra existente
        public FrmCompra(long idCompra)
        {
            InitializeComponent();

            cbxTipoDocumento.Items.Clear();
            cbxTipoDocumento.Items.Add("Factura");
            cbxTipoDocumento.Items.Add("Nota de entrega");
            cbxTipoDocumento.Items.Add("Recibo");

            cbxTipoDocumento.SelectedIndex = -1;

            configurarGrid();
            configurarNumericos();

            cargarProveedores();
            cargarProductos();

            cbxProducto.SelectedIndexChanged +=
                cbxProducto_SelectedIndexChanged;

            btnAgregar.Click += btnAgregar_Click;
            btnRegistrar.Click += btnRegistrar_Click;
            btnVolver.Click += btnVolver_Click;
            btnCancelar.Click += btnCancelar_Click;

            if (idCompra > 0)
            {
                modoEdicion = true;
                idCompraEdicion = idCompra;

                btnRegistrar.Text = "Guardar cambios";
                this.Text = "Modificar compra";

                cargarCompra(idCompra);
            }
        }

        // CONFIGURAR NUMÉRICOS
        private void configurarNumericos()
        {
            nudCantidad.DecimalPlaces = 2;
            nudCantidad.Minimum = 0.01m;
            nudCantidad.Maximum = 1000000;
            nudCantidad.Value = 1;

            nudCostoUnitario.DecimalPlaces = 2;
            nudCostoUnitario.Minimum = 0;
            nudCostoUnitario.Maximum = 1000000;

            nudDescuento.DecimalPlaces = 2;
            nudDescuento.Minimum = 0;
            nudDescuento.Maximum = 1000000;
        }

        // CONFIGURAR GRID
        private void configurarGrid()
        {
            dgvDetalle.Columns.Clear();
            dgvDetalle.AutoGenerateColumns = false;
            dgvDetalle.AllowUserToAddRows = false;
            dgvDetalle.ReadOnly = true;

            dgvDetalle.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvDetalle.MultiSelect = false;

            dgvDetalle.Columns.Add("idProducto", "ID");
            dgvDetalle.Columns.Add("nombre", "Producto");
            dgvDetalle.Columns.Add("cantidad", "Cantidad");
            dgvDetalle.Columns.Add("costo", "Costo unitario");
            dgvDetalle.Columns.Add("descuento", "Descuento");
            dgvDetalle.Columns.Add("subtotal", "Subtotal");

            dgvDetalle.Columns["idProducto"].Visible = false;

            dgvDetalle.Columns["cantidad"].DefaultCellStyle.Format = "N2";
            dgvDetalle.Columns["costo"].DefaultCellStyle.Format = "'Bs' #,##0.00";
            dgvDetalle.Columns["descuento"].DefaultCellStyle.Format = "'Bs' #,##0.00";
            dgvDetalle.Columns["subtotal"].DefaultCellStyle.Format = "'Bs' #,##0.00";
        }

        // CARGAR PROVEEDORES
        private void cargarProveedores()
        {
            try
            {
                using (var context = new LabRodaproEntities())
                {
                    var proveedores = context.Proveedor
                        .Where(p => p.estado == 1)
                        .OrderBy(p => p.nombreEmpresa)
                        .ToList();

                    cbxProveedor.DataSource = proveedores;
                    cbxProveedor.DisplayMember = "nombreEmpresa";
                    cbxProveedor.ValueMember = "id";
                    cbxProveedor.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar proveedores: " + ex.Message
                );
            }
        }

        // CARGAR PRODUCTOS
        private void cargarProductos()
        {
            try
            {
                using (var context = new LabRodaproEntities())
                {
                    var productos = context.Producto
                        .Where(p => p.estado == 1)
                        .OrderBy(p => p.nombre)
                        .ToList();

                    cbxProducto.DataSource = productos;
                    cbxProducto.DisplayMember = "nombre";
                    cbxProducto.ValueMember = "id";
                    cbxProducto.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar productos: " + ex.Message
                );
            }
        }

        // CARGAR COMPRA PARA MODIFICAR
        private void cargarCompra(long idCompra)
        {
            try
            {
                using (var context = new LabRodaproEntities())
                {
                    var compra = context.Compra.FirstOrDefault(
                        x => x.id == idCompra && x.estado == 1
                    );

                    if (compra == null)
                        throw new Exception("La compra no existe o está anulada.");

                    // Cargar encabezado
                    cbxProveedor.SelectedValue = compra.idProveedor;

                    dtpFecha.Value = compra.fecha;

                    txtNumeroDocumento.Text =
                        compra.numeroDocumento;

                    cbxTipoDocumento.SelectedItem =
                        compra.tipoDocumento;

                    // Cargar detalles
                    var detalles = context.CompraDetalle
                        .Where(x => x.idCompra == idCompra && x.estado == 1)
                        .ToList();

                    dgvDetalle.Rows.Clear();

                    foreach (var detalle in detalles)
                    {
                        var producto = context.Producto.FirstOrDefault(
                            x => x.id == detalle.idProducto
                        );

                        if (producto == null)
                            continue;

                        decimal subtotal =
                            detalle.cantidad * detalle.costoUnitario;

                        dgvDetalle.Rows.Add(
                            detalle.idProducto,
                            producto.nombre,
                            detalle.cantidad,
                            detalle.costoUnitario,
                            detalle.descuento,
                            subtotal
                        );
                    }

                    calcularTotales();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al cargar la compra:\n" + ex.Message,
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                btnRegistrar.Enabled = false;
            }
        }

        // CARGAR PRECIO DE COMPRA
        private void cbxProducto_SelectedIndexChanged(
            object sender, EventArgs e)
        {
            if (cbxProducto.SelectedItem is Producto producto)
            {
                decimal precio = producto.precioCompra;

                if (precio >= nudCostoUnitario.Minimum &&
                    precio <= nudCostoUnitario.Maximum)
                {
                    nudCostoUnitario.Value = precio;
                }
            }
        }

        // AGREGAR PRODUCTO
        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (cbxProducto.SelectedItem == null)
            {
                MessageBox.Show("Seleccione un producto.");
                cbxProducto.Focus();
                return;
            }

            Producto producto = cbxProducto.SelectedItem as Producto;

            if (producto == null)
                return;

            decimal cantidad = nudCantidad.Value;
            decimal costo = nudCostoUnitario.Value;
            decimal descuento = nudDescuento.Value;

            decimal subtotal = cantidad * costo;

            if (cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser mayor a cero.");
                return;
            }

            if (costo <= 0)
            {
                MessageBox.Show("El costo debe ser mayor a cero.");
                return;
            }

            if (descuento > subtotal)
            {
                MessageBox.Show(
                    "El descuento no puede ser mayor al subtotal."
                );
                return;
            }

            dgvDetalle.Rows.Add(
                producto.id,
                producto.nombre,
                cantidad,
                costo,
                descuento,
                subtotal
            );

            calcularTotales();

            cbxProducto.SelectedIndex = -1;
            nudCantidad.Value = 1;
            nudCostoUnitario.Value = 0;
            nudDescuento.Value = 0;

            cbxProducto.Focus();
        }

        // CALCULAR TOTALES
        private void calcularTotales()
        {
            decimal subtotalGeneral = 0;
            decimal descuentoGeneral = 0;

            foreach (DataGridViewRow fila in dgvDetalle.Rows)
            {
                subtotalGeneral += Convert.ToDecimal(
                    fila.Cells["subtotal"].Value
                );

                descuentoGeneral += Convert.ToDecimal(
                    fila.Cells["descuento"].Value
                );
            }

            decimal totalGeneral =
                subtotalGeneral - descuentoGeneral;

            lblSubtotal.Text = "Bs " + subtotalGeneral.ToString("N2");
            lblDescuento.Text = "Bs " + descuentoGeneral.ToString("N2");
            lblTotal.Text = "Bs " + totalGeneral.ToString("N2");
        }

        // REGISTRAR O MODIFICAR COMPRA
        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                Proveedor proveedor =
                    cbxProveedor.SelectedItem as Proveedor;

                if (proveedor == null)
                {
                    MessageBox.Show("Seleccione un proveedor.");
                    cbxProveedor.Focus();
                    return;
                }

                if (cbxTipoDocumento.SelectedIndex == -1)
                {
                    MessageBox.Show("Seleccione el tipo de documento.");
                    cbxTipoDocumento.Focus();
                    return;
                }

                if (dgvDetalle.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "Agregue al menos un producto a la compra."
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

                Compra compra = new Compra
                {
                    idProveedor = proveedor.id,
                    idUsuario = CpRodaPro.Util.usuario.id,
                    fecha = dtpFecha.Value,
                    numeroDocumento = txtNumeroDocumento.Text.Trim(),
                    tipoDocumento = cbxTipoDocumento.Text
                };

                List<CompraDetalle> detalles =
                    new List<CompraDetalle>();

                foreach (DataGridViewRow fila in dgvDetalle.Rows)
                {
                    CompraDetalle detalle = new CompraDetalle
                    {
                        idProducto = Convert.ToInt32(
                            fila.Cells["idProducto"].Value
                        ),

                        cantidad = Convert.ToDecimal(
                            fila.Cells["cantidad"].Value
                        ),

                        costoUnitario = Convert.ToDecimal(
                            fila.Cells["costo"].Value
                        ),

                        descuento = Convert.ToDecimal(
                            fila.Cells["descuento"].Value
                        )
                    };

                    detalles.Add(detalle);
                }

                if (modoEdicion)
                {
                    compra.id = idCompraEdicion;

                    CompraCln.modificar(
                        compra,
                        detalles,
                        CpRodaPro.Util.usuario.usuario1
                    );

                    MessageBox.Show(
                        "Compra modificada correctamente.",
                        "RodaPro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    long idCompra = CompraCln.registrar(
                        compra,
                        detalles,
                        CpRodaPro.Util.usuario.usuario1
                    );

                    MessageBox.Show(
                        "Compra registrada correctamente.\n" +
                        "Número de compra: " + idCompra,
                        "RodaPro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    dgvDetalle.Rows.Clear();
                    calcularTotales();

                    cbxProveedor.SelectedIndex = -1;
                    cbxTipoDocumento.SelectedIndex = -1;
                    txtNumeroDocumento.Clear();
                    dtpFecha.Value = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al guardar la compra:\n" + ex.Message,
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        // VOLVER
        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // CANCELAR
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            DialogResult respuesta = MessageBox.Show(
                "¿Desea cancelar la operación actual?",
                "Cancelar compra",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta == DialogResult.Yes)
            {
                if (modoEdicion)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }

                cbxProveedor.SelectedIndex = -1;
                cbxTipoDocumento.SelectedIndex = -1;
                txtNumeroDocumento.Clear();
                dtpFecha.Value = DateTime.Now;

                dgvDetalle.Rows.Clear();

                cbxProducto.SelectedIndex = -1;
                nudCantidad.Value = 1;
                nudCostoUnitario.Value = 0;
                nudDescuento.Value = 0;

                calcularTotales();

                cbxProveedor.Focus();

                MessageBox.Show("Compra cancelada correctamente.");
            }
        }

        // ELIMINAR PRODUCTO DEL DETALLE
        private void btnEliminarProducto_Click(object sender, EventArgs e)
        {
            if (dgvDetalle.CurrentRow == null)
            {
                MessageBox.Show(
                    "Seleccione un producto del detalle para eliminarlo.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar este producto del detalle?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta == DialogResult.Yes)
            {
                dgvDetalle.Rows.Remove(dgvDetalle.CurrentRow);
                calcularTotales();
            }
        }

        // VER COMPRAS
        private void btnVerCompras_Click(object sender, EventArgs e)
        {
            btnVerCompras.Enabled = false;

            try
            {
                using (FrmListaCompras formulario =
                    new FrmListaCompras())
                {
                    formulario.ShowDialog(this);
                }
            }
            finally
            {
                btnVerCompras.Enabled = true;
            }
        }
    }
}
