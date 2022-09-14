namespace ProyectoFinal2.DTO
{
    public class GetProductoVendido
    {
        public int IdProducto { get; set; }
        public int Stock { get; set; }
        public int IdUsuario { get; set; }
        public string Descripciones { get; set; }
        public double PrecioVenta { get; set; }

    }
}
