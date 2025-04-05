using BlazorServer.Servicios;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ModeloClasesAlumnos;
using System;
using System.Threading.Tasks;

namespace BlazorServer.Pages
{
    public partial class NuevoCurso
    {
        [Inject]
        private IServicioCursos ServicioCursos { get; set; }

        [Inject]
        private NavigationManager Navigation { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        private Curso Curso { get; set; } = new Curso();
        private Precio Precio { get; set; } = new Precio();
        public bool IsLoading { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                //Curso.NombreCurso = "Nuevo Curso";
                Precio.Coste = 00.00;
                Precio.FechaInicio = DateTime.Now;
                Precio.FechaFin = DateTime.Now.AddDays(30);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task GuardarCurso()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Curso.NombreCurso))
                {
                    await JS.InvokeVoidAsync("alert", "Debe ingresar un nombre de curso.");
                    return;
                }

                if (Precio.Coste <= 0)
                {
                    await JS.InvokeVoidAsync("alert", "Debe ingresar un precio válido.");
                    return;
                }

                if (Precio.FechaInicio >= Precio.FechaFin)
                {
                    await JS.InvokeVoidAsync("alert", "La fecha de inicio debe ser anterior a la fecha de fin.");
                    return;
                }

                // Asociar el precio al curso antes de enviarlo
                Curso.ListaPrecios = new List<Precio> { Precio };

                var exito = await ServicioCursos.AltaCurso(Curso);
                if (exito != null)
                {
                    Console.WriteLine("Curso creado correctamente.");
                    Navigation.NavigateTo("/ListaCursos");
                }
                else
                {
                    await JS.InvokeVoidAsync("alert", "Error al crear el curso.");
                    Console.WriteLine("Error al crear el curso.");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("alert", $"Error: {ex.Message}");
                Console.WriteLine($"Error al guardar el curso: {ex.Message}");
            }
        }

        private void Cancelar()
        {
            Navigation.NavigateTo("/ListaCursos");
        }
    }
}
