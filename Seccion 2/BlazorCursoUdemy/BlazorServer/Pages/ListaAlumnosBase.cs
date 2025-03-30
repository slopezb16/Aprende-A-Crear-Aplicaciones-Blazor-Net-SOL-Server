using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModeloClasesAlumnos;
using BlazorServer.Servicios;

namespace BlazorServer.Pages
{
    public class ListaAlumnosBase : ComponentBase
    {
        [Inject]
        public IServicioAlumnos ServicioAlumnos { get; set; }

        public IEnumerable<Alumno> Alumnos { get; set; }

        //API
        protected override async Task OnInitializedAsync()
        {
            Alumnos = (await ServicioAlumnos.DameAlumnos()).ToList();
        }

        //protected override Task OnInitializedAsync()
        //{
        //    CargarAlumnos();
        //    return base.OnInitializedAsync();
        //}

        // Prueba antes del Api
        //private void CargarAlumnos()
        //{

        //    Precio precioBlazor = new Precio();
        //    precioBlazor.Id = 1;
        //    precioBlazor.Coste = 19.99;
        //    precioBlazor.FechaInicio = DateTime.Now;
        //    precioBlazor.FechaFin = DateTime.Now.AddDays(3);

        //    Curso cursoBlazor = new Curso();
        //    cursoBlazor.Id = 1;
        //    cursoBlazor.NombreCurso = "Curso Blazor";
        //    cursoBlazor.ListaPrecios = new List<Precio>();
        //    cursoBlazor.ListaPrecios.Add(precioBlazor);

        //    Alumno alumno1 = new Alumno
        //    {
        //        Id = 1,
        //        Nombre = "Jap Software",
        //        Email = "Mail@pruebamail.es",
        //        Foto = "images/Alumno1.jpg",
        //        ListaCursos = new List<Curso>(),
        //        FechaAlta = DateTime.Now,
        //        FechaBaja = null,

        //    };

        //    Alumno alumno2 = new Alumno
        //    {
        //        Id = 2,
        //        Nombre = "Jap Software 2",
        //        Email = "Mail2@pruebamail.es",
        //        Foto = "images/Alumno2.jpg",
        //        ListaCursos = new List<Curso>(),
        //        FechaAlta = DateTime.Now,
        //        FechaBaja = null,

        //    };

        //    Alumno alumno3 = new Alumno
        //    {
        //        Id = 3,
        //        Nombre = "Jap Software 3",
        //        Email = "Mail3@pruebamail.es",
        //        Foto = "images/ChicaCodigo65.jpg",
        //        ListaCursos = new List<Curso>(),
        //        FechaAlta = DateTime.Now,
        //        FechaBaja = null,

        //    };

        //    alumno1.ListaCursos.Add(cursoBlazor);
        //    alumno2.ListaCursos.Add(cursoBlazor);
        //    alumno3.ListaCursos.Add(cursoBlazor);

        //    Alumnos = new List<Alumno> { alumno1, alumno2, alumno3 };
        //}
    }

}
