using ModeloClasesAlumnos;
using System.Net;
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
            try
            {
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Api/Cursos", curso);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Curso>();
                }
                else
                {
                    throw new Exception($"Error en la solicitud: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Error al conectar con el servidor: {ex.Message}");
            }
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

        public Task<Curso> DameCurso(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Curso> DameCurso(int id, int idPrecio)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<Curso>($"Api/Cursos/{id}/{idPrecio}");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al obtener el curso con ID {id} y precio ID {idPrecio}: {ex.Message}");
                return null;
            }
        }

        public async Task<Curso> ModificarCurso(Curso curso)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"Api/Cursos/{curso.Id}", curso);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Curso>();
                }
                else
                {
                    Console.WriteLine($"Error al modificar el curso con ID {curso.Id}: {response.ReasonPhrase}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al modificar el curso con ID {curso.Id}: {ex.Message}");
                return null;
            }
        }

        public async Task<Curso> BorrarCurso(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"Api/Cursos/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Curso>();
                }
                else
                {
                    Console.WriteLine($"Error al eliminar el curso con ID {id}: {response.ReasonPhrase}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error al eliminar el curso con ID {id}: {ex.Message}");
                return null;
            }
        }
    }
}
