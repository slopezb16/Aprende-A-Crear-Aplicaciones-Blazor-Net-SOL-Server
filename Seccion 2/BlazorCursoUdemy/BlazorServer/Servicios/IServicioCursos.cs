using ModeloClasesAlumnos;

namespace BlazorServer.Servicios
{
    public interface IServicioCursos
    {
        //Daremos de alta un curso y su precio
        Task<Curso> AltaCurso(Curso curso);
        //Devolveremos todos los cursos con sus precios
        Task<IEnumerable<Curso>> DameCursos(int idAlumno);
        //
        Task<Curso> DameCurso(int id, int idPrecio);

        ////Devolvera los datos de un curso con sus precios buscando por curso
        //Task<Curso> DameCurso(string nombreCurso);
        //Podremos modificar los datos de un curso y sus precios
        Task<Curso> ModificarCurso(Curso curso);
        //Borrar curso
        Task<Curso> BorrarCurso(int id);
    }
}
