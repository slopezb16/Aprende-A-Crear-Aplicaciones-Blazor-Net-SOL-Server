using BlazorServer.Servicios;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ModeloClasesAlumnos;

namespace BlazorServer.Pages
{
    public partial class ListaAlumnos
    {
        [Inject]
        public IServicioAlumnos ServicioAlumnos { get; set; }
        [Inject]
        NavigationManager Navigation { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }

        public IEnumerable<Alumno> Alumnos { get; set; }
        public Alumno? Alumno { get; set; }
        public bool IsLoading { get; set; } = true;

        // API
        [Inject]
        public IServicioLogin ServicioLogin { get; set; }

        private Login login = new();
        private UsuarioAPI usuarioAPI = new();

        protected override async Task OnInitializedAsync()
        {
            //throw new Exception("Forzar");
            try
            {
                login.Usuario = Environment.GetEnvironmentVariable("UsuarioAPI");
                login.Password = Environment.GetEnvironmentVariable("PassAPI");

                usuarioAPI = await ServicioLogin.SolicitudLogin(login);
                Environment.SetEnvironmentVariable("Token", usuarioAPI.Token);

                Alumnos = (await ServicioAlumnos.DameAlumnos()).ToList();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar alumnos: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void CrearAlumno()
        {
            Navigation.NavigateTo("/NuevoAlumno");
        }

        private void VerAlumnoCursos(int id)
        {
            Navigation.NavigateTo($"/AlumnoCursosDetalle/{id}");
        }

        private void VerAlumno(int id)
        {
            Navigation.NavigateTo($"/AlumnoDetalle/{id}/ver");
        }

        private void EditarAlumno(int id)
        {
            Navigation.NavigateTo($"/AlumnoDetalle/{id}/editar");
        }

        private async Task BorrarAlumno(int id)
        {
            Alumno = await ServicioAlumnos.DameAlumnoPorId(id);
            if (Alumno == null)
            {
                Console.WriteLine("Alumno no encontrado.");
                return;
            }

            bool confirmacion = await ConfirmarEliminacion();
            if (confirmacion)
            {
                var exito = await ServicioAlumnos.BorrarAlumno(Alumno.Id);

                if (exito != null)
                {
                    Console.WriteLine("Alumno eliminado correctamente.");
                    await OnInitializedAsync();  // 🔄 Recarga la lista de alumnos
                }
                else
                {
                    await JS.InvokeVoidAsync("alert", "Error al eliminar el alumno.");
                    Console.WriteLine("Error al eliminar el alumno.");
                }
            }
        }

        private async Task<bool> ConfirmarEliminacion()
        {
            return await JS.InvokeAsync<bool>("confirm", "¿Estás seguro de que deseas eliminar este alumno?");
        }

        private async Task ActivarAlumno(int id)
        {
            Alumno = await ServicioAlumnos.DameAlumnoPorId(id);
            if (Alumno == null)
            {
                Console.WriteLine("Alumno no encontrado.");
                return;
            }

            bool confirmacion = await ConfirmarActivacion();
            if (confirmacion)
            {
                Alumno.FechaBaja = null;
                var exito = await ServicioAlumnos.ModificarAlumno(Alumno);

                if (exito != null)
                {
                    Console.WriteLine("Alumno activado correctamente.");
                    await OnInitializedAsync();  // 🔄 Recarga la lista de alumnos
                }
                else
                {
                    await JS.InvokeVoidAsync("alert", "Error al activado el alumno.");
                    Console.WriteLine("Error al activado el alumno.");
                }
            }
        }

        private async Task<bool> ConfirmarActivacion()
        {
            return await JS.InvokeAsync<bool>("confirm", "¿Estás seguro de que deseas activar este alumno?");
        }
    }
}
