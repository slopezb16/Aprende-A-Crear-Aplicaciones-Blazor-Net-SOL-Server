using ApiAlumnos.Repositorios;
using Microsoft.AspNetCore.Mvc;
using ModeloClasesAlumnos;

namespace ApiAlumnos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CursosController : ControllerBase
    {
        private readonly IRepositorioCursos _cursosRepositorio;

        public CursosController(IRepositorioCursos cursosRepositorio)
        {
            _cursosRepositorio = cursosRepositorio;
        }

        [HttpPost]
        public async Task<IActionResult> AltaCurso([FromBody] Curso curso)
        {
            if (curso == null || curso.ListaPrecios == null || !curso.ListaPrecios.Any())
                return BadRequest("El curso y su lista de precios no pueden estar vacíos.");

            var cursoExistente = await _cursosRepositorio.DameCurso(curso.NombreCurso);
            if (cursoExistente != null)
                return Conflict("El curso ya está registrado.");

            var nuevoCurso = await _cursosRepositorio.AltaCurso(curso);
            return CreatedAtAction(nameof(DameCurso), new { id = nuevoCurso.Id }, nuevoCurso);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> BorrarCurso(int id)
        {
            var curso = await _cursosRepositorio.DameCurso(id);
            if (curso == null)
                return NotFound($"Curso con ID {id} no encontrado.");

            await _cursosRepositorio.BorrarCurso(id);
            return NoContent();
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> DameCurso(int id)
        {
            var curso = await _cursosRepositorio.DameCurso(id);
            if (curso == null)
                return NotFound($"Curso con ID {id} no encontrado.");

            return Ok(curso);
        }

        [HttpGet("nombre/{nombreCurso}")]
        public async Task<IActionResult> DameCurso(string nombreCurso)
        {
            var curso = await _cursosRepositorio.DameCurso(nombreCurso);
            if (curso == null)
                return NotFound($"Curso '{nombreCurso}' no encontrado.");

            return Ok(curso);
        }

        [HttpGet]
        public async Task<IActionResult> DameCursos()
        {
            var cursos = await _cursosRepositorio.DameCursos();
            return Ok(cursos);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ModificarCurso(int id, [FromBody] Curso curso)
        {
            if (id != curso.Id)
                return BadRequest("El ID del curso no coincide.");

            var cursoExistente = await _cursosRepositorio.DameCurso(id);
            if (cursoExistente == null)
                return NotFound($"Curso con ID {id} no encontrado.");

            var cursoModificado = await _cursosRepositorio.ModificarCurso(curso);
            return Ok(cursoModificado);
        }
    }
}
