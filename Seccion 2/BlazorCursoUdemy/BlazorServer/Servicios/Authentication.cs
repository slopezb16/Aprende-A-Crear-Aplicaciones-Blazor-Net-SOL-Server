using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorServer.Servicios
{
    public class Authentication : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorage;
        private ClaimsPrincipal _usuarioActual = new ClaimsPrincipal(new ClaimsIdentity());

        public Authentication(ISessionStorageService sessionStorage)
        {
            _sessionStorage = sessionStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var email = await _sessionStorage.GetItemAsync<string>("email");
            ClaimsIdentity identity;
            if (email != null)
            {
                identity = new ClaimsIdentity(new[] {
              new Claim(ClaimTypes.Name,email),
              }, "apiauth_type");
            }
            else
            {
                identity = new ClaimsIdentity();
            }


            var usuario = new ClaimsPrincipal(identity);
            return await Task.FromResult(new AuthenticationState(usuario));
        }

        public void UsuarioAutenticado(string email)
        {
            var identity = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier,email),}, "apiauth_type);"
         );

            var usuario = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(usuario)));
        }

        public void MarcarUsuarioComoAutenticado(string email)
        {
            var identity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, email),
        }, "apiauth");

            _usuarioActual = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_usuarioActual)));
        }

        public void MarcarUsuarioComoDesconectado()
        {
            _usuarioActual = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_usuarioActual)));
        }

        public async Task CerrarSesion()
        {
            await _sessionStorage.RemoveItemAsync("email");

            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}
