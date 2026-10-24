using MapsterMapper;
using Proyecto.Application.Interfaces;
using Proyecto.Application.Mapper;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;
using Proyecto.Domain.Entities;
using Proyecto.Domain.Excepciones;
using Proyecto.Infraestructure.Interfaces;

namespace Proyecto.Application.Implementation;

public class ActivoService : IActivoService
{
    // Límite de registros por página
    public const int TamanoMaximoPagina = 50;

    public const string EstadoBaja = "Decommissioned";
    public const string EstadoEnMantenimiento = "UnderMaintenance";

    // Estados en los que una orden se considera cerrada; cualquier otro estado impide la baja del activo
    public static readonly string[] EstadosOrdenCerrada = ["Completed", "Rejected", "Cancelled"];

    public const long TamanoMaximoImagen = 2 * 1024 * 1024;
    public static readonly string[] ExtensionesImagenPermitidas = [".jpg", ".jpeg", ".png", ".webp"];

    private readonly IActivoRepository _repositorio;
    private readonly IMapper _mapeador;
    private readonly IAlmacenamientoArchivosService _almacenamientoArchivos;

    public ActivoService(IActivoRepository repositorio, IMapper mapeador, IAlmacenamientoArchivosService fileStorage)
    {
        _repositorio = repositorio;
        _mapeador = mapeador;
        _almacenamientoArchivos = fileStorage;
    }

    // CONSULTA

    public async Task<Resultado<IEnumerable<ActivoListaDto>>> ObtenerTodosAsync(int? categoriaId = null, string? estado = null)
    {
        var assets = await _repositorio.ObtenerTodosAsync(categoriaId, estado);

        var datos = await MapearListaConConteoAsync(assets);

        var resultado = Resultado<IEnumerable<ActivoListaDto>>.Exito(datos);

        return resultado;
    }

    public async Task<Resultado<IEnumerable<ActivoListaDto>>> ObtenerPaginacionAsync(int paginaNumero, int tamanoPagina, int? categoriaId = null, string? estado = null)
    {
        if (paginaNumero < 1 || tamanoPagina < 1 || tamanoPagina > TamanoMaximoPagina)
            throw new ExcepcionSolicitudInvalida($"El número de página debe ser 1 o mayor y el tamaño debe estar entre 1 y {TamanoMaximoPagina}.");

        var assets = await _repositorio.ObtenerPaginacionAsync(paginaNumero, tamanoPagina, categoriaId, estado);
        var registrosTotales = await _repositorio.ContarAsync(categoriaId, estado);

        var datos = await MapearListaConConteoAsync(assets);

        var resultado = Resultado<IEnumerable<ActivoListaDto>>.Paginado(datos, paginaNumero, tamanoPagina, registrosTotales);

        return resultado;
    }

    public async Task<Resultado<ActivoDetalleDto>> ObtenerPorIdentificadorAsync(int id)
    {
        // La consulta por id sí muestra un activo dado de baja: su estado y su historial se siguen consultando
        var activo = await _repositorio.ObtenerPorIdentificadorAsync(id);

        if (activo is null)
            throw new ExcepcionNoEncontrado($"El activo {id} no existe.");

        var datos = await MapearDetalleAsync(activo);

        var resultado = Resultado<ActivoDetalleDto>.Exito(datos);

        return resultado;
    }

    // CREACIÓN
    public async Task<Resultado<ActivoDetalleDto>> AgregarAsync(ActivoCrearDto activo)
    {
        var codePrefix = await _repositorio.ObtenerPrefijoCodigoCategoriaAsync(activo.CategoriaActivoId);

        if (codePrefix is null)
            throw new ExcepcionSolicitudInvalida($"La categoría {activo.CategoriaActivoId} no existe.");

        var error = await ValidateRelationsAsync(activo.UbicacionId, activo.ProveedorId);

        if (error is not null)
            throw new ExcepcionSolicitudInvalida(error);

        // Regla de negocio: el número de serie no se puede repetir
        if (!string.IsNullOrWhiteSpace(activo.NumeroSerie)
            && await _repositorio.ExistePorNumeroSerieAsync(activo.NumeroSerie))
            throw new ExcepcionConflicto($"Ya existe un activo con el número de serie {activo.NumeroSerie}.");

        var entidad = _mapeador.Map<Activo>(activo);
        entidad.CodigoActivo = await GenerateAssetCodeAsync(codePrefix);
        entidad.Estado = "Operational";
        entidad.HorasUsoAlUltimoMantenimiento = 0;
        entidad.CreadoEn = DateTime.UtcNow;

        await _repositorio.AgregarAsync(entidad);        // SQL Server asigna el Identificador

        // Se vuelve a consultar para incluir los nombres de categoría, ubicación y proveedor
        var created = await _repositorio.ObtenerPorIdentificadorAsync(entidad.Identificador);

        var datos = await MapearDetalleAsync(created!);

        var resultado = Resultado<ActivoDetalleDto>.Exito(datos, 201, "Created", "Registro guardado.");

        return resultado;
    }

    // ACTUALIZACIÓN

    public async Task<Resultado<ActivoDetalleDto>> ActualizarAsync(int id, ActivoActualizarDto activo)
    {
        var entidad = await _repositorio.ObtenerParaActualizarAsync(id);

        if (entidad is null)
            throw new ExcepcionNoEncontrado($"El activo {id} no existe o está dado de baja.");

        if (await _repositorio.ObtenerPrefijoCodigoCategoriaAsync(activo.CategoriaActivoId) is null)
            throw new ExcepcionSolicitudInvalida($"La categoría {activo.CategoriaActivoId} no existe.");

        var error = await ValidateRelationsAsync(activo.UbicacionId, activo.ProveedorId);

        if (error is not null)
            throw new ExcepcionSolicitudInvalida(error);

        // Evita violar la restricción CK_Assets_UsageHours de la base de datos
        if (activo.HorasUso < entidad.HorasUsoAlUltimoMantenimiento)
            throw new ExcepcionSolicitudInvalida($"Las horas de uso no pueden ser menores que las registradas en el último mantenimiento ({entidad.HorasUsoAlUltimoMantenimiento}).");

        if (!string.IsNullOrWhiteSpace(activo.NumeroSerie)
            && await _repositorio.ExistePorNumeroSerieAsync(activo.NumeroSerie, id))
            throw new ExcepcionConflicto($"Ya existe un activo con el número de serie {activo.NumeroSerie}.");

        _mapeador.Map(activo, entidad);

        await _repositorio.GuardarCambiosAsync();

        var updated = await _repositorio.ObtenerPorIdentificadorAsync(id);

        var datos = await MapearDetalleAsync(updated!);

        var resultado = Resultado<ActivoDetalleDto>.Exito(datos, 200, "Correcto", "Registro actualizado.");

        return resultado;
    }

    public async Task<Resultado<ActivoDetalleDto>> ActualizarEstadoAsync(int id, ActualizarEstadoActivoDto estado)
    {
        var entidad = await _repositorio.ObtenerParaActualizarAsync(id);

        if (entidad is null)
            throw new ExcepcionNoEncontrado($"El activo {id} no existe o está dado de baja.");

        // Regla de negocio: mientras haya una orden abierta, el estado del activo lo controla la orden.
        // Cubre el paso a OutOfService y a Decommissioned (misma regla que DELETE).
        if (entidad.Estado != estado.Estado
            && await _repositorio.TieneOrdenesAbiertasAsync(id, EstadosOrdenCerrada))
            throw new ExcepcionConflicto($"El activo {id} tiene una orden de mantenimiento abierta y no se puede cambiar a {estado.Estado}.");

        entidad.Estado = estado.Estado!;

        await _repositorio.GuardarCambiosAsync();

        var updated = await _repositorio.ObtenerPorIdentificadorAsync(id);

        var datos = await MapearDetalleAsync(updated!);

        var resultado = Resultado<ActivoDetalleDto>.Exito(datos, 200, "Correcto", "Estado actualizado.");

        return resultado;
    }

    // ELIMINAR

    // Baja lógica: la fila se conserva para mantener el historial de órdenes y el código del activo
    public async Task<Resultado<int>> EliminarAsync(int id)
    {
        var entidad = await _repositorio.ObtenerParaActualizarAsync(id);

        if (entidad is null)
            throw new ExcepcionNoEncontrado($"El activo {id} no existe o está dado de baja.");

        // Regla de negocio (enunciado 7.1 y rúbrica C11, C12 y C19): con órdenes asociadas el activo no se
        // borra físicamente, se da de baja lógica; solo se bloquea (409) si alguna orden sigue abierta.
        // Es la misma regla que aplica PATCH /status al pasar a Decommissioned.
        if (await _repositorio.TieneOrdenesAbiertasAsync(id, EstadosOrdenCerrada))
            throw new ExcepcionConflicto($"El activo {id} tiene órdenes de mantenimiento sin cerrar y no se puede dar de baja.");

        entidad.Estado = EstadoBaja;

        await _repositorio.GuardarCambiosAsync();

        var resultado = Resultado<int>.Exito(id, 200, "Correcto", "Activo dado de baja.");

        return resultado;
    }

    // IMAGE

    // Asigna la imagen o la reemplaza; al reemplazarla se borra el archivo anterior
    public async Task<Resultado<string>> SubirImagenAsync(int id, Stream content, string nombreArchivo, long length)
    {
        var entidad = await _repositorio.ObtenerParaActualizarAsync(id);

        if (entidad is null)
            throw new ExcepcionNoEncontrado($"El activo {id} no existe o está dado de baja.");

        if (length == 0)
            throw new ExcepcionSolicitudInvalida("El archivo está vacío.");

        if (length > TamanoMaximoImagen)
            throw new ExcepcionSolicitudInvalida("La imagen no puede superar 2 MB.");

        var extension = Path.GetExtension(nombreArchivo).ToLowerInvariant();

        if (!ExtensionesImagenPermitidas.Contains(extension))
            throw new ExcepcionSolicitudInvalida($"La imagen debe tener una de estas extensiones: {string.Join(", ", ExtensionesImagenPermitidas)}.");

        // El nombre lo genera el servidor: evita colisiones y no usa el nombre que envía el cliente
        var newFileName = $"{Guid.NewGuid()}{extension}";
        var oldFileName = entidad.Imagen;

        await _almacenamientoArchivos.GuardarAsync(content, newFileName);

        entidad.Imagen = newFileName;

        try
        {
            await _repositorio.GuardarCambiosAsync();
        }
        catch
        {
            // Sin este borrado, el archivo quedaría en disco sin ningún activo que lo referencie
            _almacenamientoArchivos.Eliminar(newFileName);
            throw;
        }

        DeleteStoredImage(oldFileName);

        var resultado = Resultado<string>.Exito(newFileName, 200, "Correcto", "Imagen guardada.");

        return resultado;
    }

    public async Task<Resultado<int>> EliminarImagenAsync(int id)
    {
        var entidad = await _repositorio.ObtenerParaActualizarAsync(id);

        if (entidad is null)
            throw new ExcepcionNoEncontrado($"El activo {id} no existe o está dado de baja.");

        if (string.IsNullOrWhiteSpace(entidad.Imagen))
            throw new ExcepcionNoEncontrado($"El activo {id} no tiene imagen.");

        var oldFileName = entidad.Imagen;

        entidad.Imagen = string.Empty;

        // Primero la base de datos: si falla, el archivo se conserva y el activo sigue apuntando a él
        await _repositorio.GuardarCambiosAsync();

        DeleteStoredImage(oldFileName);

        var resultado = Resultado<int>.Exito(id, 200, "Correcto", "Imagen eliminada.");

        return resultado;
    }

    // Mapea el listado y completa la cantidad de órdenes con un solo conteo agrupado en SQL
    private async Task<IEnumerable<ActivoListaDto>> MapearListaConConteoAsync(IEnumerable<Activo> activos)
    {
        var datos = _mapeador.Map<List<ActivoListaDto>>(activos);
        var conteos = await _repositorio.ContarOrdenesPorActivoAsync(datos.Select(a => a.Identificador));

        foreach (var activo in datos)
            activo.CantidadOrdenes = conteos.GetValueOrDefault(activo.Identificador);

        return datos;
    }

    private async Task<ActivoDetalleDto> MapearDetalleAsync(Activo activo)
    {
        var datos = _mapeador.Map<ActivoDetalleDto>(activo);
        var conteos = await _repositorio.ContarOrdenesPorActivoAsync([activo.Identificador]);

        datos.CantidadOrdenes = conteos.GetValueOrDefault(activo.Identificador);

        return datos;
    }

    // La ubicación debe existir y el proveedor también, si se indica. Devuelve null si no hay error.
    private async Task<string?> ValidateRelationsAsync(int locationId, int? supplierId)
    {
        if (!await _repositorio.ExisteUbicacionAsync(locationId))
            return $"La ubicación {locationId} no existe.";

        if (supplierId is not null && !await _repositorio.ExisteProveedorAsync(supplierId.Value))
            return $"El proveedor {supplierId} no existe.";

        return null;
    }

    // Prefijo de la categoría + consecutivo de 4 dígitos: SRV-0001, SRV-0002...
    private async Task<string> GenerateAssetCodeAsync(string codePrefix)
    {
        var lastCode = await _repositorio.ObtenerUltimoCodigoActivoAsync(codePrefix);

        var lastNumber = lastCode is not null && int.TryParse(lastCode[(codePrefix.Length + 1)..], out var number)
            ? number
            : 0;

        return $"{codePrefix}-{lastNumber + 1:D4}";
    }

    // La imagen por defecto es compartida por todos los activos y nunca se borra.
    // La comparación ignora mayúsculas porque el sistema de archivos de Windows también lo hace.
    private void DeleteStoredImage(string? nombreArchivo)
    {
        if (string.IsNullOrWhiteSpace(nombreArchivo)
            || string.Equals(Path.GetFileName(nombreArchivo), ConfiguracionMapster.ImagenPredeterminadaActivo, StringComparison.OrdinalIgnoreCase))
            return;

        _almacenamientoArchivos.Eliminar(nombreArchivo);
    }
}
