using ApiAlumnos.Repositorios;
using Microsoft.AspNetCore.Mvc;
using ModeloClasesAlumnos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiAlumnos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlumnosController : ControllerBase
    {

        private readonly IRepositorioAlumnos _repositorioAlumnos;

        public AlumnosController(IRepositorioAlumnos repositorioAlumnos)
        {
            this._repositorioAlumnos = repositorioAlumnos;
        }

        // GET: api/<AlumnosController>
        [HttpGet]
        public async Task<ActionResult> GetAlumnos()
        {
            try
            {
                   return Ok(await _repositorioAlumnos.DameAlumnos());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo los Datos");
            }
        }

        // GET api/<AlumnosController>/5
        //[HttpGet("{id:int}")]
        //public async Task<ActionResult> Get(int id)
        //{
        //    try
        //    {
        //        return Ok(await _repositorioAlumnos.DameAlumno(id));
        //    }
        //    catch (KeyNotFoundException ex)
        //    {
        //        return NotFound(ex.Message);  // Retorna 404 si no se encuentra el alumno
        //    }
        //    catch (Exception)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo los Datos");
        //    }
        //}

        // GET api/<AlumnosController>/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult> Get(int id)
        {
            try
            {
                var result = await _repositorioAlumnos.DameAlumno(id);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo los Datos");
            }
        }

        // POST api/<AlumnosController>
        [HttpPost]
        public async Task<ActionResult<Alumno>> Post(Alumno alumno)
        {
            try
            {
                if (alumno == null)
                {
                    return BadRequest();
                }

                var validacionEmail = await _repositorioAlumnos.DameAlumno(alumno.Email);
                if (validacionEmail != null)
                {
                    ModelState.AddModelError("Email", "El email ya esta en uso");
                    return BadRequest(ModelState);
                }

                var result = await _repositorioAlumnos.AltaAlumno(alumno);

                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Creando el alumno");
            }
        }

        // PUT api/<AlumnosController>/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Alumno>> Put(int id, Alumno alumno)
        {
            try
            {
                if (id != alumno.Id)
                    return BadRequest("Alumno Id no coincide");

                var AlumnoModificar = await _repositorioAlumnos.DameAlumno(id);
                if (AlumnoModificar == null)
                    return NotFound($"Alumno con Id {id} no encontrado");

                var result = await _repositorioAlumnos.ModificarAlumno(alumno);

                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error actualizando el alumno");
            }
        }

        // DELETE api/<AlumnosController>/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Alumno>> Delete(int id)
        {
            try
            {
                var alumnoExistente = await _repositorioAlumnos.DameAlumno(id);
                if (alumnoExistente == null)
                    return NotFound($"Alumno con Id {id} no encontrado");

                var result = await _repositorioAlumnos.BorrarAlumno(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error al eliminar el alumno");
            }
        }

        [HttpGet("BuscarAlumnos")]
        public async Task<ActionResult<IEnumerable<Alumno>>> Buscar(string texto)
        {
            try
            {
                var alumnos = await _repositorioAlumnos.BuscarAlumnos(texto);

                if (!alumnos.Any())
                    return NotFound("No se encontraron alumnos con el criterio especificado.");

                return Ok(alumnos);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error buscando alumnos");
            }
        }

        [HttpPost("InscribirAlumno/{idAlumno}/{idCurso}")]
        public async Task<ActionResult<Alumno>> InscribirAlumnoCurso(int idAlumno, int idCurso)
        {
            try
            {
                var alumnoValidar = await _repositorioAlumnos.DameAlumno(idAlumno);

                if (alumnoValidar == null)
                    return NotFound("Alumno no encontrado");

                return await _repositorioAlumnos.InscribirAlumnoCurso(idAlumno, idCurso);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error inscribiendo alumno en curso");
            }
        }

        [HttpGet("CursosAlumno/{idAlumno}")]
        public async Task<ActionResult<Alumno>> AlumnoCursos(int idAlumno)
        {
            try
            {
                var alumnoValidar = await _repositorioAlumnos.DameAlumno(idAlumno);

                if (alumnoValidar == null)
                    return NotFound($"Alumno no encontrado");

                var result = await _repositorioAlumnos.AlumnoCursos(idAlumno);
                //return result;
                // Si el resultado es null, devolvemos un Alumno con una lista vacía de cursos
                return result ?? new Alumno
                {
                    Id = idAlumno,
                    Nombre = alumnoValidar.Nombre,
                    Email = alumnoValidar.Email,
                    ListaCursos = new List<Curso>()
                };
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error obteniendo cursos alumno");
            }
        }
    }
}
