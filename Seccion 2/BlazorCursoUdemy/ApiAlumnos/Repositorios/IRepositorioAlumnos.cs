using ModeloClasesAlumnos;

namespace ApiAlumnos.Repositorios
{
    public interface IRepositorioAlumnos
    {
        Task<Alumno> AltaAlumno(Alumno alumno);
        Task<IEnumerable<Alumno>> DameAlumnos();
        Task<Alumno> DameAlumno(int id);
        Task<Alumno> DameAlumno(string email);
        Task<Alumno> ModificarAlumno(Alumno alumno);
        Task<Alumno> BorrarAlumno(int id);
        Task<IEnumerable<Alumno>> BuscarAlumnos(string texto);
        //Inscribir Alumnos en curso
        Task<Alumno> InscribirAlumnoCurso(int idAlumno, int idCurso);
        //Devuelve los datos de un alumno y todos sus cursos
        Task<Alumno> AlumnoCursos(int idAlumno);
    }
}
