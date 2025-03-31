using ApiAlumnos.Datos;
using ModeloClasesAlumnos;
using System.Data;
using System.Data.SqlClient;

namespace ApiAlumnos.Repositorios
{
    public class RepositorioAlumnos : IRepositorioAlumnos
    {
        private readonly string _CadenaConexion;

        public RepositorioAlumnos(AccesoDatos bd)
        {
            _CadenaConexion = bd.CadenaConexionSQL;
        }

        public SqlConnection Conexion()
        {
            return new SqlConnection(_CadenaConexion);
        }

        public async Task<Alumno> AltaAlumno(Alumno alumno)
        {
            using (var sqlConnection = Conexion())
            using (var sqlCommand = new SqlCommand("dbo.UsuarioAltaAlumno", sqlConnection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@Nombre", SqlDbType.VarChar, 500).Value = alumno.Nombre;
                sqlCommand.Parameters.Add("@Email", SqlDbType.VarChar, 500).Value = alumno.Email;
                sqlCommand.Parameters.Add("@Foto", SqlDbType.VarChar, 500).Value = alumno.Foto;
                sqlCommand.Parameters.Add("@FechaAlta", SqlDbType.DateTime).Value = alumno.FechaAlta;

                try
                {
                    await sqlConnection.OpenAsync();
                    var idGenerado = Convert.ToInt32(await sqlCommand.ExecuteScalarAsync());

                    alumno.Id = idGenerado;  // Asignamos el ID generado al objeto alumno

                    alumno = await DameAlumno(idGenerado);

                    return alumno;
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al dar de alta al alumno. Intente nuevamente más tarde.", ex);
                }
            }
        }

        public async Task<Alumno> BorrarAlumno(int id)
        {
            using (var sqlConnection = Conexion())
            using (var sqlCommand = new SqlCommand("dbo.UsuarioMarcaBaja", sqlConnection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                try
                {
                    await sqlConnection.OpenAsync();
                    var idGenerado = await sqlCommand.ExecuteScalarAsync(); // Obtener el ID modificado

                    if (idGenerado == null)
                        throw new Exception("Error al eliminar el alumno, no se recibió un ID.");

                    return await DameAlumno(Convert.ToInt32(idGenerado)); // Consultamos el alumno actualizado
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al eliminar el alumno. Intente nuevamente más tarde.", ex);
                }
            }
        }


        public async Task<Alumno> DameAlumno(int id)
        {
            Alumno alumno = null;

            using (var sqlConnection = Conexion())
            using (var sqlCommand = new SqlCommand("dbo.UsuarioDameAlumnos", sqlConnection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Id", id);

                try
                {
                    await sqlConnection.OpenAsync();
                    using (var reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            alumno = new Alumno
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Nombre = reader["Nombre"].ToString(),
                                Email = reader["Email"].ToString(),
                                Foto = reader["Foto"].ToString(),
                                FechaAlta = reader.GetDateTime(reader.GetOrdinal("FechaAlta")),
                                FechaBaja = reader.IsDBNull(reader.GetOrdinal("FechaBaja"))
                                            ? (DateTime?)null
                                            : reader.GetDateTime(reader.GetOrdinal("FechaBaja"))
                            };
                        }
                        //else
                        //{
                        //    // Si se usan los 2 cath para recibir el error
                        //    throw new KeyNotFoundException($"No se encontró ningún alumno con el ID {id}.");
                        //}
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener los datos del alumno. Intente nuevamente más tarde.", ex);
                }
            }

            return alumno;
        }

        //public async Task<IEnumerable<Alumno>> DameAlumnos()
        //{
        //    List<Alumno> list = new List<Alumno>();
        //    SqlConnection sqlConnection = Conexion();
        //    SqlCommand Comm = null;
        //    try
        //    {
        //        sqlConnection.Open();
        //        Comm = sqlConnection.CreateCommand();
        //        Comm.CommandText = "dbo.UsuarioDameAlumnos";
        //        Comm.CommandType = System.Data.CommandType.StoredProcedure;
        //        SqlDataReader reader = await Comm.ExecuteReaderAsync();
        //        while (reader.Read())
        //        {
        //            Alumno alu = new Alumno();
        //            alu.Id = Convert.ToInt32(reader["Id"]);
        //            alu.Nombre = reader["Nombre"].ToString();
        //            alu.Email = reader["Email"].ToString();
        //            alu.Foto = reader["Foto"].ToString();
        //            alu.FechaAlta = Convert.ToDateTime(reader["FechaAlta"].ToString());
        //            if (reader["FechaAlta"] != System.DBNull.Value)
        //                alu.FechaAlta = Convert.ToDateTime(reader["FechaAlta"].ToString());

        //            list.Add(alu);
        //        }

        //        reader.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error obteniendo los Datos" + ex.Message);
        //    }
        //    finally
        //    {
        //        Comm.Dispose();
        //        sqlConnection.Close();
        //        sqlConnection.Dispose();
        //    }

        //    return list;
        //}

        public async Task<Alumno> DameAlumno(string email)
        {
            Alumno alumno = null;

            using (var sqlConnection = Conexion())
            using (var sqlCommand = new SqlCommand("dbo.UsuarioDameAlumnos", sqlConnection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Email", email);

                try
                {
                    await sqlConnection.OpenAsync();
                    using (var reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            alumno = new Alumno
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Nombre = reader["Nombre"].ToString(),
                                Email = reader["Email"].ToString(),
                                Foto = reader["Foto"].ToString(),
                                FechaAlta = reader.GetDateTime(reader.GetOrdinal("FechaAlta")),
                                FechaBaja = reader.IsDBNull(reader.GetOrdinal("FechaBaja"))
                                            ? (DateTime?)null
                                            : reader.GetDateTime(reader.GetOrdinal("FechaBaja"))
                            };
                        }
                        //else
                        //{
                        //    // Si se usan los 2 cath para recibir el error
                        //    throw new KeyNotFoundException($"No se encontró ningún alumno con el ID {id}.");
                        //}
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener los datos del alumno. Intente nuevamente más tarde.", ex);
                }
            }

            return alumno;
        }

        public async Task<IEnumerable<Alumno>> DameAlumnos()
        {
            var listaAlumnos = new List<Alumno>();

            using (var sqlConnection = Conexion())
            using (var sqlCommand = new SqlCommand("dbo.UsuarioDameAlumnos", sqlConnection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;

                try
                {
                    await sqlConnection.OpenAsync();
                    using (var reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var alumno = new Alumno
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Nombre = reader["Nombre"].ToString(),
                                Email = reader["Email"].ToString(),
                                Foto = reader["Foto"].ToString(),
                                FechaAlta = Convert.ToDateTime(reader["FechaAlta"].ToString()),
                                FechaBaja = reader.IsDBNull(reader.GetOrdinal("FechaBaja"))
                                            ? (DateTime?)null
                                            : reader.GetDateTime(reader.GetOrdinal("FechaBaja"))
                            };

                            listaAlumnos.Add(alumno);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Puedes registrar el error para un análisis posterior si tienes un sistema de logging
                    throw new Exception("Error al obtener los datos de los alumnos. Intente nuevamente más tarde.", ex);
                }
            }

            return listaAlumnos;
        }

        public async Task<Alumno> ModificarAlumno(Alumno alumno)
        {
            using (var sqlConnection = Conexion())
            using (var sqlCommand = new SqlCommand("dbo.UsuarioModificarAlumno", sqlConnection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.Add("@Id", SqlDbType.Int).Value = alumno.Id;
                sqlCommand.Parameters.Add("@Nombre", SqlDbType.NVarChar, 500).Value = alumno.Nombre;
                sqlCommand.Parameters.Add("@Email", SqlDbType.NVarChar, 500).Value = alumno.Email;
                sqlCommand.Parameters.Add("@Foto", SqlDbType.NVarChar, 500).Value = alumno.Foto;
                //sqlCommand.Parameters.Add("@FechaAlta", SqlDbType.DateTime).Value = alumno.FechaAlta;
                if (alumno.FechaBaja != null)
                    sqlCommand.Parameters.Add("@FechaBaja", SqlDbType.DateTime).Value = (object)alumno.FechaBaja ?? DBNull.Value;

                try
                {
                    await sqlConnection.OpenAsync();
                    //int idGenerado = await sqlCommand.ExecuteNonQueryAsync();
                    var idGenerado = Convert.ToInt32(await sqlCommand.ExecuteScalarAsync());

                    if (idGenerado == null)
                        throw new Exception("Error al modificar el alumno, no se recibió un ID.");

                    alumno.Id = idGenerado;  // Asignamos el ID generado al objeto alumno

                    alumno = await DameAlumno(idGenerado);

                    //if (filasAfectadas == 0)
                    //    throw new KeyNotFoundException($"No se encontró ningún alumno con el ID {alumno.Id}.");

                    return alumno;
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al modificar los datos del alumno. Intente nuevamente más tarde.", ex);
                }
            }
        }

        public async Task<IEnumerable<Alumno>> BuscarAlumnos(string texto)
        {
            var listaAlumnos = new List<Alumno>();

            using (var sqlConnection = Conexion())
            using (var sqlCommand = new SqlCommand("dbo.UsuarioBuscarAlumnos", sqlConnection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@Texto", texto);

                try
                {
                    await sqlConnection.OpenAsync();
                    using (var reader = await sqlCommand.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var alumno = new Alumno
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Nombre = reader["Nombre"].ToString(),
                                Email = reader["Email"].ToString(),
                                Foto = reader["Foto"].ToString(),
                                FechaAlta = Convert.ToDateTime(reader["FechaAlta"].ToString()),
                                FechaBaja = reader.IsDBNull(reader.GetOrdinal("FechaBaja"))
                                            ? (DateTime?)null
                                            : reader.GetDateTime(reader.GetOrdinal("FechaBaja"))
                            };

                            listaAlumnos.Add(alumno);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    // Puedes registrar el error para un análisis posterior si tienes un sistema de logging
                    throw new Exception("Error al obtener los datos de los alumnos. Intente nuevamente más tarde.", ex);
                }
            }

            return listaAlumnos;
        }

        public async Task<Alumno> InscribirAlumnoCurso(int idAlumno, int idCurso, int idPrecio)
        {
            Alumno alumnoInscrito = null;

            using (var sqlConexion = Conexion())
            using (var comm = new SqlCommand("dbo.UsuarioInscribirCurso", sqlConexion) { CommandType = CommandType.StoredProcedure })
            {
                comm.Parameters.Add("@idAlumno", SqlDbType.Int).Value = idAlumno;
                comm.Parameters.Add("@idCurso", SqlDbType.Int).Value = idCurso;
                comm.Parameters.Add("@idPrecio", SqlDbType.Int).Value = idPrecio;

                try
                {
                    await sqlConexion.OpenAsync();
                    await comm.ExecuteNonQueryAsync();

                    alumnoInscrito = await DameAlumno(idAlumno);
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error inscribiendo alumno en curso: " + ex.Message, ex);
                }
            }

            return alumnoInscrito;
        }

        public async Task<Alumno> AlumnoCursos(int idAlumno)
        {
            Alumno alumno = null;
            Curso cursoActual = null;
            int idCursoAux = -1;

            using (var sqlConexion = Conexion())
            using (var comm = new SqlCommand("dbo.UsuarioInscritoCursos", sqlConexion) { CommandType = CommandType.StoredProcedure })
            {
                comm.Parameters.Add("@idAlumno", SqlDbType.Int).Value = idAlumno;

                try
                {
                    await sqlConexion.OpenAsync();

                    using (var reader = await comm.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            if (alumno == null)
                            {
                                alumno = new Alumno
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Nombre = reader["Nombre"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Foto = reader["Foto"].ToString(),
                                    ListaCursos = new List<Curso>()
                                };
                            }

                            int idCurso = Convert.ToInt32(reader["IdCurso"]);

                            if (idCursoAux != idCurso)
                            {
                                if (cursoActual != null)
                                {
                                    alumno.ListaCursos.Add(cursoActual);
                                }

                                cursoActual = new Curso
                                {
                                    Id = idCurso,
                                    NombreCurso = reader["NombreCurso"].ToString(),
                                    ListaPrecios = new List<Precio>()
                                };

                                idCursoAux = idCurso;
                            }

                            // Agregar los precios asociados al curso
                            var precio = new Precio
                            {
                                Coste = Convert.ToDouble(reader["Coste"]),
                                FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                                FechaFin = Convert.ToDateTime(reader["FechaFin"])
                            };

                            cursoActual.ListaPrecios.Add(precio);
                        }

                        // Agregar el último curso si existe
                        if (cursoActual != null)
                        {
                            alumno.ListaCursos.Add(cursoActual);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error cargando los datos del alumno: " + ex.Message, ex);
                }
            }

            return alumno;
        }
    }
}
