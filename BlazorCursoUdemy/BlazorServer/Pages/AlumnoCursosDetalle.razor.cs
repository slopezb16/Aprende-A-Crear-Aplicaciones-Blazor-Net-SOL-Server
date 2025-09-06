using BlazorServer.Servicios;
using Microsoft.AspNetCore.Components;
using ModeloClasesAlumnos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlazorServer.Pages
{
    public partial class AlumnoCursosDetalle
    {
        [Inject]
        public IServicioAlumnos ServicioAlumnos { get; set; }

        [Inject]
        public NavigationManager Navigation { get; set; }

        [Parameter]
        public int idAlumno { get; set; }

        public Alumno? Alumno { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (idAlumno > 0)
            {
                Alumno = await ServicioAlumnos.AlumnoCursos(idAlumno);

                // Asegurar que ListaCursos y ListaPrecios no sean null
                if (Alumno?.ListaCursos != null)
                {
                    foreach (var curso in Alumno.ListaCursos)
                    {
                        curso.ListaPrecios ??= new List<Precio>();
                    }
                }

                Console.WriteLine($"Alumno cargado: {Alumno?.Nombre}, Cursos: {Alumno?.ListaCursos?.Count}");
            }
        }
    }
}
