using BlazorServer.Servicios;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ModeloClasesAlumnos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorServer.Pages
{
    public partial class ListaCursos
    {
        [Inject]
        private IServicioCursos ServicioCursos { get; set; }
        [Inject]
        private NavigationManager Navigation { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }

        private IEnumerable<Curso> ListaCursos1;
        private string Descripcion { get; set; }
        private bool MostrarModalEliminar { get; set; } = false;
        private int IdCursoSeleccionado { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ListaCursos1 = await ServicioCursos.DameCursos(-1);
        }

        private void VerCurso(int id, int idPrecio)
        {
            Navigation.NavigateTo($"/CursoDetalle/{id}/{idPrecio}/ver");
        }

        private void EditarCurso(int id, int idPrecio)
        {
            Navigation.NavigateTo($"/CursoDetalle/{id}/{idPrecio}/editar");
        }

        private void MostrarConfirmacionEliminar(int idCurso)
        {
            var curso = ListaCursos1.FirstOrDefault(c => c.Id == idCurso);
            IdCursoSeleccionado = idCurso;
            Descripcion = $"Esta acción eliminará el curso '{curso?.NombreCurso ?? "este curso"}' de forma permanente.";
            MostrarModalEliminar = true;
        }

        private async Task EliminarCurso(int id)
        {
            await ServicioCursos.BorrarCurso(id);
            ListaCursos1 = await ServicioCursos.DameCursos(-1);
            await JS.InvokeVoidAsync("alert", "Se elimino el curso correctamente.");

        }

        private void CrearCurso()
        {
            Navigation.NavigateTo("/NuevoCurso");
        }
    }
}
