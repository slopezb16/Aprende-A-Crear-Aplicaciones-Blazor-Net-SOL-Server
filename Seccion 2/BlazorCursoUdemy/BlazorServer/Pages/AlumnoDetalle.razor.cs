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
    public partial class AlumnoDetalle
    {
        [Inject] 
        public IServicioAlumnos ServicioAlumnos { get; set; }
        [Inject] 
        public NavigationManager Navigation { get; set; }
        [Inject] 
        public IJSRuntime JS { get; set; }

        [Parameter] public int Id { get; set; }
        [Parameter] public string Modo { get; set; }

        public Alumno? Alumno { get; set; }
        public bool IsLoading { get; set; } = true;
        public bool EsModoEdicion => Modo?.ToLower() == "editar";

        public List<string> FotosDisponibles { get; set; } = new();
        private string _fotoSeleccionada;
        public string FotoSeleccionada
        {
            get => _fotoSeleccionada;
            set
            {
                _fotoSeleccionada = value;
                if (Alumno != null)
                {
                    Alumno.Foto = _fotoSeleccionada;
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Alumno = await ServicioAlumnos.DameAlumnoPorId(Id);
                if (Alumno == null)
                {
                    Console.WriteLine("Alumno no encontrado.");
                }

                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (Directory.Exists(path))
                {
                    FotosDisponibles = Directory.GetFiles(path)
                        .Select(f => "/images/" + Path.GetFileName(f))
                        .ToList();
                }

                if (FotosDisponibles.Any() && Alumno != null && string.IsNullOrEmpty(Alumno.Foto))
                {
                    FotoSeleccionada = FotosDisponibles[0];
                }
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

        private void EditarAlumno()
        {
            Navigation.NavigateTo($"/AlumnoDetalle/{Id}/editar");
        }

        public async Task GuardarCambios()
        {
            if (Alumno != null)
            {
                try
                {
                    var exito = await ServicioAlumnos.ModificarAlumno(Alumno);

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
                    Console.WriteLine($"Error en la actualización: {ex.Message}");
                }
            }
        }

        private async Task BorrarAlumno()
        {
            if (Alumno != null)
            {
                bool confirmacion = await ConfirmarEliminacion();
                if (confirmacion)
                {
                    var exito = await ServicioAlumnos.BorrarAlumno(Alumno.Id);

                    if (exito != null)
                    {
                        Console.WriteLine("Alumno eliminado correctamente.");
                        Navigation.NavigateTo("/ListaAlumnos");
                    }
                    else
                    {
                        await JS.InvokeVoidAsync("alert", "Error al eliminar el alumno.");
                        Console.WriteLine("Error al eliminar el alumno.");
                    }
                }
            }
        }

        private async Task<bool> ConfirmarEliminacion()
        {
            return await JS.InvokeAsync<bool>("confirm", "¿Estás seguro de que deseas eliminar este alumno?");
        }

        private async Task ActivarAlumno(Alumno alumno)
        {
            Alumno = await ServicioAlumnos.DameAlumnoPorId(alumno.Id);
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

        public void Cancelar()
        {
            Navigation.NavigateTo("/ListaAlumnos");
        }
    }
}
