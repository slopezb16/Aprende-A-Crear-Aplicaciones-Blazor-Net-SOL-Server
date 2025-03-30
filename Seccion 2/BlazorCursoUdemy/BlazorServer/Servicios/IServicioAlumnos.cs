using ModeloClasesAlumnos;

namespace BlazorServer.Servicios
{
    public interface IServicioAlumnos
    {
        Task<IEnumerable<Alumno>> DameAlumnos();
        Task<Alumno?> DameAlumnoPorId(int id);
        Task<Alumno> DameAlumnoPorEmail(string email);
        Task<Alumno> AltaAlumno(Alumno alumno);
        Task<Alumno> ModificarAlumno(Alumno alumno);
        Task<Alumno> BorrarAlumno(int id);
        //Task<IEnumerable<Alumno>> BuscarAlumnos(string texto);
        ////Inscribir Alumnos en curso
        Task<Alumno> InscribirAlumnoCurso(int idAlumno, int idCurso);
        ////Devuelve los datos de un alumno y todos sus cursos
        Task<Alumno> AlumnoCursos(int idAlumno);
    }
}
