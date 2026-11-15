namespace CpRodapro
{
    partial class FrmVenta
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmVenta));
            this.cbxCliente = new System.Windows.Forms.ComboBox();
            this.lblCliente = new System.Windows.Forms.Label();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.lblTipoDocumento = new System.Windows.Forms.Label();
            this.lblFecha = new System.Windows.Forms.Label();
            this.cbxTipoDocumento = new System.Windows.Forms.ComboBox();
            this.txtNumeroDocumento = new System.Windows.Forms.TextBox();
            this.lblNumeroDocumento = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.cbxProducto = new System.Windows.Forms.ComboBox();
            this.lblProducto = new System.Windows.Forms.Label();
            this.nudCantidad = new System.Windows.Forms.NumericUpDown();
            this.lblCantidad = new System.Windows.Forms.Label();
            this.txtPrecio = new System.Windows.Forms.TextBox();
            this.lblPrecio = new System.Windows.Forms.Label();
            this.btnAgregarProducto = new System.Windows.Forms.Button();
            this.txtDescuentoDetalle = new System.Windows.Forms.TextBox();
            this.lblDescuentoDetalle = new System.Windows.Forms.Label();
            this.btnQuitarProducto = new System.Windows.Forms.Button();
            this.dgvDetalle = new System.Windows.Forms.DataGridView();
            this.lblDescuento = new System.Windows.Forms.Label();
            this.txtDescuento = new System.Windows.Forms.TextBox();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblDescuentoGeneral = new System.Windows.Forms.Label();
            this.btnVolver = new System.Windows.Forms.Button();
            this.btnVerVentas = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).BeginInit();
            this.SuspendLayout();
            // 
            // cbxCliente
            // 
            this.cbxCliente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCliente.FormattingEnabled = true;
            this.cbxCliente.Location = new System.Drawing.Point(195, 72);
            this.cbxCliente.Name = "cbxCliente";
            this.cbxCliente.Size = new System.Drawing.Size(275, 21);
            this.cbxCliente.TabIndex = 10;
            // 
            // lblCliente
            // 
            this.lblCliente.AutoSize = true;
            this.lblCliente.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCliente.Location = new System.Drawing.Point(15, 75);
            this.lblCliente.Name = "lblCliente";
            this.lblCliente.Size = new System.Drawing.Size(57, 18);
            this.lblCliente.TabIndex = 9;
            this.lblCliente.Text = "Cliente:";
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.Location = new System.Drawing.Point(25, 24);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(218, 32);
            this.lblTitulo.TabIndex = 11;
            this.lblTitulo.Text = "Gestión de Ventas";
            // 
            // lblTipoDocumento
            // 
            this.lblTipoDocumento.AutoSize = true;
            this.lblTipoDocumento.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTipoDocumento.Location = new System.Drawing.Point(16, 112);
            this.lblTipoDocumento.Name = "lblTipoDocumento";
            this.lblTipoDocumento.Size = new System.Drawing.Size(143, 18);
            this.lblTipoDocumento.TabIndex = 12;
            this.lblTipoDocumento.Text = "Tipo de Documento:";
            // 
            // lblFecha
            // 
            this.lblFecha.AutoSize = true;
            this.lblFecha.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFecha.Location = new System.Drawing.Point(503, 75);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(53, 18);
            this.lblFecha.TabIndex = 26;
            this.lblFecha.Text = "Fecha:";
            // 
            // cbxTipoDocumento
            // 
            this.cbxTipoDocumento.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxTipoDocumento.FormattingEnabled = true;
            this.cbxTipoDocumento.Location = new System.Drawing.Point(195, 113);
            this.cbxTipoDocumento.Name = "cbxTipoDocumento";
            this.cbxTipoDocumento.Size = new System.Drawing.Size(275, 21);
            this.cbxTipoDocumento.TabIndex = 28;
            // 
            // txtNumeroDocumento
            // 
            this.txtNumeroDocumento.Location = new System.Drawing.Point(195, 152);
            this.txtNumeroDocumento.MaxLength = 50;
            this.txtNumeroDocumento.Name = "txtNumeroDocumento";
            this.txtNumeroDocumento.Size = new System.Drawing.Size(275, 20);
            this.txtNumeroDocumento.TabIndex = 47;
            // 
            // lblNumeroDocumento
            // 
            this.lblNumeroDocumento.AutoSize = true;
            this.lblNumeroDocumento.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumeroDocumento.Location = new System.Drawing.Point(15, 154);
            this.lblNumeroDocumento.Name = "lblNumeroDocumento";
            this.lblNumeroDocumento.Size = new System.Drawing.Size(168, 18);
            this.lblNumeroDocumento.TabIndex = 46;
            this.lblNumeroDocumento.Text = "Número de Documento:";
            // 
            // dtpFecha
            // 
            this.dtpFecha.Location = new System.Drawing.Point(569, 73);
            this.dtpFecha.Name = "dtpFecha";
            this.dtpFecha.Size = new System.Drawing.Size(288, 20);
            this.dtpFecha.TabIndex = 48;
            // 
            // cbxProducto
            // 
            this.cbxProducto.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxProducto.FormattingEnabled = true;
            this.cbxProducto.Location = new System.Drawing.Point(195, 189);
            this.cbxProducto.Name = "cbxProducto";
            this.cbxProducto.Size = new System.Drawing.Size(275, 21);
            this.cbxProducto.TabIndex = 50;
            // 
            // lblProducto
            // 
            this.lblProducto.AutoSize = true;
            this.lblProducto.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProducto.Location = new System.Drawing.Point(16, 192);
            this.lblProducto.Name = "lblProducto";
            this.lblProducto.Size = new System.Drawing.Size(73, 18);
            this.lblProducto.TabIndex = 49;
            this.lblProducto.Text = "Producto:";
            // 
            // nudCantidad
            // 
            this.nudCantidad.Location = new System.Drawing.Point(195, 234);
            this.nudCantidad.Name = "nudCantidad";
            this.nudCantidad.Size = new System.Drawing.Size(275, 20);
            this.nudCantidad.TabIndex = 51;
            // 
            // lblCantidad
            // 
            this.lblCantidad.AutoSize = true;
            this.lblCantidad.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCantidad.Location = new System.Drawing.Point(16, 234);
            this.lblCantidad.Name = "lblCantidad";
            this.lblCantidad.Size = new System.Drawing.Size(70, 18);
            this.lblCantidad.TabIndex = 52;
            this.lblCantidad.Text = "Cantidad:";
            // 
            // txtPrecio
            // 
            this.txtPrecio.Location = new System.Drawing.Point(680, 114);
            this.txtPrecio.MaxLength = 50;
            this.txtPrecio.Name = "txtPrecio";
            this.txtPrecio.Size = new System.Drawing.Size(177, 20);
            this.txtPrecio.TabIndex = 53;
            // 
            // lblPrecio
            // 
            this.lblPrecio.AutoSize = true;
            this.lblPrecio.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrecio.Location = new System.Drawing.Point(501, 113);
            this.lblPrecio.Name = "lblPrecio";
            this.lblPrecio.Size = new System.Drawing.Size(55, 18);
            this.lblPrecio.TabIndex = 54;
            this.lblPrecio.Text = "Precio:";
            // 
            // btnAgregarProducto
            // 
            this.btnAgregarProducto.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnAgregarProducto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAgregarProducto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAgregarProducto.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgregarProducto.Image = global::CpRodapro.Properties.Resources.anadir;
            this.btnAgregarProducto.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAgregarProducto.Location = new System.Drawing.Point(506, 450);
            this.btnAgregarProducto.Name = "btnAgregarProducto";
            this.btnAgregarProducto.Size = new System.Drawing.Size(110, 56);
            this.btnAgregarProducto.TabIndex = 20;
            this.btnAgregarProducto.Text = "Agregar Producto";
            this.btnAgregarProducto.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnAgregarProducto.UseVisualStyleBackColor = false;
            // 
            // txtDescuentoDetalle
            // 
            this.txtDescuentoDetalle.Location = new System.Drawing.Point(680, 152);
            this.txtDescuentoDetalle.MaxLength = 50;
            this.txtDescuentoDetalle.Name = "txtDescuentoDetalle";
            this.txtDescuentoDetalle.Size = new System.Drawing.Size(177, 20);
            this.txtDescuentoDetalle.TabIndex = 55;
            // 
            // lblDescuentoDetalle
            // 
            this.lblDescuentoDetalle.AutoSize = true;
            this.lblDescuentoDetalle.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescuentoDetalle.Location = new System.Drawing.Point(500, 154);
            this.lblDescuentoDetalle.Name = "lblDescuentoDetalle";
            this.lblDescuentoDetalle.Size = new System.Drawing.Size(133, 18);
            this.lblDescuentoDetalle.TabIndex = 56;
            this.lblDescuentoDetalle.Text = "Descuento Detalle:";
            // 
            // btnQuitarProducto
            // 
            this.btnQuitarProducto.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnQuitarProducto.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnQuitarProducto.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuitarProducto.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnQuitarProducto.Image = global::CpRodapro.Properties.Resources.eliminar;
            this.btnQuitarProducto.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnQuitarProducto.Location = new System.Drawing.Point(622, 450);
            this.btnQuitarProducto.Name = "btnQuitarProducto";
            this.btnQuitarProducto.Size = new System.Drawing.Size(110, 56);
            this.btnQuitarProducto.TabIndex = 57;
            this.btnQuitarProducto.Text = "Quitar Producto";
            this.btnQuitarProducto.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnQuitarProducto.UseVisualStyleBackColor = false;
            // 
            // dgvDetalle
            // 
            this.dgvDetalle.AllowUserToAddRows = false;
            this.dgvDetalle.AllowUserToDeleteRows = false;
            this.dgvDetalle.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDetalle.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalle.Location = new System.Drawing.Point(195, 260);
            this.dgvDetalle.MultiSelect = false;
            this.dgvDetalle.Name = "dgvDetalle";
            this.dgvDetalle.ReadOnly = true;
            this.dgvDetalle.RowHeadersVisible = false;
            this.dgvDetalle.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetalle.Size = new System.Drawing.Size(667, 168);
            this.dgvDetalle.TabIndex = 58;
            // 
            // lblDescuento
            // 
            this.lblDescuento.AutoSize = true;
            this.lblDescuento.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescuento.Location = new System.Drawing.Point(16, 276);
            this.lblDescuento.Name = "lblDescuento";
            this.lblDescuento.Size = new System.Drawing.Size(84, 18);
            this.lblDescuento.TabIndex = 60;
            this.lblDescuento.Text = "Descuento:";
            // 
            // txtDescuento
            // 
            this.txtDescuento.Location = new System.Drawing.Point(680, 192);
            this.txtDescuento.MaxLength = 50;
            this.txtDescuento.Name = "txtDescuento";
            this.txtDescuento.Size = new System.Drawing.Size(177, 20);
            this.txtDescuento.TabIndex = 59;
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.AutoSize = true;
            this.lblSubtotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubtotal.Location = new System.Drawing.Point(14, 318);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(66, 18);
            this.lblSubtotal.TabIndex = 61;
            this.lblSubtotal.Text = "Subtotal:";
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.Location = new System.Drawing.Point(16, 354);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(45, 18);
            this.lblTotal.TabIndex = 62;
            this.lblTotal.Text = "Total:";
            // 
            // lblDescuentoGeneral
            // 
            this.lblDescuentoGeneral.AutoSize = true;
            this.lblDescuentoGeneral.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescuentoGeneral.Location = new System.Drawing.Point(500, 194);
            this.lblDescuentoGeneral.Name = "lblDescuentoGeneral";
            this.lblDescuentoGeneral.Size = new System.Drawing.Size(140, 18);
            this.lblDescuentoGeneral.TabIndex = 63;
            this.lblDescuentoGeneral.Text = "Descuento General:";
            // 
            // btnVolver
            // 
            this.btnVolver.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnVolver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVolver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVolver.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVolver.Image = global::CpRodapro.Properties.Resources.hogar;
            this.btnVolver.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVolver.Location = new System.Drawing.Point(738, 450);
            this.btnVolver.Name = "btnVolver";
            this.btnVolver.Size = new System.Drawing.Size(110, 56);
            this.btnVolver.TabIndex = 64;
            this.btnVolver.Text = "Volver";
            this.btnVolver.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVolver.UseVisualStyleBackColor = false;
            // 
            // btnVerVentas
            // 
            this.btnVerVentas.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnVerVentas.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVerVentas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerVentas.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerVentas.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVerVentas.Location = new System.Drawing.Point(249, 449);
            this.btnVerVentas.Name = "btnVerVentas";
            this.btnVerVentas.Size = new System.Drawing.Size(110, 57);
            this.btnVerVentas.TabIndex = 65;
            this.btnVerVentas.Text = "Ver Ventas Registradas ";
            this.btnVerVentas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVerVentas.UseVisualStyleBackColor = false;
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.Image = global::CpRodapro.Properties.Resources.guardar_el_archivo;
            this.btnGuardar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGuardar.Location = new System.Drawing.Point(378, 449);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(110, 57);
            this.btnGuardar.TabIndex = 66;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGuardar.UseVisualStyleBackColor = false;
            // 
            // FrmVenta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(884, 561);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnVerVentas);
            this.Controls.Add(this.btnVolver);
            this.Controls.Add(this.lblDescuentoGeneral);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblSubtotal);
            this.Controls.Add(this.lblDescuento);
            this.Controls.Add(this.txtDescuento);
            this.Controls.Add(this.dgvDetalle);
            this.Controls.Add(this.btnQuitarProducto);
            this.Controls.Add(this.lblDescuentoDetalle);
            this.Controls.Add(this.txtDescuentoDetalle);
            this.Controls.Add(this.btnAgregarProducto);
            this.Controls.Add(this.lblPrecio);
            this.Controls.Add(this.txtPrecio);
            this.Controls.Add(this.lblCantidad);
            this.Controls.Add(this.nudCantidad);
            this.Controls.Add(this.cbxProducto);
            this.Controls.Add(this.lblProducto);
            this.Controls.Add(this.dtpFecha);
            this.Controls.Add(this.txtNumeroDocumento);
            this.Controls.Add(this.lblNumeroDocumento);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.cbxTipoDocumento);
            this.Controls.Add(this.lblCliente);
            this.Controls.Add(this.lblFecha);
            this.Controls.Add(this.cbxCliente);
            this.Controls.Add(this.lblTipoDocumento);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmVenta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "::: Gestión de Ventas - RodaPro :::";
            ((System.ComponentModel.ISupportInitialize)(this.nudCantidad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxCliente;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Label lblTipoDocumento;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.TextBox txtNumeroDocumento;
        private System.Windows.Forms.Label lblNumeroDocumento;
        private System.Windows.Forms.ComboBox cbxTipoDocumento;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.ComboBox cbxProducto;
        private System.Windows.Forms.Label lblProducto;
        private System.Windows.Forms.NumericUpDown nudCantidad;
        private System.Windows.Forms.Label lblCantidad;
        private System.Windows.Forms.TextBox txtPrecio;
        private System.Windows.Forms.Label lblPrecio;
        private System.Windows.Forms.Button btnAgregarProducto;
        private System.Windows.Forms.TextBox txtDescuentoDetalle;
        private System.Windows.Forms.Label lblDescuentoDetalle;
        private System.Windows.Forms.Button btnQuitarProducto;
        private System.Windows.Forms.DataGridView dgvDetalle;
        private System.Windows.Forms.Label lblDescuento;
        private System.Windows.Forms.TextBox txtDescuento;
        private System.Windows.Forms.Label lblSubtotal;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblDescuentoGeneral;
        private System.Windows.Forms.Button btnVolver;
        private System.Windows.Forms.Button btnVerVentas;
        private System.Windows.Forms.Button btnGuardar;
    }
}