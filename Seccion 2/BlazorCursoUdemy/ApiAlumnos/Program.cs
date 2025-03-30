using ApiAlumnos.Datos;
using ApiAlumnos.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// Configurar cadena de conexión desde appsettings.json
string cadenaConexion = builder.Configuration.GetConnectionString("DefaultConnection");

// Inyectar AccesoDatos
builder.Services.AddSingleton(new AccesoDatos(cadenaConexion));

// Add services to the container.

builder.Services.AddControllers();

//Anadir Interfaz
builder.Services.AddScoped<IRepositorioAlumnos, RepositorioAlumnos>();
builder.Services.AddScoped<IRepositorioCursos, RepositorioCursos>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
