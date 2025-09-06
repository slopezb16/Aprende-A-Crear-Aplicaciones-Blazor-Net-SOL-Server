using Microsoft.AspNetCore.Components;
using BlazorServer.Servicios;
using Microsoft.AspNetCore.Components.Authorization;
using ModeloClasesAlumnos;
using Blazored.SessionStorage;

namespace BlazorServer.Pages
{
    public partial class LoginPage
    {
        private string textoError = string.Empty;
        private bool mostrarModal = false;

        private Login login = new();
        private UsuarioLogin usuarioLogin = new();
        private UsuarioAPI usuarioAPI = new();

        [Inject]
        public IServicioLogin ServicioLogin { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject]
        public ISessionStorageService AlmacenarSesion { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                login.Usuario = Environment.GetEnvironmentVariable("UsuarioAPI");
                login.Password = Environment.GetEnvironmentVariable("PassAPI");

                usuarioAPI = await ServicioLogin.SolicitudLogin(login);
                Environment.SetEnvironmentVariable("Token", usuarioAPI.Token);
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
        }

        private async Task ValidarUsuario()
        {
            try
            {

                if (!string.IsNullOrWhiteSpace(usuarioLogin.EmailLogin) &&
                    !string.IsNullOrWhiteSpace(usuarioLogin.Password))
                {
                    //Prueba
                    //((Authentication)AuthenticationStateProvider).UsuarioAutenticado(usuarioLogin.EmailLogin);
                    //await AlmacenarSesion.SetItemAsync("email", usuarioLogin.EmailLogin);

                    var usuario = await ServicioLogin.ValidarUsuario(usuarioLogin);

                    await AlmacenarSesion.SetItemAsync("email", usuario.EmailLogin);

                    // Notificar autenticación
                    if (AuthenticationStateProvider is Authentication customAuth)
                    {
                        customAuth.MarcarUsuarioComoAutenticado(usuario.EmailLogin);
                    }

                    NavigationManager.NavigateTo("/Index", forceLoad: true);
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                MostrarError(ex.Message);
            }
        }

        private void IrARegistro()
        {
            NavigationManager.NavigateTo("/RegistroLogin");
        }

        private void MostrarError(string mensaje)
        {
            textoError = mensaje;
            mostrarModal = true;
        }

        private string GetParticleStyle(int index)
        {
            var rand = new Random(Guid.NewGuid().GetHashCode()); // más aleatoriedad

            var top = rand.Next(0, 100);
            var left = rand.Next(0, 100);
            var size = rand.Next(6, 58); // entre 6px y 14px
            var duration = rand.Next(8, 62); // duración de animación en segundos
            var delay = rand.Next(0, 5); // retardo para que no comiencen al tiempo

            return $"top: {top}vh; left: {left}vw; width: {size}px; height: {size}px; animation-duration: {duration}s; animation-delay: {delay}s;";
        }

    }
}
