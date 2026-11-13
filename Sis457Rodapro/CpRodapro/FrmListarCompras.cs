using ClnRodaPro;
using System;
using System.Windows.Forms;

namespace CpRodapro
{
    public partial class FrmListaCompras : Form
    {
        public FrmListaCompras()
        {
            InitializeComponent();

            Load += FrmListaCompras_Load;

            btnBuscar.Click -= btnBuscar_Click;
            btnBuscar.Click += btnBuscar_Click;

            btnCerrar.Click -= btnCerrar_Click;
            btnCerrar.Click += btnCerrar_Click;

            btnModificar.Click -= btnModificar_Click;
            btnModificar.Click += btnModificar_Click;

            btnAnular.Click -= btnAnular_Click;
            btnAnular.Click += btnAnular_Click;

            txtParametro.KeyDown -= txtParametro_KeyDown;
            txtParametro.KeyDown += txtParametro_KeyDown;
        }

        private void FrmListaCompras_Load(object sender, EventArgs e)
        {
            listar();
        }

        // LISTAR COMPRAS
        private void listar()
        {
            try
            {
                dgvCompras.DataSource =
                    CompraCln.listar(txtParametro.Text.Trim());

                dgvCompras.AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill;

                dgvCompras.ReadOnly = true;
                dgvCompras.AllowUserToAddRows = false;
                dgvCompras.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;

                dgvCompras.MultiSelect = false;

                if (dgvCompras.Columns.Count > 0)
                {
                    dgvCompras.Columns["id"].HeaderText = "N.º Compra";

                    dgvCompras.Columns["idProveedor"].Visible = false;
                    dgvCompras.Columns["idUsuario"].Visible = false;

                    dgvCompras.Columns["fecha"].HeaderText = "Fecha";
                    dgvCompras.Columns["numeroDocumento"].HeaderText = "N.º Documento";
                    dgvCompras.Columns["tipoDocumento"].HeaderText = "Tipo Documento";
                    dgvCompras.Columns["proveedor"].HeaderText = "Proveedor";
                    dgvCompras.Columns["usuarioRegistro"].HeaderText = "Registrado por";

                    dgvCompras.Columns["subtotal"].HeaderText = "Subtotal (Bs.)";
                    dgvCompras.Columns["descuento"].HeaderText = "Descuento (Bs.)";
                    dgvCompras.Columns["total"].HeaderText = "Total (Bs.)";

                    dgvCompras.Columns["fecha"].DefaultCellStyle.Format =
                        "dd/MM/yyyy HH:mm";

                    dgvCompras.Columns["subtotal"].DefaultCellStyle.Format = "N2";
                    dgvCompras.Columns["descuento"].DefaultCellStyle.Format = "N2";
                    dgvCompras.Columns["total"].DefaultCellStyle.Format = "N2";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al listar las compras: " + ex.Message,
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

        // CERRAR
        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }

        // OBTENER COMPRA SELECCIONADA
        private CompraListarDto obtenerCompraSeleccionada()
        {
            if (dgvCompras.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Seleccione una compra.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return null;
            }

            return dgvCompras.SelectedRows[0].DataBoundItem
                as CompraListarDto;
        }

        // MODIFICAR COMPRA

        private void btnModificar_Click(object sender, EventArgs e)
        {
            CompraListarDto compra = obtenerCompraSeleccionada();

            if (compra == null)
                return;

            using (FrmCompra formulario = new FrmCompra(compra.id))
            {
                DialogResult resultado = formulario.ShowDialog(this);

                if (resultado == DialogResult.OK)
                {
                    listar();

                    MessageBox.Show(
                        "El listado de compras fue actualizado.",
                        "RodaPro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }


        // ANULAR COMPRA
        private void btnAnular_Click(object sender, EventArgs e)
        {
            CompraListarDto compra = obtenerCompraSeleccionada();

            if (compra == null)
                return;

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de anular esta compra?" +
                "\n\nNúmero de compra: " + compra.id +
                "\nProveedor: " + compra.proveedor +
                "\nTotal: Bs. " + compra.total.ToString("N2") +
                "\n\nSe revertirá el stock de los productos.",
                "Confirmar anulación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                CompraCln.anular(compra.id, "admin");

                MessageBox.Show(
                    "La compra fue anulada correctamente.",
                    "Compra anulada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Actualizar listado
                listar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al anular la compra:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

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
