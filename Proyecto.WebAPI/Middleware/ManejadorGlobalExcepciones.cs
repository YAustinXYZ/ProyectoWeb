using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Proyecto.Domain.DTO.ResultadoPatron;
using Proyecto.Domain.Excepciones;

namespace Proyecto.WebAPI.Middleware;

/// <summary>
/// Punto único donde las excepciones se convierten en respuestas HTTP con el formato Resultado.
/// Las de negocio responden con su propio código (400, 404 o 409) y se registran como Warning;
/// cualquier otra es un fallo inesperado: responde 500 sin exponer detalles internos y se registra como Error.
/// </summary>
public sealed class ManejadorGlobalExcepciones : IExceptionHandler
{
    private readonly ILogger<ManejadorGlobalExcepciones> _registro;

    public ManejadorGlobalExcepciones(ILogger<ManejadorGlobalExcepciones> registro)
    {
        _registro = registro;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext contexto, Exception excepcion, CancellationToken cancelacion)
    {
        // Mismo TraceId que Serilog escribe en cada línea del log: así se cruza la respuesta con el log
        var identificadorTraza = Activity.Current?.TraceId.ToString() ?? contexto.TraceIdentifier;
        var ruta = $"{contexto.Request.Method} {contexto.Request.Path}";

        Resultado<object> respuesta;

        if (excepcion is ExcepcionNegocio negocio)
        {
            _registro.LogWarning("Regla de negocio incumplida en {Ruta} ({CodigoEstado}): {Mensaje}. TraceId {TraceId}",
                ruta, negocio.CodigoEstado, negocio.Message, identificadorTraza);

            respuesta = Resultado<object>.Fallo(negocio.CodigoEstado, negocio.Titulo, negocio.Message);
        }
        else
        {
            _registro.LogError(excepcion, "Error no controlado en {Ruta}. TraceId {TraceId}", ruta, identificadorTraza);

            respuesta = Resultado<object>.Fallo(StatusCodes.Status500InternalServerError, "Error interno",
                "Ocurrió un error inesperado. Reporte el traceId al administrador.");
        }

        respuesta.IdentificadorTraza = identificadorTraza;

        contexto.Response.StatusCode = respuesta.Estado;
        await contexto.Response.WriteAsJsonAsync(respuesta, cancelacion);

        return true;
    }
}
