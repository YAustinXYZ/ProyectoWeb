using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto.Application.Implementation;
using Proyecto.Application.Interfaces;
using Proyecto.Application.Mapper;
using Proyecto.Domain.DTO.ResultadoPatron;
using Proyecto.Infraestructure.Data;
using Proyecto.Infraestructure.Implementation;
using Proyecto.Infraestructure.Interfaces;
using Proyecto.WebAPI.Middleware;
using Scalar.AspNetCore;
using Serilog;

var constructor = WebApplication.CreateBuilder(args);

// Serilog: consola para desarrollo y un archivo diario en Logs/ para revisar errores después.
// Los niveles se leen de la sección "Serilog" de appsettings.json.
constructor.Host.UseSerilog((contexto, configuracionLog) =>
{
    configuracionLog
        .ReadFrom.Configuration(contexto.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console();

    // Las pruebas automatizadas (entorno Testing) no escriben archivos de log
    if (!contexto.HostingEnvironment.IsEnvironment("Testing"))
        configuracionLog.WriteTo.File(Path.Combine("Logs", "log-.txt"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14);
});

// Agrega los servicios al contenedor.

constructor.Services.AddControllers()
    .ConfigureApiBehaviorOptions(opciones =>
    {
        // Sin esto, un DTO inválido responde con el ValidationProblemDetails de ASP.NET y no con Resultado<T>
        opciones.InvalidModelStateResponseFactory = contexto =>
        {
            var errores = contexto.ModelState
                .Where(entrada => entrada.Value!.Errors.Count > 0)
                .ToDictionary(
                    entrada => entrada.Key,
                    entrada => entrada.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

            var resultado = Resultado<Dictionary<string, string[]>>.Fallo(400, "Solicitud incorrecta",
                "Uno o más datos no son válidos.");
            resultado.Datos = errores;
            resultado.IdentificadorTraza = System.Diagnostics.Activity.Current?.Id ?? contexto.HttpContext.TraceIdentifier;

            return new BadRequestObjectResult(resultado);
        };
    });
// Toda excepción termina aquí: las de negocio como 400/404/409 y el resto como 500 con traceId
constructor.Services.AddExceptionHandler<ManejadorGlobalExcepciones>();
constructor.Services.AddProblemDetails();

// Más información sobre OpenAPI: https://aka.ms/aspnet/openapi
constructor.Services.AddOpenApi();
constructor.Services.AddDbContext<ContextoAplicacion>(opciones =>
    opciones.UseSqlServer(constructor.Configuration.GetConnectionString("DefaultConnection")));

// Catálogos (ubicaciones y tipos de mantenimiento)
constructor.Services.AddScoped<ICatalogoRepository, CatalogoRepository>();
constructor.Services.AddScoped<ICatalogoService, CatalogoService>();

// CategoriaActivo
constructor.Services.AddScoped<ICategoriaActivoRepository, CategoriaActivoRepository>();
constructor.Services.AddScoped<ICategoriaActivoService, CategoriaActivoService>();

// Activo
constructor.Services.AddScoped<IActivoRepository, ActivoRepository>();
constructor.Services.AddScoped<IActivoService, ActivoService>();

// Proveedor
constructor.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
constructor.Services.AddScoped<IProveedorService, ProveedorService>();

// OrdenMantenimiento
constructor.Services.AddScoped<IOrdenMantenimientoRepository, OrdenMantenimientoRepository>();
constructor.Services.AddScoped<IOrdenMantenimientoService, OrdenMantenimientoService>();

// Tecnico
constructor.Services.AddScoped<ITecnicoRepository, TecnicoRepository>();
constructor.Services.AddScoped<ITecnicoService, TecnicoService>();

// Especialidad
constructor.Services.AddScoped<IEspecialidadRepository, EspecialidadRepository>();
constructor.Services.AddScoped<IEspecialidadService, EspecialidadService>();

// Repuesto
constructor.Services.AddScoped<IRepuestoRepository, RepuestoRepository>();
constructor.Services.AddScoped<IRepuestoService, RepuestoService>();

// Mapster: Pattern Mapper
// 1. Registrar las configuraciones personalizadas
ConfiguracionMapster.RegistrarMapeos();
// 2. Tomar la configuración global donde quedaron registradas
var configuracion = TypeAdapterConfig.GlobalSettings;
// 3. Registrar Mapster en el contenedor para inyectar IMapper en los servicios
constructor.Services.AddSingleton(configuracion);
constructor.Services.AddScoped<IMapper, ServiceMapper>();
constructor.Services.AddMapster();

// Almacenamiento de imágenes de activos: una sola instancia para toda la aplicación
var rutaImagenesActivos = Path.Combine(constructor.Environment.ContentRootPath, "wwwroot", "images", "assets");
constructor.Services.AddSingleton<IAlmacenamientoArchivosService>(new AlmacenamientoArchivosLocalService(rutaImagenesActivos));

var aplicacion = constructor.Build();

// Debe ir primero para capturar las excepciones de todo lo que sigue en la canalización
aplicacion.UseExceptionHandler();
aplicacion.UseSerilogRequestLogging();

if (aplicacion.Environment.IsDevelopment())
{
    await using var alcance = aplicacion.Services.CreateAsyncScope();
    var baseDatos = alcance.ServiceProvider.GetRequiredService<ContextoAplicacion>();
    await baseDatos.Database.MigrateAsync();
}

// Configura la canalización de solicitudes HTTP.
if (aplicacion.Environment.IsDevelopment())
{
    aplicacion.MapOpenApi();
    aplicacion.MapScalarApiReference();
}

aplicacion.UseStaticFiles();             // Sirve wwwroot: /images/assets/{nombre}

aplicacion.UseAuthorization();

aplicacion.MapControllers();

aplicacion.Run();
