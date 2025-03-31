using BlazorServer.Servicios;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Cargar configuración desde appsettings.json
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiBaseUrl") ?? "https://localhost:44319/";

// Agregar servicios
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configurar HttpClient con la URL de la API desde appsettings.json
builder.Services.AddHttpClient<IServicioAlumnos, ServicioAlumnos>(cliente =>
{
    cliente.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddHttpClient<IServicioCursos, ServicioCursos>(cliente =>
{
    cliente.BaseAddress = new Uri(apiBaseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
