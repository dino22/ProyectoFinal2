﻿using ProyectoFinal2.Model;
using System.Data;
using System.Data.SqlClient;

namespace ProyectoFinal2.Repository
{
    public static class UsuarioHandler
    {
        public const string connectionString = "Server=localhost\\SQLEXPRESS;Database=SistemaGestion;Trusted_Connection=True;";
        public static List<Usuario> TraerUsuarios(string NombreUsuario)
        {   
            Usuario usuarioObj = new Usuario();
            List<Usuario> usuarios = new List<Usuario>();

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string querySelect = "SELECT * FROM Usuario WHERE NombreUsuario = @nombreUsuario";

                    SqlParameter nombreUsuarioParameter = new SqlParameter("nombreUsuario", System.Data.SqlDbType.VarChar) { Value = NombreUsuario };

                    sqlConnection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand(querySelect, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(nombreUsuarioParameter);

                        using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                        {
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    Usuario usuario = new Usuario();
                                    usuario.Id = Convert.ToInt32(dataReader["Id"]);
                                    usuario.Nombre = dataReader["Nombre"].ToString();
                                    usuario.Apellido = dataReader["Apellido"].ToString();
                                    usuario.Contraseña = dataReader["Contraseña"].ToString();
                                    usuario.NombreUsuario = dataReader["NombreUsuario"].ToString();
                                    usuario.Mail = dataReader["Mail"].ToString();
                                    usuarios.Add(usuario);
                                }
                            }
                            else
                            {
                                usuarioObj.Id = 0;
                                usuarios.Add(usuarioObj);
                            }
                        }
                        sqlConnection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                usuarioObj.Id = 0;
                usuarios.Add(usuarioObj);
            }
            return usuarios;
        }

        public static string AgregarUsuario(Usuario usuario)
        {
            string resultado = String.Empty;
            bool error = false;
            bool idExistente = false;

            if (String.IsNullOrEmpty(usuario.Nombre) || String.IsNullOrEmpty(usuario.Apellido) || String.IsNullOrEmpty(usuario.NombreUsuario))
            {
                resultado = "Ingresar Nombre, Apellido o Nombre de Usuario.";
                error = true;
            }

            if (!error)
            {
                idExistente = ValidarNombreUsuarioExistente(usuario);
                if (idExistente)
                {
                    resultado = "El Nombre de Usuario: " + usuario.NombreUsuario + " ya existe.";
                }
            }
            
            if (!error && !idExistente)
            {
                resultado = AgregarUsuarioTable(usuario);
            }
            return resultado;
        }

        public static string ModificarUsuario(Usuario usuario)
        {
            string respuesta = String.Empty;

            respuesta = ValidarUsuarioId(usuario);

            if (String.IsNullOrEmpty(respuesta))
            {
                respuesta = ActualizarUsuario(usuario);
            }
            return respuesta;
        }

        public static string QuitarUsuario(int idUsuario)
        {
            string resultado = String.Empty;
            int filasAfectadas = 0;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    SqlParameter idUsuarioParameter = new SqlParameter("IdUsuario", System.Data.SqlDbType.BigInt) { Value = idUsuario };

                    string queryDeleteProductoVendido = "DELETE PV FROM ProductoVendido PV INNER JOIN Producto P ON PV.IdProducto = P.Id INNER JOIN Usuario U ON P.IdUsuario = U.Id WHERE U.Id = @IdUsuario";

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(queryDeleteProductoVendido, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(idUsuarioParameter);
                        filasAfectadas = sqlCommand.ExecuteNonQuery();
                        sqlCommand.Parameters.Clear();
                    }
                    sqlConnection.Close();

                    string queryDeleteProducto = "DELETE P FROM Producto P INNER JOIN Usuario U ON P.IdUsuario = U.Id WHERE U.Id = @IdUsuario";

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(queryDeleteProducto, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(idUsuarioParameter);
                        filasAfectadas = sqlCommand.ExecuteNonQuery();
                        sqlCommand.Parameters.Clear();
                    }
                    sqlConnection.Close();

                    string QueryUpdate = "DELETE FROM Usuario WHERE Id = @IdUsuario";

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(QueryUpdate, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(idUsuarioParameter);
                        filasAfectadas = sqlCommand.ExecuteNonQuery();
                    }
                    
                    if (filasAfectadas == 1)
                    {
                        resultado = "El usuario: " + idUsuario + " ha sido eliminado de los registros.";
                    }
                    else
                    {
                        resultado = "El usuario: " + idUsuario + " no ha sido eliminado.";
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                resultado = "El usuario: " + idUsuario + " no ha sido eliminado. - Error: " + ex.Message;
            }
            return resultado;
        }

        public static Usuario ValidarUsuario(string user, string psw)
        {
            Usuario usuario = new Usuario();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Usuario WHERE NombreUsuario = @user AND Contraseña = @psw", sqlConnection))
                {
                    sqlConnection.Open();

                    sqlCommand.Parameters.AddWithValue("@user", user);
                    sqlCommand.Parameters.AddWithValue("@psw", psw);
                    SqlDataAdapter sqlAdapter = new SqlDataAdapter();
                    sqlAdapter.SelectCommand = sqlCommand;
                    
                    DataTable table = new DataTable();
                    sqlAdapter.Fill(table);

                    if (table.Rows.Count > 0)
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            usuario.Id = Convert.ToInt32(row["Id"]);
                            usuario.Nombre = row["Nombre"].ToString();
                            usuario.Apellido = row["Apellido"].ToString();
                            usuario.Mail = row["Mail"].ToString();
                            usuario.NombreUsuario = user;
                            usuario.Contraseña = psw;
                        }
                    }
                    else
                    {
                        usuario.Id = 0;
                    }

                    sqlConnection.Close();
                }
            }
            return usuario;
        }

        private static bool ValidarNombreUsuarioExistente(Usuario usuario)
        {
            bool idExistente = false;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string querySelect = "SELECT * FROM Usuario WHERE NombreUsuario = @nombreUsuario";

                    //Parámetros
                    SqlParameter nombreUsuarioParameter = new SqlParameter("nombreUsuario", System.Data.SqlDbType.VarChar) { Value = usuario.NombreUsuario };

                    sqlConnection.Open();

                    SqlCommand sqlCommand = new SqlCommand(querySelect, sqlConnection);
                    sqlCommand.Parameters.Add(nombreUsuarioParameter);
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                    if (sqlDataReader.HasRows)
                    {
                        idExistente = true;
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception)
            {
                idExistente = true;
            }
            return idExistente;
        }

        private static string AgregarUsuarioTable(Usuario usuario)
        {
            string resultado = String.Empty;
            int filasAfectadas = 0;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string QueryUpdate = "INSERT INTO Usuario (Nombre, Apellido, NombreUsuario, Contraseña, Mail) VALUES (@Nombre, @Apellido, @NombreUsuario, @Contraseña, @Mail )";

                    //Parámetros
                    SqlParameter param_Nombre = new SqlParameter("Nombre", SqlDbType.VarChar) { Value = usuario.Nombre };
                    SqlParameter param_Apellido = new SqlParameter("Apellido", SqlDbType.VarChar) { Value = usuario.Apellido };
                    SqlParameter param_NombreUsuario = new SqlParameter("NombreUsuario", SqlDbType.VarChar) { Value = usuario.NombreUsuario };
                    SqlParameter param_Contraseña = new SqlParameter("Contraseña", SqlDbType.VarChar) { Value = usuario.Contraseña };
                    SqlParameter param_Mail = new SqlParameter("Mail", SqlDbType.VarChar) { Value = usuario.Mail };

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(QueryUpdate, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(param_Nombre);
                        sqlCommand.Parameters.Add(param_Apellido);
                        sqlCommand.Parameters.Add(param_NombreUsuario);
                        sqlCommand.Parameters.Add(param_Contraseña);
                        sqlCommand.Parameters.Add(param_Mail);
                        filasAfectadas = sqlCommand.ExecuteNonQuery();
                    }
                    if (filasAfectadas == 1)
                    {
                        resultado = "El usuario: " + usuario.NombreUsuario + " ha sido resgitrado.";
                    }
                    else
                    {
                        resultado = "El usuario " + usuario.NombreUsuario + " no ha sido registrado.";
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                resultado = "El usuario " + usuario.NombreUsuario + " no ha sido registrado. - Error: " + ex.Message;
            }
            return resultado;
        }

        private static string ValidarUsuarioId(Usuario usuario)
        {
            string resultado = String.Empty;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string querySelect = "SELECT * FROM Usuario WHERE NombreUsuario = @NombreUsuario AND Id != @IdUsuario";

                    //Parámetros
                    SqlParameter parametroUsuarioId = new SqlParameter("IdUsuario", System.Data.SqlDbType.BigInt) { Value = usuario.Id };
                    SqlParameter param_NombreUsuario = new SqlParameter("NombreUsuario", System.Data.SqlDbType.VarChar) { Value = usuario.NombreUsuario };

                    sqlConnection.Open();

                    SqlCommand sqlCommand = new SqlCommand(querySelect, sqlConnection);
                    sqlCommand.Parameters.Add(parametroUsuarioId);
                    sqlCommand.Parameters.Add(param_NombreUsuario);
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                    if (sqlDataReader.HasRows)
                    {
                        resultado = "El Nombre de Usuario: " + usuario.NombreUsuario + " ya existe.";
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                resultado = "Error al actualizar el usuario: " + usuario.NombreUsuario + " - Error: " + ex.Message;
            }
            return resultado;
        }

        private static string ActualizarUsuario(Usuario usuario)
        {
            string resultado = String.Empty;
            int filasAfectadas = 0;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string queryUpdate = "UPDATE Usuario SET Nombre = @Nombre, Apellido = @Apellido, NombreUsuario = @NombreUsuario, Contraseña = @Contraseña, Mail = @Mail WHERE Id = @IdUsuario";

                    //Parámetros
                    SqlParameter idUsuarioParameter = new SqlParameter("IdUsuario", System.Data.SqlDbType.Int) { Value = usuario.Id };
                    SqlParameter nombreParameter = new SqlParameter("Nombre", System.Data.SqlDbType.VarChar) { Value = usuario.Nombre };
                    SqlParameter apellidoParameter = new SqlParameter("Apellido", System.Data.SqlDbType.VarChar) { Value = usuario.Apellido };
                    SqlParameter nombreUsuarioParameter = new SqlParameter("NombreUsuario", System.Data.SqlDbType.VarChar) { Value = usuario.NombreUsuario };
                    SqlParameter contraseñaParameter = new SqlParameter("Contraseña", System.Data.SqlDbType.VarChar) { Value = usuario.Contraseña };
                    SqlParameter mailParameter = new SqlParameter("Mail", System.Data.SqlDbType.VarChar) { Value = usuario.Mail };

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(queryUpdate, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(idUsuarioParameter);
                        sqlCommand.Parameters.Add(nombreParameter);
                        sqlCommand.Parameters.Add(apellidoParameter);
                        sqlCommand.Parameters.Add(nombreUsuarioParameter);
                        sqlCommand.Parameters.Add(contraseñaParameter);
                        sqlCommand.Parameters.Add(mailParameter);
                        filasAfectadas = sqlCommand.ExecuteNonQuery();
                    }
                    if (filasAfectadas == 1)
                    {
                        resultado = "Se ha actualizado el usuario: " + usuario.NombreUsuario;
                    }
                    else
                    {
                        resultado = "No se ha actualizado el usuario: " + usuario.NombreUsuario;
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                resultado = "Error al actualizar el usuario: " + usuario.NombreUsuario + " - Error: " + ex.Message;
            }
            return resultado;
        }
    }


}