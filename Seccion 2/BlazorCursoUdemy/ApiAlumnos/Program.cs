using ApiAlumnos.Datos;
using ApiAlumnos.Repositorios;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging con NLog
builder.Host.ConfigureLogging((hostingContext, logging) =>
{
    logging.ClearProviders();
    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    logging.AddNLog(); // Asegúrate de que nlog.config esté en el root y copiado
});

// Configurar cadena de conexión desde appsettings.json
string cadenaConexion = builder.Configuration.GetConnectionString("DefaultConnection");

// Inyectar AccesoDatos
builder.Services.AddSingleton(new AccesoDatos(cadenaConexion));

//Anadir Interfaz
builder.Services.AddScoped<IRepositorioAlumnos, RepositorioAlumnos>();
builder.Services.AddScoped<IRepositorioCursos, RepositorioCursos>();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Crear carpeta Log si no existe
Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Log"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Log de prueba para verificar que NLog funciona
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogError("✅ Logger funcionando: mensaje de prueba desde Program.cs");

app.Run();
