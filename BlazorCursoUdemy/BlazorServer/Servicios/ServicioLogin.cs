using ModeloClasesAlumnos;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BlazorServer.Servicios
{
    public class ServicioLogin : IServicioLogin
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<ServicioCursos> log;

        public ServicioLogin(HttpClient httpClient, ILogger<ServicioCursos> l)
        {
            this.httpClient = httpClient;
            this.log = l;
        }

        public async Task<UsuarioAPI> SolicitudLogin(Login l)
        {
            try
            {
                var response = await httpClient.PostAsJsonAsync("Api/Login/", l);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    log.LogError("Error autenticando Api: " + error);
                    throw new Exception("Error al autenticar con la Api");
                }

                return await response.Content.ReadFromJsonAsync<UsuarioAPI>() ?? throw new Exception("Respuesta inválida del servidor");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al autenticar con la Api: " + ex.Message);
            }
        }

        public async Task<UsuarioLogin> CrearUsuario(UsuarioLogin usuarioLogin)
        {
            string token = Environment.GetEnvironmentVariable("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.PostAsJsonAsync("Api/Login/CrearUsuario/", usuarioLogin);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                log.LogError("Error creando usuario: " + error);
                throw new Exception("Error creando usuario");
            }

            return await response.Content.ReadFromJsonAsync<UsuarioLogin>() ?? throw new Exception("Respuesta inválida del servidor");
        }

        public async Task<UsuarioLogin> ValidarUsuario(UsuarioLogin usuarioLogin)
        {
            string token = Environment.GetEnvironmentVariable("Token");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await httpClient.PostAsJsonAsync("Api/Login/ValidarUsuario/", usuarioLogin);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                log.LogError("Error validando usuario: " + error);
                throw new Exception("Usuario o contraseña incorrectos.");
            }

            return await response.Content.ReadFromJsonAsync<UsuarioLogin>() ?? throw new Exception("Respuesta inválida del servidor");
        }

    }
}
