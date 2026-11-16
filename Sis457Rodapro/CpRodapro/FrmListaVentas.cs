using ClnRodaPro;
using System;
using System.Windows.Forms;

namespace CpRodapro
{
    public partial class FrmListaVentas : Form
    {
        public int IdUsuarioActual { get; set; }
        public string UsuarioActual { get; set; }

        public FrmListaVentas()
        {
            InitializeComponent();

            Load += FrmListaVentas_Load;

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

        // CARGAR FORMULARIO
        private void FrmListaVentas_Load(object sender, EventArgs e)
        {
            ConfigurarGrilla();
            listarVentas();
        }

        // CONFIGURAR DATAGRIDVIEW
        private void ConfigurarGrilla()
        {
            dgvLista.AutoGenerateColumns = true;

            dgvLista.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvLista.MultiSelect = false;
            dgvLista.ReadOnly = true;
            dgvLista.AllowUserToAddRows = false;

            dgvLista.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }

        // LISTAR VENTAS
        private void listarVentas()
        {
            try
            {
                dgvLista.DataSource = null;

                dgvLista.DataSource =
                    VentaCln.listar(txtParametro.Text.Trim());

                ConfigurarColumnas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al listar las ventas: " + ex.Message,
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // CONFIGURAR COLUMNAS
        private void ConfigurarColumnas()
        {
            CambiarEncabezado("id", "N.º Venta");
            CambiarEncabezado("numeroDocumento", "N.º Documento");
            CambiarEncabezado("tipoDocumento", "Tipo Documento");
            CambiarEncabezado("fecha", "Fecha");
            CambiarEncabezado("cliente", "Cliente");
            CambiarEncabezado("usuarioRegistro", "Registrado por");
            CambiarEncabezado("subtotal", "Subtotal (Bs.)");
            CambiarEncabezado("descuento", "Descuento (Bs.)");
            CambiarEncabezado("total", "Total (Bs.)");

            OcultarColumna("idCliente");
            OcultarColumna("idUsuario");

            ConfigurarFormato("subtotal", "N2");
            ConfigurarFormato("descuento", "N2");
            ConfigurarFormato("total", "N2");
            ConfigurarFormato("fecha", "dd/MM/yyyy HH:mm");
        }

        // CAMBIAR ENCABEZADO
        private void CambiarEncabezado(
            string columna, string encabezado)
        {
            if (dgvLista.Columns.Contains(columna))
            {
                dgvLista.Columns[columna].HeaderText = encabezado;
            }
        }

        // OCULTAR COLUMNA
        private void OcultarColumna(string columna)
        {
            if (dgvLista.Columns.Contains(columna))
            {
                dgvLista.Columns[columna].Visible = false;
            }
        }

        // CONFIGURAR FORMATO
        private void ConfigurarFormato(
            string columna, string formato)
        {
            if (dgvLista.Columns.Contains(columna))
            {
                dgvLista.Columns[columna]
                    .DefaultCellStyle.Format = formato;
            }
        }

        // VALIDAR USUARIO AUTENTICADO
        private bool validarUsuarioActual()
        {
            if (IdUsuarioActual <= 0 ||
                string.IsNullOrWhiteSpace(UsuarioActual))
            {
                MessageBox.Show(
                    "No se pudo identificar al usuario autenticado.",
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            return true;
        }

        // OBTENER ID DE VENTA SELECCIONADA
        private long? obtenerIdVentaSeleccionada()
        {
            if (dgvLista.CurrentRow == null ||
                !dgvLista.Columns.Contains("id") ||
                dgvLista.CurrentRow.Cells["id"].Value == null)
            {
                MessageBox.Show(
                    "Seleccione una venta.",
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return null;
            }

            return Convert.ToInt64(
                dgvLista.CurrentRow.Cells["id"].Value);
        }

        // BUSCAR AL PRESIONAR ENTER
        private void txtParametro_KeyDown(
            object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnBuscar.PerformClick();
            }
        }

        // BUSCAR VENTAS
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            listarVentas();
        }

        // NUEVA VENTA
        private void btnNuevo_Click(object sender, EventArgs e)
        {
            if (!validarUsuarioActual())
                return;

            using (FrmVenta formulario = new FrmVenta())
            {
                formulario.IdUsuarioActual = IdUsuarioActual;
                formulario.UsuarioActual = UsuarioActual;
                formulario.ModoEdicion = false;

                formulario.ShowDialog(this);
            }

            listarVentas();
        }

        // MODIFICAR VENTA
        private void btnModificar_Click(object sender, EventArgs e)
        {
            long? idVenta = obtenerIdVentaSeleccionada();

            if (!idVenta.HasValue)
                return;

            if (!validarUsuarioActual())
                return;

            try
            {
                using (FrmVenta formulario = new FrmVenta())
                {
                    formulario.IdUsuarioActual = IdUsuarioActual;
                    formulario.UsuarioActual = UsuarioActual;

                    formulario.ModoEdicion = true;
                    formulario.IdVentaEditar = idVenta.Value;

                    // FrmListaVentas será el propietario.
                    formulario.ShowDialog(this);
                }

                // Al cerrar la edición, se actualiza la lista.
                listarVentas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al abrir la venta para modificar: " +
                    ex.Message,
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ANULAR VENTA
        private void btnAnular_Click(object sender, EventArgs e)
        {
            long? idVenta = obtenerIdVentaSeleccionada();

            if (!idVenta.HasValue)
                return;

            if (!validarUsuarioActual())
                return;

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de que desea anular esta venta?\n\n" +
                "N.º de venta: " + idVenta.Value,
                "Confirmar anulación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (respuesta != DialogResult.Yes)
                return;

            try
            {
                VentaCln.anular(idVenta.Value, UsuarioActual);

                MessageBox.Show(
                    "La venta fue anulada correctamente.",
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                listarVentas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo anular la venta: " + ex.Message,
                    "RodaPro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // CANCELAR BÚSQUEDA
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            txtParametro.Clear();
            listarVentas();
        }

        // VOLVER
        private void btnVolver_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}