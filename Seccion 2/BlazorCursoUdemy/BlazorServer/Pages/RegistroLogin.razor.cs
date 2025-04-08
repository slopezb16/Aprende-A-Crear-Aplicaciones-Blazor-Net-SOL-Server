using Microsoft.AspNetCore.Components;
using BlazorServer.Servicios;
using Microsoft.AspNetCore.Components.Authorization;
using ModeloClasesAlumnos;

namespace BlazorServer.Pages
{
    public partial class RegistroLogin
    {
        private string iconError = string.Empty;
        private string textoError = string.Empty;
        private string tipoError = string.Empty;
        private bool mostrarModal = false;

        private Login login = new();
        private UsuarioLogin usuarioRegistro = new();
        private UsuarioAPI usuarioAPI = new();

        [Inject] 
        public IServicioLogin ServicioLogin { get; set; }
        [Inject] 
        public NavigationManager NavigationManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                login.Usuario = Environment.GetEnvironmentVariable("UsuarioAPI");
                login.Password = Environment.GetEnvironmentVariable("PassAPI");

                usuarioAPI = await ServicioLogin.SolicitudLogin(login);
                Environment.SetEnvironmentVariable("Token", usuarioAPI.Token);

                StateHasChanged();
            }
            catch (Exception ex)
            {
                MostrarModal("Error", ex.Message, "error");
            }
        }

        private async Task RegistrarUsuario()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(usuarioRegistro.EmailLogin) &&
                    !string.IsNullOrWhiteSpace(usuarioRegistro.Password))
                {
                    var resultado = await ServicioLogin.CrearUsuario(usuarioRegistro);
                    // Puedes mostrar un mensaje de éxito o redirigir al login
                    MostrarModal("success", "Usuario registrado con exito, puedes iniciar sesion", "success");
                    //NavigationManager.NavigateTo("/LoginPage");
                }
            }
            catch (Exception ex)
            {
                MostrarModal("Error", ex.Message, "error");
            }
        }

        private void IrALogin()
        {
            NavigationManager.NavigateTo("/LoginPage");
        }

        private void MostrarModal(string icon,string mensaje, string tipo)
        {
            iconError = icon;
            textoError = mensaje;
            tipoError = tipo;
            mostrarModal = true;
        }

        private string GetParticleStyle(int index)
        {
            var rand = new Random(Guid.NewGuid().GetHashCode());

            var top = rand.Next(0, 100);
            var left = rand.Next(0, 100);
            var size = rand.Next(6, 58); // entre 6px y 14px
            var duration = rand.Next(8, 62); // duración de animación en segundos
            var delay = rand.Next(0, 5); // retardo para que no comiencen al tiempo

            return $"top: {top}vh; left: {left}vw; width: {size}px; height: {size}px; animation-duration: {duration}s; animation-delay: {delay}s;";
        }
    }
}
