using BlazorServer.Servicios;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ModeloClasesAlumnos;
using System;
using System.Threading.Tasks;

namespace BlazorServer.Pages
{
    public partial class CursoDetalle
    {
        [Inject]
        public IServicioCursos ServicioCursos { get; set; }
        [Inject]
        public NavigationManager Navigation { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }

        [Parameter]
        public int Id { get; set; }
        [Parameter]
        public int IdPrecio { get; set; }
        [Parameter]
        public string Modo { get; set; }

        public Curso Curso { get; set; }
        public Precio Precio { get; set; } = new Precio();
        public Precio NuevoPrecio { get; set; } = new Precio();

        public bool EsNuevoPrecio { get; set; } = false;
        public bool IsLoading { get; set; } = true;
        public bool EsModoEdicion => Modo?.ToLower() == "editar";
        private bool MostrarModalEliminar { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Curso = await ServicioCursos.DameCurso(Id, IdPrecio);

                if (Curso == null)
                {
                    Console.WriteLine("Curso no encontrado.");
                    Navigation.NavigateTo("/ListaCursos");
                    return;
                }

                // Buscar el precio específico en la lista de precios del curso
                Precio = Curso.ListaPrecios[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar datos: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void AgregarNuevoPrecio()
        {
            EsNuevoPrecio = true;
            NuevoPrecio = new Precio();
            NuevoPrecio.Coste = 00.00;
            NuevoPrecio.FechaInicio = DateTime.Now;
            NuevoPrecio.FechaFin = DateTime.Now.AddDays(30);
        }

        private void CancelarNuevoPrecio()
        {
            EsNuevoPrecio = false;
            NuevoPrecio = new Precio();
        }

        private void EditarAlumno()
        {
            Navigation.NavigateTo($"/CursoDetalle/{Id}/{IdPrecio}/editar");
        }

        public async Task GuardarCambios()
        {
            try
            {
                Curso.ListaPrecios[0] = Precio;
                var cursoActualizado = await ServicioCursos.ModificarCurso(Curso);

                if (cursoActualizado != null)
                {
                    Console.WriteLine("Curso actualizado correctamente.");
                    Navigation.NavigateTo("/ListaCursos");
                }
                else
                {
                    await JS.InvokeVoidAsync("alert", "Error al actualizar el curso.");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("alert", ex.Message);
                Console.WriteLine($"Error en la actualización: {ex.Message}");
            }
        }

        public async Task GuardarPrecio()
        {
            try
            {
                if (NuevoPrecio.Coste >= 0 && NuevoPrecio.FechaFin != null && NuevoPrecio.FechaInicio != null)
                {

                    Curso.ListaPrecios[0] = Precio;
                    Curso.ListaPrecios.Add(NuevoPrecio);

                    Curso = await ServicioCursos.ModificarCurso(Curso);

                    Navigation.NavigateTo("/listaCursos");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("alert", ex.Message);
                Console.WriteLine($"Error en la actualización: {ex.Message}");
            }
        }

        public async Task BorrarCurso(int id)
        {
            if (Curso != null)
            {
                try
                {
                    var eliminado = await ServicioCursos.BorrarCurso(Id);

                    if (eliminado != null)
                    {
                        Console.WriteLine("Curso eliminado correctamente.");
                        Navigation.NavigateTo("/ListaCursos");
                        await JS.InvokeVoidAsync("alert", "Se elimino el curso correctamente.");
                    }
                    else
                    {
                        await JS.InvokeVoidAsync("alert", "Error al eliminar el curso.");
                    }
                }
                catch (Exception ex)
                {
                    await JS.InvokeVoidAsync("alert", ex.Message);
                    Console.WriteLine($"Error al eliminar: {ex.Message}");
                }
            }
        }

        public void Cancelar()
        {
            Navigation.NavigateTo("/ListaCursos");
        }
    }
}
