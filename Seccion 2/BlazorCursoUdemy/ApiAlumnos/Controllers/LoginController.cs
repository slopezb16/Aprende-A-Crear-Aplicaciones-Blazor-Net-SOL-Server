using ApiAlumnos.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ModeloClasesAlumnos;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.PortableExecutable;
using System.Security.Claims;
using System.Text;

namespace ApiAlumnos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginController> _logger;
        private readonly IRepositorioUsuarios _usuariosRepositorio;

        public LoginController(IConfiguration configuration, ILogger<LoginController> logger, IRepositorioUsuarios usuariosRepositorio)
        {
            _configuration = configuration;
            _logger = logger;
            _usuariosRepositorio = usuariosRepositorio;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UsuarioAPI>> Login(Login usuarioLogin)
        {
            try
            {
                var infoUsuario = await AutenticarUsuarioAsync(usuarioLogin.Usuario, usuarioLogin.Password);
                if (infoUsuario == null)
                {
                    return Unauthorized("Credenciales no válidas");
                }

                infoUsuario.Token = GenerarTokenJWT(infoUsuario.Email);

                return Ok(infoUsuario);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error al autenticar usuario: " + ex.ToString());
                return StatusCode(500, "Error interno al procesar la solicitud");
            }
        }

        // Autenticación de prueba local (puedes reemplazar con validación desde BBDD)
        private async Task<UsuarioAPI> AutenticarUsuarioAsync(string usuario, string password)
        {
            // Simula un pequeño delay (opcional)
            await Task.Delay(100);

            if (_configuration["UsuarioAPI"] == usuario && _configuration["UsuarioAPI"] == password)
            {
                return new UsuarioAPI
                {
                    Nombre = _configuration["NombreUsuario"],
                    Apellidos = _configuration["ApellidosUsuario"],
                    Email = _configuration["EmmailUsuario"]
                };
            }

            return null;
        }

        // Genera un JWT válido a partir del email
        private string GenerarTokenJWT(string email)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("usuario", email),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("CrearUsuario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UsuarioLogin>> CrearUsuario(UsuarioLogin usuario)
        {
            try
            {
                if (usuario == null)
                    return BadRequest("Datos inválidos");

                var resultado = await _usuariosRepositorio.AltaUsuario(usuario);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error en CrearUsuario: " + ex.ToString());
                return StatusCode(500, "Error interno");
            }
        }

        [HttpPost("ValidarUsuario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<UsuarioLogin>> ValidarUsuario(UsuarioLogin usuario)
        {
            try
            {
                if (usuario == null)
                    return BadRequest("Datos inválidos");

                var resultado = await _usuariosRepositorio.DameUsuario(usuario.EmailLogin);
                if (resultado == null || resultado.Password != usuario.Password)
                    return Unauthorized("Credenciales no válidas");

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error en ValidarUsuario: " + ex.ToString());
                return StatusCode(500, "Error interno");
            }
        }
    }
}
