using Blazored.SessionStorage;
using BlazorServer.Servicios;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging con NLog
builder.Host.ConfigureLogging((hostingContext, logging) =>
{
    logging.ClearProviders();
    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    logging.AddNLog(); // Asegúrate de que nlog.config esté en el root y copiado
});

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
builder.Services.AddHttpClient<IServicioLogin, ServicioLogin>(cliente =>
{
    cliente.BaseAddress = new Uri(apiBaseUrl);
});

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddScoped<AuthenticationStateProvider, Authentication>();

var app = builder.Build();

// Crear carpeta Log si no existe
Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Log"));

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

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");


// Log de prueba para verificar que NLog funciona
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogError("✅ Logger funcionando: mensaje de prueba desde Program.cs");

app.Run();
