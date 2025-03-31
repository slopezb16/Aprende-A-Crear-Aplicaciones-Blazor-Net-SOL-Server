using BlazorServer.Servicios;
using Microsoft.AspNetCore.Components;
using ModeloClasesAlumnos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorServer.Pages
{
    public partial class InscribirAlumno
    {
        [Inject]
        public IServicioAlumnos ServicioAlumnos { get; set; }

        [Inject]
        public IServicioCursos ServicioCursos { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Parameter]
        public int id { get; set; }

        public Alumno? Alumno { get; set; }
        public IEnumerable<Curso> ListaCursos { get; set; } //= new IEnumerable<Curso>();

        protected override async Task OnInitializedAsync()
        {
            if (id > 0)
            {
                Alumno = await ServicioAlumnos.DameAlumnoPorId(id);
                ListaCursos = await ServicioCursos.DameCursos(id);
            }
        }

        private async Task InscribirAlumnoEnCurso(int idCurso, int idPrecio)
        {
            if (Alumno != null)
            {
                try
                {
                    var exito = await ServicioAlumnos.InscribirAlumnoCurso(Alumno.Id, idCurso, idPrecio);
                    if (exito != null)
                    {
                        Navigation.NavigateTo($"/AlumnoCursosDetalle/{Alumno.Id}");
                    }
                    else
                    {
                        //mensajeError = "No se pudo inscribir al alumno. Intente nuevamente.";
                        Console.WriteLine("Error al inscribir al alumno en el curso.");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

    }
}
