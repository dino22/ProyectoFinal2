using ProyectoFinal2.DTO;
using ProyectoFinal2.Model;
using System.Data;
using System.Data.SqlClient;

namespace ProyectoFinal2.Repository
{
    public static class ProductoHandler
    {
        public const string connectionString = "Server=localhost\\SQLEXPRESS;Database=SistemaGestion;Trusted_Connection=True;";
        //Método para traer una lista de Productos
        public static List<Producto> TraerProductos()
        {

            List<Producto> productos = new List<Producto>();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Producto", sqlConnection))
                {
                    sqlConnection.Open();

                    SqlDataAdapter sqlAdapter = new SqlDataAdapter();
                    sqlAdapter.SelectCommand = sqlCommand;

                    DataTable table = new DataTable();
                    sqlAdapter.Fill(table);

                    foreach (DataRow row in table.Rows)
                    {
                        Producto producto = new Producto();
                        producto.Id = Convert.ToInt32(row["Id"]);
                        producto.Descripciones = row["Descripciones"].ToString();
                        producto.Costo = Convert.ToDouble(row["Costo"]);
                        producto.PrecioVenta = Convert.ToDouble(row["PrecioVenta"]);
                        producto.Stock = Convert.ToInt32(row["Stock"]);
                        producto.IdUsuario = Convert.ToInt32(row["IdUsuario"]);
                        productos.Add(producto);
                    }
                    sqlConnection.Close();
                }
            }
            return productos;
        }
        //Método para agregar un nuevo Producto por Usuario si no existe
        public static List<PostProducto> AgregarProducto(List<PostProducto> Producto)
        {
            DataTable dtUsuarios = new DataTable();
            DataRow[] querySelect;
            string query = string.Empty;
            int cont = -1;

            dtUsuarios = TraerIdUsuarios();

            foreach (var row in Producto)
            {
                cont++;

                query = "Id = " + row.IdUsuario.ToString();
                querySelect = dtUsuarios.Select(query);

                if (querySelect.Length == 0)
                {
                    Producto[cont].Estado = "El producto: " + row.Descripciones + " no se ha registrado. No existe el Id de Usuario";
                }

                Producto[cont].Estado = AgregarProductoTable(row);
            }
            return Producto;
        }
        //Método para modificar un registro de Producto
        public static string ModificarProducto(Producto producto)
        {
            string resultado = String.Empty;
            int filasAfectadas = 0;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string queryUpdate = "UPDATE Producto SET Descripciones = @Descripciones, Costo = @Costo, PrecioVenta = @PrecioVenta, Stock = @Stock, IdUsuario = @IdUsuario WHERE Id = @IdProducto";

                    SqlParameter idProductoParameter = new SqlParameter("IdProducto", System.Data.SqlDbType.BigInt) { Value = producto.Id };
                    SqlParameter descripcionesParameter = new SqlParameter("Descripciones", SqlDbType.VarChar) { Value = producto.Descripciones };
                    SqlParameter costoParameter = new SqlParameter("Costo", System.Data.SqlDbType.Money) { Value = producto.Costo };
                    SqlParameter precioVentaParameter = new SqlParameter("PrecioVenta", System.Data.SqlDbType.Money) { Value = producto.PrecioVenta };
                    SqlParameter stockParameter = new SqlParameter("Stock", System.Data.SqlDbType.Int) { Value = producto.Stock };
                    SqlParameter idUsuarioParameter = new SqlParameter("IdUsuario", System.Data.SqlDbType.BigInt) { Value = producto.IdUsuario };

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(queryUpdate, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(idProductoParameter);
                        sqlCommand.Parameters.Add(descripcionesParameter);
                        sqlCommand.Parameters.Add(costoParameter);
                        sqlCommand.Parameters.Add(precioVentaParameter);
                        sqlCommand.Parameters.Add(stockParameter);
                        sqlCommand.Parameters.Add(idUsuarioParameter);
                        filasAfectadas = sqlCommand.ExecuteNonQuery();
                    }
                    if (filasAfectadas == 1)
                    {
                        resultado = "Se ha actualizado el Id de Producto: " + producto.Descripciones;
                    }
                    else
                    {
                        resultado = "No se ha actualizado información para el Id de Producto: " + producto.Descripciones;
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                resultado = "Error al actualizar Id de Producto: " + producto.Id + " - Error: " + ex.Message;
            }
            return resultado;
        }
        //Método para eliminar un registro de Producto
        public static string EliminarProducto(int idProducto)
        {
            string resultado = String.Empty;
            int filasAfectadas = 0;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string queryDeleteProductoVendido = "DELETE FROM ProductoVendido WHERE IdProducto = @idProducto";

                    SqlParameter idProductoParameter = new SqlParameter("idProducto", System.Data.SqlDbType.BigInt) { Value = idProducto };

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(queryDeleteProductoVendido, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(idProductoParameter);
                        filasAfectadas = sqlCommand.ExecuteNonQuery();
                        sqlCommand.Parameters.Clear();

                        if (filasAfectadas > 0)
                        {
                            string queryDeleteProducto = "DELETE FROM Producto WHERE Id = @idProducto";

                            using (SqlCommand sqlCommand1 = new SqlCommand(queryDeleteProducto, sqlConnection))
                            {
                                sqlCommand1.Parameters.Add(idProductoParameter);
                                sqlCommand1.ExecuteNonQuery();
                            }
                            resultado = "El producto: " + idProducto + " ha sido eliminado.";
                        }
                        else
                        {
                            resultado = "El producto: " + idProducto + " no ha sido eliminado.";
                        }
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                resultado = "El producto: " + idProducto + " no ha sido eliminado - Error: " + ex.Message;
            }
            return resultado;
        }
        //Método para traer los id de Usuarios
        private static DataTable TraerIdUsuarios()
        {
            DataTable dtId = new DataTable();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                SqlDataAdapter SqlAdapter = new SqlDataAdapter("SELECT Id FROM Usuario", sqlConnection);
                sqlConnection.Open();

                SqlAdapter.Fill(dtId);
                sqlConnection.Close();
            }
            return dtId;
        }
        //Método para agregar Productos
        private static string AgregarProductoTable(PostProducto producto)
        {
            string Estado = string.Empty;
            int filasAfectadas = 0;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string queryInsert = "INSERT INTO Producto (Descripciones, Costo, PrecioVenta, Stock, IdUsuario) VALUES (@Descripciones, @Costo, @PrecioVenta, @Stock, @IdUsuario)";

                    SqlParameter descripcionesParameter = new SqlParameter("Descripciones", SqlDbType.VarChar) { Value = producto.Descripciones };
                    SqlParameter costoParameter = new SqlParameter("Costo", SqlDbType.Decimal) { Value = producto.Costo };
                    SqlParameter precioVentaParameter = new SqlParameter("PrecioVenta", SqlDbType.Decimal) { Value = producto.PrecioVenta };
                    SqlParameter stockParameter = new SqlParameter("Stock", SqlDbType.Int) { Value = producto.Stock };
                    SqlParameter idUsuarioParameter = new SqlParameter("IdUsuario", SqlDbType.Int) { Value = producto.IdUsuario };

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(queryInsert, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(descripcionesParameter);
                        sqlCommand.Parameters.Add(costoParameter);
                        sqlCommand.Parameters.Add(precioVentaParameter);
                        sqlCommand.Parameters.Add(stockParameter);
                        sqlCommand.Parameters.Add(idUsuarioParameter);
                        filasAfectadas = sqlCommand.ExecuteNonQuery();
                        sqlCommand.Parameters.Clear();
                    }
                    if (filasAfectadas == 1)
                    {
                        Estado = "Producto: " + producto.Descripciones + " registrado correctamente.";
                    }
                    else
                    {
                        Estado = "Producto: " + producto.Descripciones + " no se ha registrado.";
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Estado = "Producto: " + producto.Descripciones + " no se ha registrado. - Error: " + ex.Message;
            }
            return Estado;
        }
    }
}
