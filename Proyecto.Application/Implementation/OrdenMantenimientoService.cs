using MapsterMapper;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;
using Proyecto.Domain.Entities;
using Proyecto.Domain.Excepciones;
using Proyecto.Infraestructure.Interfaces;

namespace Proyecto.Application.Implementation;

public class OrdenMantenimientoService : IOrdenMantenimientoService
{
    // Límite de registros por página
    public const int TamanoMaximoPagina = 50;

    public const string EstadoSolicitado = "Requested";

    private readonly IOrdenMantenimientoRepository _repositorio;
    private readonly IMapper _mapeador;

    public OrdenMantenimientoService(IOrdenMantenimientoRepository repositorio, IMapper mapeador)
    {
        _repositorio = repositorio;
        _mapeador = mapeador;
    }

    // CONSULTA

    public async Task<Resultado<IEnumerable<OrdenMantenimientoListaDto>>> ObtenerTodosAsync()
    {
        var orders = await _repositorio.ObtenerTodosAsync();

        var datos = _mapeador.Map<IEnumerable<OrdenMantenimientoListaDto>>(orders);

        var resultado = Resultado<IEnumerable<OrdenMantenimientoListaDto>>.Exito(datos);

        return resultado;
    }

    public async Task<Resultado<IEnumerable<OrdenMantenimientoListaDto>>> ObtenerPaginacionAsync(int paginaNumero, int tamanoPagina)
    {
        if (paginaNumero < 1 || tamanoPagina < 1 || tamanoPagina > TamanoMaximoPagina)
            throw new ExcepcionSolicitudInvalida($"El número de página debe ser 1 o mayor y el tamaño debe estar entre 1 y {TamanoMaximoPagina}.");

        var orders = await _repositorio.ObtenerPaginacionAsync(paginaNumero, tamanoPagina);
        var registrosTotales = await _repositorio.ContarAsync();

        var datos = _mapeador.Map<IEnumerable<OrdenMantenimientoListaDto>>(orders);

        var resultado = Resultado<IEnumerable<OrdenMantenimientoListaDto>>.Paginado(datos, paginaNumero, tamanoPagina, registrosTotales);

        return resultado;
    }

    public async Task<Resultado<OrdenMantenimientoDetalleDto>> ObtenerPorIdentificadorAsync(int id)
    {
        var orden = await _repositorio.ObtenerPorIdentificadorAsync(id);

        if (orden is null)
            throw new ExcepcionNoEncontrado($"La orden {id} no existe.");

        var datos = _mapeador.Map<OrdenMantenimientoDetalleDto>(orden);

        var resultado = Resultado<OrdenMantenimientoDetalleDto>.Exito(datos);

        return resultado;
    }

    // Un activo sin órdenes devuelve una lista vacía; un activo inexistente, 404
    public async Task<Resultado<IEnumerable<OrdenMantenimientoListaDto>>> ObtenerPorActivoAsync(int assetId)
    {
        if (await _repositorio.ObtenerEstadoActivoAsync(assetId) is null)
            throw new ExcepcionNoEncontrado($"El activo {assetId} no existe.");

        var orders = await _repositorio.ObtenerPorActivoAsync(assetId);

        var datos = _mapeador.Map<IEnumerable<OrdenMantenimientoListaDto>>(orders);

        var resultado = Resultado<IEnumerable<OrdenMantenimientoListaDto>>.Exito(datos);

        return resultado;
    }

    // CREACIÓN
    public async Task<Resultado<OrdenMantenimientoDetalleDto>> AgregarAsync(OrdenMantenimientoCrearDto orden)
    {
        var assetStatus = await _repositorio.ObtenerEstadoActivoAsync(orden.ActivoId);

        // La rúbrica pide 404 cuando el activo no existe; los demás Ids del cuerpo siguen el patrón
        // de clase (un Id de relación inexistente es un cuerpo inválido: 400)
        if (assetStatus is null)
            throw new ExcepcionNoEncontrado($"El activo {orden.ActivoId} no existe.");

        var isEmergency = await _repositorio.ObtenerSiTipoMantenimientoEsEmergenciaAsync(orden.TipoMantenimientoId);

        if (isEmergency is null)
            throw new ExcepcionSolicitudInvalida($"El tipo de mantenimiento {orden.TipoMantenimientoId} no existe.");

        bool? supplierIsActive = null;

        if (orden.ProveedorId is not null)
        {
            supplierIsActive = await _repositorio.ObtenerSiProveedorEstaActivoAsync(orden.ProveedorId.Value);

            if (supplierIsActive is null)
                throw new ExcepcionSolicitudInvalida($"El proveedor {orden.ProveedorId} no existe.");
        }

        // Reglas de negocio sobre el estado de los registros relacionados

        if (assetStatus == ActivoService.EstadoBaja)
            throw new ExcepcionConflicto($"El activo {orden.ActivoId} está dado de baja y no puede recibir órdenes.");

        if (supplierIsActive == false)
            throw new ExcepcionConflicto($"El proveedor {orden.ProveedorId} está inactivo.");

        // Un tipo de emergencia permite abrir otra orden aunque el activo ya esté en mantenimiento
        if (assetStatus == ActivoService.EstadoEnMantenimiento
            && isEmergency == false
            && await _repositorio.TieneOrdenesAbiertasAsync(orden.ActivoId, ActivoService.EstadosOrdenCerrada))
            throw new ExcepcionConflicto($"El activo {orden.ActivoId} está en mantenimiento y tiene una orden sin cerrar; solo se permite una orden de emergencia.");

        var now = DateTime.UtcNow;

        var entidad = _mapeador.Map<OrdenMantenimiento>(orden);
        entidad.CodigoOrdenTrabajo = await GenerateWorkOrderCodeAsync(now.Year);
        entidad.Estado = EstadoSolicitado;
        entidad.SolicitadoEn = now;
        entidad.CreadoEn = now;

        await _repositorio.AgregarAsync(entidad);        // SQL Server asigna el Identificador

        // Se vuelve a consultar para incluir los datos del activo, el tipo y el proveedor
        var created = await _repositorio.ObtenerPorIdentificadorAsync(entidad.Identificador);

        var datos = _mapeador.Map<OrdenMantenimientoDetalleDto>(created!);

        var resultado = Resultado<OrdenMantenimientoDetalleDto>.Exito(datos, 201, "Created", "Registro guardado.");

        return resultado;
    }

    // OT-{año}-{consecutivo de 4 dígitos}: OT-2026-0001, OT-2026-0002...
    private async Task<string> GenerateWorkOrderCodeAsync(int year)
    {
        var codePrefix = $"OT-{year}-";
        var lastCode = await _repositorio.ObtenerUltimoCodigoOrdenTrabajoAsync(codePrefix);

        var lastNumber = lastCode is not null && int.TryParse(lastCode[codePrefix.Length..], out var number)
            ? number
            : 0;

        return $"{codePrefix}{lastNumber + 1:D4}";
    }
}
