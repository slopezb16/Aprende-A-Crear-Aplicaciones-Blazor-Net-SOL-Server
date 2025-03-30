using BlazorServer.Servicios;
using ModeloClasesAlumnos;
using System.Net;
using System.Net.Http;

public class ServicioAlumnos : IServicioAlumnos
{
    private readonly HttpClient _httpClient;

    public ServicioAlumnos(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<IEnumerable<Alumno>> DameAlumnos()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<Alumno>>("Api/Alumnos") ?? new List<Alumno>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error al obtener alumnos: {ex.Message}");
            return new List<Alumno>();
        }
    }

    public async Task<Alumno?> DameAlumnoPorId(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Alumno>($"Api/Alumnos/{id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error al obtener alumno con ID {id}: {ex.Message}");
            return null;
        }
    }

    public async Task<Alumno?> DameAlumnoPorEmail(string email)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Alumno>($"Api/Alumnos/{email}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error al obtener alumno con ID {email}: {ex.Message}");
            return null;
        }
    }

    public async Task<Alumno?> AltaAlumno(Alumno alumno)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("Api/Alumnos", alumno);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Alumno>();
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();
                if (errors != null && errors.ContainsKey("Email"))
                {
                    string mensaje = errors["Email"].FirstOrDefault() ?? "Error desconocido en el email.";
                    throw new Exception(mensaje);
                }
                else
                {
                    throw new Exception("Datos inválidos. Verifica la información ingresada.");
                }
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

    public async Task<Alumno?> ModificarAlumno(Alumno alumno)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"Api/Alumnos/{alumno.Id}", alumno);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Alumno>();
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();
                if (errors != null && errors.ContainsKey("Email"))
                {
                    string mensaje = errors["Email"].FirstOrDefault() ?? "Error desconocido en el email.";
                    throw new Exception(mensaje);
                }
                else
                {
                    throw new Exception("Datos inválidos. Verifica la información ingresada.");
                }
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

    public async Task<Alumno?> BorrarAlumno(int id)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"Api/Alumnos/{id}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Alumno>();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                Console.WriteLine($"Alumno con ID {id} no encontrado.");
                return null;
            }
            else
            {
                Console.WriteLine($"Error en la solicitud: {response.StatusCode}");
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Error al conectar con el servidor: {ex.Message}");
        }
    }

    public async Task<Alumno> InscribirAlumnoCurso(int idAlumno, int idCurso)
    {
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsync($"API/Alumnos/InscribirAlumno/{idAlumno}/{idCurso}", null);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Alumno>();
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var errors = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();
                string mensaje = errors?.Values.FirstOrDefault()?.FirstOrDefault() ?? "Datos inválidos.";
                throw new Exception(mensaje);
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

    public async Task<Alumno?> AlumnoCursos(int id)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<Alumno>($"Api/Alumnos/CursosAlumno/{id}");
            return result;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error al obtener alumno con ID {id}: {ex.Message}");
            return null;
        }
    }
}