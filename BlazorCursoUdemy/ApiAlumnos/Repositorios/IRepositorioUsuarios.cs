using ModeloClasesAlumnos;

namespace ApiAlumnos.Repositorios
{
    public interface IRepositorioUsuarios
    {
        Task<UsuarioLogin> AltaUsuario(UsuarioLogin Alumno);

        Task<UsuarioLogin> DameUsuario(int id);

        Task<UsuarioLogin> DameUsuario(string email);
    }
}
