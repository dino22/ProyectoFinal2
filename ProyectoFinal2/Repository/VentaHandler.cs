using ProyectoFinal2.Controllers.DTO;
using ProyectoFinal2.Model;
using System.Data.SqlClient;
using System.Data;

namespace ProyectoFinal2.Repository
{
    public static class VentaHandler
    {
        public const string ConnectionString = "Server=localhost\\SQLEXPRESS;Database=SistemaGestion;Trusted_Connection=True;";
        public static List<Controllers.DTO.GetVenta> TraerVentas(int IdUsuario)
        {
            List<Controllers.DTO.GetVenta> Ventas = new List<Controllers.DTO.GetVenta>();

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
                                Controllers.DTO.GetVenta venta = new Controllers.DTO.GetVenta();
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

        public static List<PostVenta> AgregarVentas(List<PostVenta> DetalleVenta)
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

            foreach (var row in DetalleVenta)
            {
                cont++;

                query = "Id = " + row.IdProducto.ToString();
                querySelect = dtProductos.Select(query);

                if (querySelect.Length == 0)
                {
                    DetalleVenta[cont].Status = "Venta no Registrada - No existe el producto.";
                }
                else
                {
                    if (row.Stock > Convert.ToInt32(querySelect[0].ItemArray[1]))
                    {
                        DetalleVenta[cont].Status = "Venta no Registrada - No hay Stock de producto.";
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
                    DetalleVenta[cont].Status = "Venta no Registrada - No existe el Usuario.";
                }

                DetalleVenta[cont].Status = AgregarVenta(row.IdProducto, row.IdUsuario);

                if (DetalleVenta[cont].Status == "OK")
                {
                    IdVenta = TraerIdVenta();
                    DetalleVenta[cont].Status = "Venta Registrada - Id Venta: " + IdVenta + " - IdUsuario: " + row.IdUsuario;
                }
                else
                {
                    continue;
                }

                DetalleVenta[cont].Status = InsertProductoVendido(row.IdProducto, row.Stock, IdVenta);

                if (DetalleVenta[cont].Status == "OK")
                {
                    DetalleVenta[cont].Status = "Venta Registrada - Id Venta: " + IdVenta + " - IdUsuario: " + row.IdUsuario;
                }
                else
                {
                    continue;
                }

                DetalleVenta[cont].Status = ActualizarStockProducto(row.IdProducto, stock_producto, IdVenta, row.IdUsuario);

                if (DetalleVenta[cont].Status == "OK")
                {
                    DetalleVenta[cont].Status = "Venta Registrada y Stock Actualizado - Id Venta: " + IdVenta + " - IdUsuario: " + row.IdUsuario;
                }
                else
                {
                    continue;
                }
            }
            return DetalleVenta;
        }

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

                    resultado = EliminarTablaVenta(idVenta);
                }
            }
            catch (Exception ex)
            {
                resultado = "El ID de venta: '" + idVenta + "' no se ha podido eliminar. Error: " + ex.Message;
            }
            return resultado;
        }

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

        private static string AgregarVenta(int IdProd, int IdUsu)
        {
            string Status = String.Empty;
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

        private static string InsertProductoVendido(int IdProd, int CantVend, int IdVen)
        {
            string Status = String.Empty;
            int filasAfectadas = 0;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    string queryInsert = "INSERT INTO ProductoVendido (Stock, IdProducto, IdVenta) VALUES (@Stock, @IdProducto, @IdVenta)";

                    SqlParameter stockParameter = new SqlParameter("Stock", SqlDbType.Int) { Value = CantVend };
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

        private static string ActualizarStockProducto(int Prod, int Stock, int IdVent, int IdUser)
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

        private static string EliminarTablaVenta(int IdVent)
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
