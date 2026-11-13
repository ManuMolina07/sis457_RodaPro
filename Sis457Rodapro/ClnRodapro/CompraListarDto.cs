using System;

namespace ClnRodaPro
{
    public class CompraListarDto
    {
        public long id { get; set; }
        public int idProveedor { get; set; }
        public int idUsuario { get; set; }
        public DateTime fecha { get; set; }
        public string numeroDocumento { get; set; }
        public string tipoDocumento { get; set; }
        public string proveedor { get; set; }
        public string usuarioRegistro { get; set; }
        public decimal subtotal { get; set; }
        public decimal descuento { get; set; }
        public decimal total { get; set; }
    }
}