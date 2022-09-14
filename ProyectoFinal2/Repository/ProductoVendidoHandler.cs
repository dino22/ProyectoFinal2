using ProyectoFinal2.Model;
using System.Data.SqlClient;
using System.Data;
using ProyectoFinal2.DTO;

namespace ProyectoFinal2.Repository
{
    public static class ProductoVendidoHandler
    {
        public const string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=SistemaGestion;Trusted_Connection=True;";
        //Método para traer una lista de ProductosVendidos por id de Usuario
        public static List<GetProductoVendido> TraerProductosVendidos(int idUsuario)
        {

            List<GetProductoVendido> productos = new List<GetProductoVendido>();

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
                        GetProductoVendido producto = new DTO.GetProductoVendido();
                        producto.IdProducto = Convert.ToInt32(row["IdProducto"]);
                        producto.Stock = Convert.ToInt32(row["Stock"]);
                        producto.IdUsuario = Convert.ToInt32(row["IdUsuario"]);
                        producto.Descripciones = row["Descripciones"].ToString();
                        producto.PrecioVenta = Convert.ToDouble(row["PrecioVenta"]);

                        productos.Add(producto);
                    }
                    sqlConnection.Close();
                }
            }
            return productos;
        }
    }
}
