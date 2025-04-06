using ModeloClasesAlumnos;
using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace BlazorServer.Servicios
{
    public class ServicioCursos : IServicioCursos
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ServicioCursos> _logger;

        public ServicioCursos(HttpClient httpClient, ILogger<ServicioCursos> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Curso> AltaCurso(Curso curso)
        {
            try
            {
                _logger.LogInformation($"AltaCurso: Intentando crear el curso '{curso.NombreCurso}'...");
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Api/Cursos", curso);

                if (response.IsSuccessStatusCode)
                {
                    var nuevoCurso = await response.Content.ReadFromJsonAsync<Curso>();
                    _logger.LogInformation($"AltaCurso: Curso '{nuevoCurso?.NombreCurso}' creado con éxito.");
                    return nuevoCurso;
                }
                else
                {
                    _logger.LogWarning($"AltaCurso: Falló la solicitud. Código HTTP: {response.StatusCode}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "AltaCurso: Error al conectar con el servidor.");
                return null;
            }
        }

        public async Task<IEnumerable<Curso>> DameCursos(int idAlumno)
        {
            try
            {
                _logger.LogInformation($"DameCursos: Obteniendo cursos para el alumno con ID {idAlumno}...");
                return await _httpClient.GetFromJsonAsync<IEnumerable<Curso>>($"Api/Cursos/AlumnosCursos/{idAlumno}")
                    ?? Enumerable.Empty<Curso>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"DameCursos: Error al obtener cursos para el alumno con ID {idAlumno}.");
                return Enumerable.Empty<Curso>();
            }
        }

        public async Task<Curso> DameCurso(int id, int idPrecio)
        {
            try
            {
                _logger.LogInformation($"DameCurso: Obteniendo curso con ID {id} y precio ID {idPrecio}...");
                return await _httpClient.GetFromJsonAsync<Curso>($"Api/Cursos/{id}/{idPrecio}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"DameCurso: Error al obtener el curso con ID {id} y precio ID {idPrecio}.");
                return null;
            }
        }

        public Task<Curso> DameCurso(int id)
        {
            _logger.LogWarning($"DameCurso: Método no implementado para ID {id}.");
            return Task.FromResult<Curso>(null);
        }

        public async Task<Curso> ModificarCurso(Curso curso)
        {
            try
            {
                _logger.LogInformation($"ModificarCurso: Modificando curso con ID {curso.Id}...");

                var response = await _httpClient.PutAsJsonAsync($"Api/Cursos/{curso.Id}", curso);

                if (response.IsSuccessStatusCode)
                {
                    var cursoModificado = await response.Content.ReadFromJsonAsync<Curso>();
                    _logger.LogInformation($"ModificarCurso: Curso con ID {curso.Id} modificado correctamente.");
                    return cursoModificado;
                }
                else
                {
                    _logger.LogWarning($"ModificarCurso: Falló la solicitud. Código HTTP: {response.StatusCode}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"ModificarCurso: Error al modificar el curso con ID {curso.Id}.");
                return null;
            }
        }

        public async Task<Curso> BorrarCurso(int id)
        {
            try
            {
                _logger.LogInformation($"BorrarCurso: Intentando eliminar el curso con ID {id}...");

                var response = await _httpClient.DeleteAsync($"Api/Cursos/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var cursoEliminado = await response.Content.ReadFromJsonAsync<Curso>();
                    _logger.LogInformation($"BorrarCurso: Curso con ID {id} eliminado correctamente.");
                    return cursoEliminado;
                }
                else
                {
                    _logger.LogError($"BorrarCurso: No se pudo eliminar el curso con ID {id}. Código HTTP: {response.StatusCode}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"BorrarCurso: Error al eliminar el curso con ID {id}.");
                return null;
            }
        }
    }
}
