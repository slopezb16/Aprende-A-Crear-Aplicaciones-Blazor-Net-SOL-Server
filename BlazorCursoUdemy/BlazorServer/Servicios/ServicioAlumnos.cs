using BlazorServer.Servicios;
using ModeloClasesAlumnos;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

public class ServicioAlumnos : IServicioAlumnos
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ServicioAlumnos> _logger;

    public ServicioAlumnos(HttpClient httpClient, ILogger<ServicioAlumnos> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger;
    }

    public async Task<IEnumerable<Alumno>> DameAlumnos(int idPagina, int numRegistros)
    {
        try
        {
            string token = Environment.GetEnvironmentVariable("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            //return await _httpClient.GetFromJsonAsync<List<Alumno>>("Api/Alumnos") ?? new List<Alumno>(); // Antes
            List<Alumno> alu = await _httpClient.GetFromJsonAsync<List<Alumno>>("API/Alumnos/DameAlumnos/" + idPagina.ToString() + "/" + numRegistros.ToString());

            if (alu != null && alu[0].error != null && alu[0].error.mensaje != String.Empty)
            {
                if (alu[0].error.mostrarUsuario)
                {
                    _logger.LogError("Error obteniendo alumnos: " + alu[0].error.mensaje);
                    throw new Exception(alu[0].error.mensaje);
                }
                else
                {
                    _logger.LogError("Error obteniendo alumnos: " + alu[0].error.mensaje);
                    throw new Exception("Error obteniendo alumnos");
                }

            }

            return alu;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "DameAlumnos: Error al obtener alumnos.");
            return new List<Alumno>();
        }
    }

    public async Task<Alumno?> DameAlumnoPorId(int id)
    {
        try
        {
            string token = Environment.GetEnvironmentVariable("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.GetFromJsonAsync<Alumno>($"Api/Alumnos/{id}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"DameAlumnoPorId: Error al obtener alumno con ID {id}.");
            return null;
        }
    }

    public async Task<Alumno?> DameAlumnoPorEmail(string email)
    {
        try
        {
            string token = Environment.GetEnvironmentVariable("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.GetFromJsonAsync<Alumno>($"Api/Alumnos/{email}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"DameAlumnoPorEmail: Error al obtener alumno con email {email}.");
            return null;
        }
    }

    public async Task<Alumno?> AltaAlumno(Alumno alumno)
    {
        try
        {
            string token = Environment.GetEnvironmentVariable("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Api/Alumnos", alumno);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Alumno>();
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();
                string mensaje = errors?["Email"]?.FirstOrDefault() ?? "Datos inválidos en el email.";
                _logger.LogWarning($"AltaAlumno: Error de validación - {mensaje}");
                return null;
            }
            else
            {
                _logger.LogWarning($"AltaAlumno: Error en la solicitud - Código HTTP: {response.StatusCode}");
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "AltaAlumno: Error al conectar con el servidor.");
            return null;
        }
    }

    public async Task<Alumno?> ModificarAlumno(Alumno alumno)
    {
        try
        {
            string token = Environment.GetEnvironmentVariable("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"Api/Alumnos/{alumno.Id}", alumno);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Alumno>();
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();
                string mensaje = errors?["Email"]?.FirstOrDefault() ?? "Datos inválidos en el email.";
                _logger.LogWarning($"ModificarAlumno: Error de validación - {mensaje}");
                return null;
            }
            else
            {
                _logger.LogWarning($"ModificarAlumno: Error en la solicitud - Código HTTP: {response.StatusCode}");
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "ModificarAlumno: Error al conectar con el servidor.");
            return null;
        }
    }

    public async Task<Alumno?> BorrarAlumno(int id)
    {
        try
        {
            string token = Environment.GetEnvironmentVariable("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.DeleteAsync($"Api/Alumnos/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Alumno>();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogError($"BorrarAlumno: Alumno con ID {id} no encontrado.");
                return null;
            }
            else
            {
                _logger.LogError($"BorrarAlumno: Error en la solicitud - Código HTTP: {response.StatusCode}");
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "BorrarAlumno: Error al conectar con el servidor.");
            return null;
        }
    }

    public async Task<Alumno?> InscribirAlumnoCurso(int idAlumno, int idCurso, int idPrecio)
    {
        try
        {
            string token = Environment.GetEnvironmentVariable("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.PostAsync($"API/Alumnos/InscribirAlumno/{idAlumno}/{idCurso}/{idPrecio}", null);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Alumno>();
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();
                string mensaje = errors?.Values.FirstOrDefault()?.FirstOrDefault() ?? "Datos inválidos.";
                _logger.LogWarning($"InscribirAlumnoCurso: Error de validación - {mensaje}");
                return null;
            }
            else
            {
                _logger.LogWarning($"InscribirAlumnoCurso: Error en la solicitud - Código HTTP: {response.StatusCode}");
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "InscribirAlumnoCurso: Error al conectar con el servidor.");
            return null;
        }
    }

    public async Task<Alumno?> AlumnoCursos(int id)
    {
        try
        {
            string token = Environment.GetEnvironmentVariable("Token");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.GetFromJsonAsync<Alumno>($"Api/Alumnos/CursosAlumno/{id}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"AlumnoCursos: Error al obtener cursos del alumno con ID {id}.");
            return null;
        }
    }
}
