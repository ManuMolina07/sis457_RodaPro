using ClnRodaPro;
using System;
using System.Windows.Forms;

namespace CpRodapro
{
    public partial class FrmListaCompras : Form
    {
        public int IdUsuarioActual { get; set; }
        public string UsuarioActual { get; set; }

        public FrmListaCompras()
        {
            InitializeComponent();

            Load += FrmListaCompras_Load;

            btnBuscar.Click -= btnBuscar_Click;
            btnBuscar.Click += btnBuscar_Click;

            btnNuevo.Click -= btnNuevo_Click;
            btnNuevo.Click += btnNuevo_Click;

            btnModificar.Click -= btnModificar_Click;
            btnModificar.Click += btnModificar_Click;

            btnAnular.Click -= btnAnular_Click;
            btnAnular.Click += btnAnular_Click;

            btnCancelar.Click -= btnCancelar_Click;
            btnCancelar.Click += btnCancelar_Click;

            btnVolver.Click -= btnVolver_Click;
            btnVolver.Click += btnVolver_Click;

            txtParametro.KeyDown -= txtParametro_KeyDown;
            txtParametro.KeyDown += txtParametro_KeyDown;
        }

        private void FrmListaCompras_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            listar();
        }

        // CONFIGURAR GRILLA
        private void ConfigurarGrilla()
        {
            dgvCompras.AutoGenerateColumns = true;
            dgvCompras.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvCompras.MultiSelect = false;
            dgvCompras.ReadOnly = true;
            dgvCompras.AllowUserToAddRows = false;
            dgvCompras.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        // LISTAR COMPRAS
        private void listar()
        {
            try
            {
                dgvCompras.DataSource = null;
                dgvCompras.DataSource =
                    CompraCln.listar(txtParametro.Text.Trim());

                if (dgvCompras.Columns.Count > 0)
                {
                    if (dgvCompras.Columns.Contains("id"))
                        dgvCompras.Columns["id"].HeaderText = "N.º Compra";

                    if (dgvCompras.Columns.Contains("idProveedor"))
                        dgvCompras.Columns["idProveedor"].Visible = false;

                    if (dgvCompras.Columns.Contains("idUsuario"))
                        dgvCompras.Columns["idUsuario"].Visible = false;

                    if (dgvCompras.Columns.Contains("fecha"))
                    {
                        dgvCompras.Columns["fecha"].HeaderText = "Fecha";
                        dgvCompras.Columns["fecha"].DefaultCellStyle.Format =
                            "dd/MM/yyyy HH:mm";
                    }

                    if (dgvCompras.Columns.Contains("numeroDocumento"))
                        dgvCompras.Columns["numeroDocumento"].HeaderText =
                            "N.º Documento";

                    if (dgvCompras.Columns.Contains("tipoDocumento"))
                        dgvCompras.Columns["tipoDocumento"].HeaderText =
                            "Tipo Documento";

                    if (dgvCompras.Columns.Contains("proveedor"))
                        dgvCompras.Columns["proveedor"].HeaderText =
                            "Proveedor";

                    if (dgvCompras.Columns.Contains("usuarioRegistro"))
                        dgvCompras.Columns["usuarioRegistro"].HeaderText =
                            "Registrado por";

                    if (dgvCompras.Columns.Contains("subtotal"))
                    {
                        dgvCompras.Columns["subtotal"].HeaderText =
                            "Subtotal (Bs.)";
                        dgvCompras.Columns["subtotal"].DefaultCellStyle.Format =
                            "N2";
                    }

                    if (dgvCompras.Columns.Contains("descuento"))
                    {
                        dgvCompras.Columns["descuento"].HeaderText =
                            "Descuento (Bs.)";
                        dgvCompras.Columns["descuento"].DefaultCellStyle.Format =
                            "N2";
                    }

                    if (dgvCompras.Columns.Contains("total"))
                    {
                        dgvCompras.Columns["total"].HeaderText =
                            "Total (Bs.)";
                        dgvCompras.Columns["total"].DefaultCellStyle.Format =
                            "N2";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al listar las compras: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // BUSCAR
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            listar();
        }

        // NUEVA COMPRA
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            if (IdUsuarioActual <= 0 ||
                string.IsNullOrWhiteSpace(UsuarioActual))
            {
                MessageBox.Show(
                    "No se pudo identificar al usuario autenticado.");
                return;
            }

            using (FrmCompra formulario = new FrmCompra())
            {
                formulario.IdUsuarioActual = IdUsuarioActual;
                formulario.UsuarioActual = UsuarioActual;

                formulario.ShowDialog(this);
            }

            listar();
        }

        // OBTENER COMPRA SELECCIONADA
        private CompraListarDto obtenerCompraSeleccionada()
        {
            if (dgvCompras.CurrentRow == null)
            {
                MessageBox.Show(
                    "Seleccione una compra.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return null;
            }

            return dgvCompras.CurrentRow.DataBoundItem
                as CompraListarDto;
        }

        // MODIFICAR COMPRA
        private void btnModificar_Click(object sender, EventArgs e)
        {
            CompraListarDto compra = obtenerCompraSeleccionada();

            if (compra == null)
                return;

            if (IdUsuarioActual <= 0 ||
                string.IsNullOrWhiteSpace(UsuarioActual))
            {
                MessageBox.Show(
                    "No se pudo identificar al usuario autenticado.");
                return;
            }

            try
            {
                using (FrmCompra formulario = new FrmCompra(compra.id))
                {
                    formulario.IdUsuarioActual = IdUsuarioActual;
                    formulario.UsuarioActual = UsuarioActual;

                    formulario.ShowDialog(this);
                }

                listar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al abrir la compra para modificar: " +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ANULAR COMPRA
        private void btnAnular_Click(object sender, EventArgs e)
        {
            CompraListarDto compra = obtenerCompraSeleccionada();

            if (compra == null)
                return;

            if (IdUsuarioActual <= 0 ||
                string.IsNullOrWhiteSpace(UsuarioActual))
            {
                MessageBox.Show(
                    "No se pudo identificar al usuario autenticado.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de anular esta compra?" +
                "\n\nNúmero de compra: " + compra.id +
                "\nProveedor: " + compra.proveedor +
                "\nTotal: Bs. " + compra.total.ToString("N2") +
                "\n\nSe revertirá el stock de los productos.",
                "Confirmar anulación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                CompraCln.anular(compra.id, UsuarioActual);

                MessageBox.Show(
                    "La compra fue anulada correctamente.",
                    "Compra anulada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                listar();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al anular la compra:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // CANCELAR BÚSQUEDA
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            txtParametro.Clear();
            listar();
        }

        // VOLVER
        private void btnVolver_Click(object sender, EventArgs e)
        {
            Close();
        }

        // BUSCAR CON ENTER
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