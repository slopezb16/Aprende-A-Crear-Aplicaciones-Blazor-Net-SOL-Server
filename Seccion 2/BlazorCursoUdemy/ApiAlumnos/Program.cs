using ApiAlumnos.Datos;
using ApiAlumnos.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models; // 👈 Necesario para Swagger JWT
using ModeloClasesAlumnos;
using NLog.Extensions.Logging;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging con NLog
builder.Host.ConfigureLogging((hostingContext, logging) =>
{
    logging.ClearProviders();
    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    logging.AddNLog();
});

// Configurar cadena de conexión
string cadenaConexion = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSingleton(new AccesoDatos(cadenaConexion));

// Inyección de repositorios
builder.Services.AddScoped<IRepositorioAlumnos, RepositorioAlumnos>();
builder.Services.AddScoped<IRepositorioCursos, RepositorioCursos>();
builder.Services.AddScoped<IRepositorioUsuarios, RepositorioUsuarios>();

// Configurar autenticación JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var secretKey = jwtSettings["Key"];

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorization();

// Agregar controladores
builder.Services.AddControllers();

// Agregar Swagger con configuración JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ApiAlumnos", Version = "v1" });

    // 👇 Configuración para que aparezca el botón "Authorize"
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en el campo. Ejemplo: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Crear carpeta Log si no existe
Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Log"));

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // opcional
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // 👈 Importante
app.UseAuthorization();

app.MapControllers();

// Log de prueba
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogError("✅ Logger funcionando: mensaje de prueba desde Program.cs");

app.Run();
