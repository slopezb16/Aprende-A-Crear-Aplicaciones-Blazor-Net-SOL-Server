using BlazorServer.Servicios;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ModeloClasesAlumnos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorServer.Pages
{
    public partial class NuevoAlumno
    {
        [Inject]
        public IServicioAlumnos ServicioAlumnos { get; set; }
        [Inject]
        public NavigationManager Navigation { get; set; }
        [Inject]
        public IJSRuntime JS { get; set; }


        public Alumno Alumno { get; set; } = new Alumno();
        public List<string> FotosDisponibles { get; set; } = new();

        private string _fotoSeleccionada;
        public string FotoSeleccionada
        {
            get => _fotoSeleccionada;
            set
            {
                _fotoSeleccionada = value;
                Alumno.Foto = _fotoSeleccionada; // Se actualiza automáticamente
            }
        }

        public bool IsLoading { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Obtener las imágenes desde wwwroot/images
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (Directory.Exists(path))
                {
                    FotosDisponibles = Directory.GetFiles(path)
                        .Select(f => "/images/" + Path.GetFileName(f)) // Ajustar la ruta relativa
                        .ToList();
                }

                // Seleccionar la primera foto como predeterminada si hay imágenes
                if (FotosDisponibles.Any())
                {
                    FotoSeleccionada = FotosDisponibles[0]; // Se ejecuta el setter y actualiza Alumno.Foto
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar imágenes: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task GuardarAlumno()
        {
            try
            {
                Alumno.FechaAlta = DateTime.Today;
                var exito = await ServicioAlumnos.AltaAlumno(Alumno);

                if (exito != null)
                {
                    Console.WriteLine("Alumno creado correctamente.");
                    Navigation.NavigateTo("/ListaAlumnos");
                }
                else
                {
                    await JS.InvokeVoidAsync("alert", "Error al crear el alumno.");
                    Console.WriteLine("Error al crear el alumno.");
                }
            }
            catch (Exception ex)
            {
                await JS.InvokeVoidAsync("alert", ex.Message);
                Console.WriteLine($"Error al guardar el alumno: {ex.Message}");
            }
        }

        public void Cancelar()
        {
            Navigation.NavigateTo("/ListaAlumnos");
        }
    }
}
