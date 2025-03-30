using ModeloClasesAlumnos;

namespace ApiAlumnos.Repositorios
{
    public interface IRepositorioCursos
    {
        //Daremos de alta un curso y su precio
        Task<Curso> AltaCurso(Curso curso);
        //Devolveremos todos los cursos con sus precios
        Task<IEnumerable<Curso>> DameCursos();
        //Devolvera los datos de un curso con sus precios buscando por id
        Task<Curso> DameCurso(int id);
        //Devolvera los datos de un curso con sus precios buscando por curso
        Task<Curso> DameCurso(string nombreCurso);
        //Podremos modificar los datos de un curso y sus precios
        Task<Curso> ModificarCurso(Curso curso);
        //Borrar curso
        Task<Curso> BorrarCurso(int id);
    }
}
