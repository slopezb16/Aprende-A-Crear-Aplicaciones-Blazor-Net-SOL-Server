using ModeloClasesAlumnos;
using System.Net.Http;

namespace BlazorServer.Servicios
{
    public class ServicioCursos : IServicioCursos
    {
        private readonly HttpClient _httpClient;

        public ServicioCursos(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Curso> AltaCurso(Curso curso)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Curso>> DameCursos(int idAlumno)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<IEnumerable<Curso>>($"Api/Cursos/AlumnosCursos/{idAlumno}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener cursos para el alumno con ID {idAlumno}: {ex.Message}");
                return Enumerable.Empty<Curso>();
            }
        }

        public async Task<IEnumerable<Curso>> DameCursos()
        {
            throw new NotImplementedException();
        }
    }
}
