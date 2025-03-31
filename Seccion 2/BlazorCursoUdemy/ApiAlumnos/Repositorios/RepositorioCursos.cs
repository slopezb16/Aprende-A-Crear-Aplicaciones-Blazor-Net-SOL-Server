using System.Data;
using System.Data.SqlClient;
using ApiAlumnos.Datos;
using ModeloClasesAlumnos;

namespace ApiAlumnos.Repositorios
{
    public class RepositorioCursos : IRepositorioCursos
    {
        private readonly string _CadenaConexion;

        public RepositorioCursos(AccesoDatos bd)
        {
            _CadenaConexion = bd.CadenaConexionSQL;
        }

        public SqlConnection Conexion()
        {
            return new SqlConnection(_CadenaConexion);
        }

        public async Task<Curso> AltaCurso(Curso curso)
        {
            if (curso == null || string.IsNullOrWhiteSpace(curso.NombreCurso) || curso.ListaPrecios == null || !curso.ListaPrecios.Any())
                throw new ArgumentException("El curso y su lista de precios deben ser válidos.");

            Curso cursoCreado = null;
            int idCursoCreado = -1;

            using (var sqlConexion = Conexion())
            using (var Comm = new SqlCommand("dbo.CursoAltaCurso", sqlConexion) { CommandType = CommandType.StoredProcedure })
            {
                await sqlConexion.OpenAsync().ConfigureAwait(false);

                foreach (var precio in curso.ListaPrecios)
                {
                    if (precio.FechaInicio == null || precio.FechaFin == null)
                        throw new ArgumentException("Los precios deben tener fecha de inicio y fin.");

                    Comm.Parameters.Clear();
                    Comm.Parameters.AddWithValue("@Nombrecurso", curso.NombreCurso);
                    Comm.Parameters.AddWithValue("@Coste", precio.Coste);
                    Comm.Parameters.AddWithValue("@FechaInicio", precio.FechaInicio);
                    Comm.Parameters.AddWithValue("@FechaFin", precio.FechaFin);

                    idCursoCreado = Convert.ToInt32(await Comm.ExecuteScalarAsync().ConfigureAwait(false));
                }
            }

            if (idCursoCreado != -1)
                cursoCreado = await DameCurso(idCursoCreado).ConfigureAwait(false);

            return cursoCreado;
        }

        public async Task<Curso> BorrarCurso(int id)
        {
            Curso cursoBorrado = await DameCurso(id).ConfigureAwait(false);
            if (cursoBorrado == null) return null;

            using (var sqlConexion = Conexion())
            using (var Comm = new SqlCommand("dbo.CursoBorrarCurso", sqlConexion) { CommandType = CommandType.StoredProcedure })
            {
                Comm.Parameters.AddWithValue("@idCurso", id);
                await sqlConexion.OpenAsync().ConfigureAwait(false);
                await Comm.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return cursoBorrado;
        }

        public async Task<Curso> DameCurso(int id)
        {
            return await ObtenerCursoPorParametro("@id", id).ConfigureAwait(false);
        }

        public async Task<Curso> DameCurso(string nombreCurso)
        {
            return await ObtenerCursoPorParametro("@NombreCurso", nombreCurso).ConfigureAwait(false);
        }

        private async Task<Curso> ObtenerCursoPorParametro(string parametro, object valor)
        {
            Curso curso = null;

            using (var sqlConexion = Conexion())
            using (var Comm = new SqlCommand("dbo.CursoDameCursos", sqlConexion) { CommandType = CommandType.StoredProcedure })
            {
                Comm.Parameters.AddWithValue(parametro, valor);
                await sqlConexion.OpenAsync().ConfigureAwait(false);

                using (var reader = await Comm.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (reader.Read())
                    {
                        if (curso == null)
                        {
                            curso = new Curso
                            {
                                Id = Convert.ToInt32(reader["idCurso"]),
                                NombreCurso = reader["NombreCurso"].ToString(),
                                ListaPrecios = new List<Precio>()
                            };
                        }

                        curso.ListaPrecios.Add(new Precio
                        {
                            Id = Convert.ToInt32(reader["idPrecio"]),
                            Coste = Convert.ToDouble(reader["Coste"]),
                            FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                            FechaFin = Convert.ToDateTime(reader["FechaFin"])
                        });
                    }
                }
            }

            return curso;
        }

        public async Task<IEnumerable<Curso>> DameCursos(int idAlumno)
        {
            var listaCursos = new List<Curso>();

            using (var sqlConexion = Conexion())
            using (var Comm = new SqlCommand("dbo.CursoDameCursos", sqlConexion) { CommandType = CommandType.StoredProcedure })
            {
                if (idAlumno != -1)
                    Comm.Parameters.AddWithValue("@idAlumno", idAlumno);
                else
                    Comm.Parameters.AddWithValue("@idAlumno", DBNull.Value);

                await sqlConexion.OpenAsync().ConfigureAwait(false);

                using (var reader = await Comm.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    Curso curso = null;

                    while (reader.Read())
                    {
                        int idCurso = Convert.ToInt32(reader["idCurso"]);

                        if (curso == null || curso.Id != idCurso)
                        {
                            if (curso != null)
                                listaCursos.Add(curso);

                            curso = new Curso
                            {
                                Id = idCurso,
                                NombreCurso = reader["NombreCurso"].ToString(),
                                ListaPrecios = new List<Precio>()
                            };
                        }

                        curso.ListaPrecios.Add(new Precio
                        {
                            Id = Convert.ToInt32(reader["idPrecio"]),
                            Coste = Convert.ToDouble(reader["Coste"]),
                            FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                            FechaFin = Convert.ToDateTime(reader["FechaFin"])
                        });
                    }

                    if (curso != null)
                        listaCursos.Add(curso);
                }
            }

            return listaCursos;
        }

        public async Task<Curso> ModificarCurso(Curso curso)
        {
            if (curso == null || curso.Id <= 0 || curso.ListaPrecios == null || !curso.ListaPrecios.Any())
                throw new ArgumentException("El curso y su lista de precios deben ser válidos.");

            using (var sqlConexion = Conexion())
            using (var Comm = new SqlCommand("dbo.CursoModificarCurso", sqlConexion) { CommandType = CommandType.StoredProcedure })
            {
                await sqlConexion.OpenAsync().ConfigureAwait(false);

                foreach (var precio in curso.ListaPrecios)
                {
                    Comm.Parameters.Clear();
                    Comm.Parameters.AddWithValue("@idCurso", curso.Id);
                    Comm.Parameters.AddWithValue("@idPrecio", precio.Id);
                    Comm.Parameters.AddWithValue("@NombreCurso", curso.NombreCurso);
                    Comm.Parameters.AddWithValue("@Coste", precio.Coste);
                    Comm.Parameters.AddWithValue("@FechaInicio", precio.FechaInicio);
                    Comm.Parameters.AddWithValue("@FechaFin", precio.FechaFin);

                    await Comm.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
            }

            return await DameCurso(curso.Id).ConfigureAwait(false);
        }
    }
}