using ProyectoFinal2.Model;
using System.Data.SqlClient;
using System.Data;
using ProyectoFinal2.DTO;

namespace ProyectoFinal2.Repository
{
    public static class VentaHandler
    {
        public const string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=SistemaGestion;Trusted_Connection=True;";
        ////Método para traer una lista de Ventas por un id de Usuario
        public static List<DTO.GetVenta> TraerVentas(int IdUsuario)
        {
            List<DTO.GetVenta> Ventas = new List<DTO.GetVenta>();

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                string querySelect = "SELECT U.NombreUsuario, PV.Stock, P.Descripciones, P.Costo, P.PrecioVenta, V.Comentarios FROM ProductoVendido PV INNER JOIN Producto P ON P.Id = PV.IdProducto INNER JOIN Usuario U ON U.Id = P.IdUsuario INNER JOIN Venta V ON V.Id = PV.IdVenta WHERE U.Id = @idUsuario";

                SqlParameter idUsuarioParameter = new SqlParameter("IdUsuario", System.Data.SqlDbType.BigInt) { Value = IdUsuario };

                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(querySelect, sqlConnection))
                {
                    sqlCommand.Parameters.Add(idUsuarioParameter);

                    using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                    {
                        if (dataReader.HasRows)
                        {
                            while (dataReader.Read())
                            {
                                DTO.GetVenta venta = new DTO.GetVenta();
                                venta.Comentarios = dataReader["Comentarios"].ToString();
                                venta.Descripciones = dataReader["Descripciones"].ToString();
                                venta.Costo = Convert.ToDouble(dataReader["Costo"]);
                                venta.PrecioVenta = Convert.ToDouble(dataReader["PrecioVenta"]);
                                venta.NombreUsuario = dataReader["NombreUsuario"].ToString();
                                venta.Stock = Convert.ToInt32(dataReader["Stock"]);

                                Ventas.Add(venta);
                            }
                        }
                    }

                    sqlConnection.Close();
                }
            }
            return Ventas;
        }
        //Método para agregar un registro de Venta
        public static List<PostVenta> AgregarVentas(List<PostVenta> Venta)
        {
            DataTable dtProductos = new DataTable();
            DataTable dtUsuarios = new DataTable();
            DataRow[] querySelect;
            string query = string.Empty;
            int IdVenta;
            int stock_producto = 0;
            int cont = -1;

            dtProductos = TraerStockProducto();

            dtUsuarios = TraerIdUsuarios();

            foreach (var row in Venta)
            {
                cont++;

                query = "Id = " + row.IdProducto.ToString();
                querySelect = dtProductos.Select(query);

                if (querySelect.Length == 0)
                {
                    Venta[cont].Estado = "Venta no Registrada - No existe el producto.";
                }
                else
                {
                    if (row.Stock > Convert.ToInt32(querySelect[0].ItemArray[1]))
                    {
                        Venta[cont].Estado = "Venta no Registrada - No hay Stock de producto.";
                    }
                    else
                    {
                        stock_producto = Convert.ToInt32(querySelect[0].ItemArray[1]) - row.Stock;
                    }
                }

                query = "Id = " + row.IdUsuario.ToString();
                querySelect = dtUsuarios.Select(query);

                if (querySelect.Length == 0)
                {
                    Venta[cont].Estado = "Venta no Registrada - No existe el Usuario.";
                }

                Venta[cont].Estado = AgregarVenta(row.IdProducto, row.IdUsuario);

                if (Venta[cont].Estado == "OK")
                {
                    IdVenta = TraerIdVenta();
                    Venta[cont].Estado = "Venta Registrada - Id Venta: " + IdVenta + " - IdUsuario: " + row.IdUsuario;
                }
                else
                {
                    continue;
                }

                Venta[cont].Estado = InsertProductoVendido(row.IdProducto, row.Stock, IdVenta);

                if (Venta[cont].Estado == "OK")
                {
                    Venta[cont].Estado = "Venta Registrada - Id Venta: " + IdVenta + " - IdUsuario: " + row.IdUsuario;
                }
                else
                {
                    continue;
                }

                Venta[cont].Estado = ModificarStockProducto(row.IdProducto, stock_producto, IdVenta, row.IdUsuario);

                if (Venta[cont].Estado == "OK")
                {
                    Venta[cont].Estado = "Venta Registrada y Stock Actualizado - Id Venta: " + IdVenta + " - IdUsuario: " + row.IdUsuario;
                }
                else
                {
                    continue;
                }
            }
            return Venta;
        }
        //Método para eliminar un registro de Venta
        public static string EliminarVenta(int idVenta)
        {
            string resultado = String.Empty;
            int stockVendido = 0;
            int productoVendido = 0;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    TraerStockProducto(idVenta, ref productoVendido, ref stockVendido);

                    EliminarProductoVendido(idVenta);

                    resultado = AgregarStockProducto(productoVendido, stockVendido, idVenta);

                    resultado = EliminarRegistroVenta(idVenta);
                }
            }
            catch (Exception ex)
            {
                resultado = "El ID de venta: '" + idVenta + "' no se ha podido eliminar. Error: " + ex.Message;
            }
            return resultado;
        }
        //Método para traer stock de Producto
        private static DataTable TraerStockProducto()
        {
            DataTable dtProd = new DataTable();

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter sqlAdapter = new SqlDataAdapter("SELECT Id, Stock FROM Producto", sqlConnection);
                sqlConnection.Open();
                sqlAdapter.Fill(dtProd);
                sqlConnection.Close();
            }
            return dtProd;
        }
        //Método para traer id Usuarios en una tabla
        private static DataTable TraerIdUsuarios()
        {
            DataTable dtUsu = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                SqlDataAdapter sqlAdapter = new SqlDataAdapter("SELECT Id FROM Usuario", sqlConnection);
                sqlConnection.Open();
                sqlAdapter.Fill(dtUsu);
                sqlConnection.Close();
            }
            return dtUsu;
        }
        //Método para agregar un registro de Venta por id de Producto e id de Usuario
        private static string AgregarVenta(int IdProd, int IdUsu)
        {
            string Estado = String.Empty;
            int filasAfectadas = 0;
            DataTable dtIdVenta = new DataTable();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    string queryUpdate = "INSERT INTO Venta (Comentarios) VALUES (@Comentarios)";

                    SqlParameter comentariosParameter = new SqlParameter("Comentarios", SqlDbType.VarChar)
                    {
                        Value = "Venta Registrada - Fecha Operación: " + DateTime.Now + " - Producto: " + IdProd + " - Vendedor: " + IdUsu
                    };

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(queryUpdate, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(comentariosParameter);
                        filasAfectadas = sqlCommand.ExecuteNonQuery();
                    }
                    if (filasAfectadas == 1)
                    {
                        Estado = "OK";
                    }
                    else
                    {
                        Estado = "Venta No Registrada - Error al ingresar venta.";
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Estado = "Venta No Registrada - Error al ingresar venta: " + ex.Message;
            }
            return Estado;
        }
        //Método para traer id de última Venta
        private static int TraerIdVenta()
        {
            DataTable dtIdVenta = new DataTable();
            string Status = String.Empty;

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                SqlDataAdapter sqlAdapter = new SqlDataAdapter("SELECT MAX(Id) FROM Venta", sqlConnection);
                sqlAdapter.Fill(dtIdVenta);
                sqlConnection.Close();
            }
            return Convert.ToInt32(dtIdVenta.Rows[0].ItemArray[0]);
        }
        //Método para agregar un registro de ProductoVendido por id de Producto, stockVendido e id de Venta
        private static string InsertProductoVendido(int IdProd, int StockVend, int IdVen)
        {
            string Status = String.Empty;
            int filasAfectadas = 0;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    string queryInsert = "INSERT INTO ProductoVendido (Stock, IdProducto, IdVenta) VALUES (@Stock, @IdProducto, @IdVenta)";

                    SqlParameter stockParameter = new SqlParameter("Stock", SqlDbType.Int) { Value = StockVend };
                    SqlParameter idProductoParameter = new SqlParameter("IdProducto", SqlDbType.Int) { Value = IdProd };
                    SqlParameter idVentaParameter = new SqlParameter("IdVenta", SqlDbType.Int) { Value = IdVen };

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(queryInsert, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(stockParameter);
                        sqlCommand.Parameters.Add(idProductoParameter);
                        sqlCommand.Parameters.Add(idVentaParameter);
                        filasAfectadas = sqlCommand.ExecuteNonQuery();
                        sqlCommand.Parameters.Clear();
                    }
                    if (filasAfectadas == 1)
                    {
                        Status = "OK";
                    }
                    else
                    {
                        Status = "Venta No Registrada - Error al ingresar venta.";
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Status = "Venta No Registrada - Error al ingresar venta: " + ex.Message;
            }
            return Status;
        }
        //Método para actualizar Stock de Producto
        private static string ModificarStockProducto(int Prod, int Stock, int IdVent, int IdUser)
        {
            string Status = String.Empty;
            int filasAfectadas = 0;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    string queryUpdate = "UPDATE Producto SET Stock = " + Stock + " WHERE Id = @IdProducto";

                    SqlParameter idProductoParameter = new SqlParameter("IdProducto", SqlDbType.BigInt) { Value = Prod };

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(queryUpdate, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(idProductoParameter);
                        filasAfectadas = sqlCommand.ExecuteNonQuery();
                        sqlCommand.Parameters.Clear();
                    }
                    if (filasAfectadas == 1)
                    {
                        Status = "OK";
                    }
                    else
                    {
                        Status = "Venta No Registrada - Error al ingresar venta.";
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Status = "Venta No Registrada - Error al ingresar venta: " + ex.Message;
            }
            return Status;
        }
        //Método para traer Stock de ProductoVendido
        private static void TraerStockProducto(int IdVent, ref int IdProd, ref int Stock)
        {
            DataTable table = new DataTable();

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                string queryStockVendido = "SELECT IdProducto, Stock FROM ProductoVendido WHERE IdVenta = @IdVenta";
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(queryStockVendido, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@IdVenta", IdVent);
                    SqlDataAdapter sqlAdapter = new SqlDataAdapter();
                    sqlAdapter.SelectCommand = sqlCommand;
                    sqlAdapter.Fill(table);

                    if (table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            Stock = Convert.ToInt32(row["Stock"]);
                            IdProd = Convert.ToInt32(row["IdProducto"]);
                        }
                    }
                }
                sqlConnection.Close();
            }
        }
        //Método para eliminar un registro de ProductoVendido por id de Venta
        private static void EliminarProductoVendido(int IdVenta)
        {
            int filasAfectadas = 0;

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                string queryDeleteProductoVendido = "DELETE FROM ProductoVendido WHERE IdVenta = @IdVenta";
                SqlParameter idVentaParameter = new SqlParameter("IdVenta", System.Data.SqlDbType.BigInt) { Value = IdVenta };
                
                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(queryDeleteProductoVendido, sqlConnection))
                {
                    sqlCommand.Parameters.Add(idVentaParameter);
                    filasAfectadas = sqlCommand.ExecuteNonQuery();
                    sqlCommand.Parameters.Clear();
                }
                sqlConnection.Close();
            }
        }
        //Método para modificar Stock de Producto eliminado
        private static string AgregarStockProducto(int ProdVend, int StockVend, int IdVent)
        {
            string Status = String.Empty;
            int filasAfectadas;

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                string queryUpdateStock = "UPDATE Producto SET Stock = (Stock + " + StockVend + ") where Id = @IdProducto";
                SqlParameter idProductoParameter = new SqlParameter("IdProducto", System.Data.SqlDbType.BigInt) { Value = ProdVend };

                sqlConnection.Open();
                using (SqlCommand sqlCommand = new SqlCommand(queryUpdateStock, sqlConnection))
                {
                    sqlCommand.Parameters.Add(idProductoParameter);
                    filasAfectadas = sqlCommand.ExecuteNonQuery();
                }
                if (filasAfectadas == 1)
                {
                    Status = "El ID de venta: " + IdVent + " se ha eliminado correctamente.";
                }
                else
                {
                    Status = "El ID de venta: " + IdVent + " no se ha podido eliminar.";
                }
                sqlConnection.Close();

            }
            return Status;
        }
        //Método para eliminar un registro de Venta
        private static string EliminarRegistroVenta(int IdVent)
        {
            string Status = String.Empty;
            int filasAfectadas;

            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                string queryDeleteVenta = "DELETE FROM Venta WHERE Id = @IdVenta";
                SqlParameter idVentaParameter = new SqlParameter("IdVenta", System.Data.SqlDbType.BigInt) { Value = IdVent };
                sqlConnection.Open();

                using (SqlCommand sqlCommand = new SqlCommand(queryDeleteVenta, sqlConnection))
                {
                    sqlCommand.Parameters.Add(idVentaParameter);
                    filasAfectadas = sqlCommand.ExecuteNonQuery();
                }
                if (filasAfectadas == 1)
                {
                    Status = "El ID de venta: " + IdVent + " se ha eliminado correctamente.";
                }
                else
                {
                    Status = "El ID de venta: " + IdVent + " no se ha podido eliminar.";
                }
                sqlConnection.Close();

            }
            return Status;
        }
    }
}
