using ProyectoFinal2.Model;
using System.Data.SqlClient;
using System.Data;

namespace ProyectoFinal2.Repository
{
    public static class ProductoVendidoHandler
    {
        public const string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=SistemaGestion;Trusted_Connection=True;";

        public static List<ProductoVendido> TraerProductosVendidos(int idUsuario)
        {

            List<ProductoVendido> productos = new List<ProductoVendido>();

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT P.IdUsuario, PV.IdProducto, P.Descripciones, PV.Stock, P.PrecioVenta FROM ProductoVendido PV INNER JOIN Producto P ON PV.IdProducto = P.Id WHERE P.IdUsuario = @IdUsuario", sqlConnection))
                {
                    sqlConnection.Open();

                    sqlCommand.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    SqlDataAdapter sqlAdapter = new SqlDataAdapter();
                    sqlAdapter.SelectCommand = sqlCommand;
        
                    DataTable table = new DataTable();
                    sqlAdapter.Fill(table);

                    foreach (DataRow row in table.Rows)
                    {
                        ProductoVendido productoVendido = new ProductoVendido();
                        productoVendido.IdProducto = Convert.ToInt32(row["IdProducto"]);
                        productoVendido.Stock = Convert.ToInt32(row["Stock"]);

                        Producto producto = new Producto();
                        producto.IdUsuario = Convert.ToInt32(row["IdUsuario"]);
                        producto.Descripciones = row["Descripciones"].ToString();
                        producto.PrecioVenta = Convert.ToDouble(row["PrecioVenta"]);
                    }
                    sqlConnection.Close();
                }
            }
            return productos;
        }
    }
}
