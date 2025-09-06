using ApiAlumnos.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModeloClasesAlumnos;

namespace ApiAlumnos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CursosController : ControllerBase
    {
        private readonly IRepositorioCursos _cursosRepositorio;
        private readonly ILogger<CursosController> _logger;

        public CursosController(IRepositorioCursos cursosRepositorio, ILogger<CursosController> logger)
        {
            _cursosRepositorio = cursosRepositorio;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AltaCurso([FromBody] Curso curso)
        {
            if (curso == null || curso.ListaPrecios == null || !curso.ListaPrecios.Any())
            {
                _logger.LogWarning("AltaCurso: Curso inválido o sin lista de precios.");
                return BadRequest("El curso y su lista de precios no pueden estar vacíos.");
            }

            try
            {
                var cursoExistente = await _cursosRepositorio.DameCurso(curso.NombreCurso);
                if (cursoExistente != null)
                {
                    _logger.LogInformation($"AltaCurso: El curso '{curso.NombreCurso}' ya existe.");
                    return Conflict("El curso ya está registrado.");
                }

                var nuevoCurso = await _cursosRepositorio.AltaCurso(curso);
                _logger.LogInformation($"AltaCurso: Curso '{nuevoCurso.NombreCurso}' creado con ID {nuevoCurso.Id}.");
                return CreatedAtAction(nameof(DameCurso), new { id = nuevoCurso.Id }, nuevoCurso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AltaCurso: Error creando curso '{curso?.NombreCurso}'.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al crear el curso.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BorrarCurso(int id)
        {
            try
            {
                var curso = await _cursosRepositorio.DameCurso(id);
                if (curso == null)
                {
                    _logger.LogWarning($"BorrarCurso: Curso con ID {id} no encontrado.");
                    return NotFound($"Curso con ID {id} no encontrado.");
                }

                var result = await _cursosRepositorio.BorrarCurso(id);
                _logger.LogInformation($"BorrarCurso: Curso con ID {id} eliminado correctamente.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"BorrarCurso: Error borrando curso con ID {id}.", ex.Message, ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, "Error borrando el curso.");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> DameCurso(int id)
        {
            try
            {
                var curso = await _cursosRepositorio.DameCurso(id);
                if (curso == null)
                {
                    _logger.LogWarning($"DameCurso (por ID): Curso con ID {id} no encontrado.");
                    return NotFound($"Curso con ID {id} no encontrado.");
                }

                return Ok(curso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DameCurso (por ID): Error al obtener curso con ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener el curso.");
            }
        }

        [HttpGet("{id:int}/{idPrecio:int}")]
        public async Task<ActionResult<Curso>> DameCurso(int id, int idPrecio)
        {
            try
            {
                var curso = await _cursosRepositorio.DameCurso(id, idPrecio);
                if (curso == null)
                {
                    _logger.LogWarning($"DameCurso (por ID y Precio): Curso con ID {id} no encontrado.");
                    return NotFound($"Curso con ID {id} no encontrado.");
                }

                return Ok(curso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DameCurso (por ID y Precio): Error al obtener curso con ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener el curso.");
            }
        }

        [HttpGet("nombre/{nombreCurso}")]
        public async Task<IActionResult> DameCurso(string nombreCurso)
        {
            try
            {
                var curso = await _cursosRepositorio.DameCurso(nombreCurso);
                if (curso == null)
                {
                    _logger.LogWarning($"DameCurso (por nombre): Curso '{nombreCurso}' no encontrado.");
                    return NotFound($"Curso '{nombreCurso}' no encontrado.");
                }

                return Ok(curso);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DameCurso (por nombre): Error al obtener curso '{nombreCurso}'.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener el curso.");
            }
        }

        [HttpGet("AlumnosCursos/{id:int}")]
        public async Task<IActionResult> DameCursos(int id)
        {
            try
            {
                var cursos = await _cursosRepositorio.DameCursos(id);
                _logger.LogInformation($"DameCursos: Cursos para alumno con ID {id} recuperados correctamente.");
                return Ok(cursos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DameCursos: Error al obtener cursos para alumno con ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al obtener los cursos.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ModificarCurso(int id, [FromBody] Curso curso)
        {
            if (id != curso.Id)
            {
                _logger.LogWarning("ModificarCurso: El ID del curso no coincide.");
                return BadRequest("El ID del curso no coincide.");
            }

            try
            {
                var cursoExistente = await _cursosRepositorio.DameCurso(id);
                if (cursoExistente == null)
                {
                    _logger.LogWarning($"ModificarCurso: Curso con ID {id} no encontrado.");
                    return NotFound($"Curso con ID {id} no encontrado.");
                }

                var cursoModificado = await _cursosRepositorio.ModificarCurso(curso);
                _logger.LogInformation($"ModificarCurso: Curso con ID {id} modificado correctamente.");
                return Ok(cursoModificado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ModificarCurso: Error modificando curso con ID {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error modificando el curso.");
            }
        }
    }
}
