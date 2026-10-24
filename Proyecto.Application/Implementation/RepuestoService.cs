using MapsterMapper;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;
using Proyecto.Domain.Entities;
using Proyecto.Domain.Excepciones;
using Proyecto.Infraestructure.Interfaces;

namespace Proyecto.Application.Implementation;

public class RepuestoService : IRepuestoService
{
    // Límite de registros por página
    public const int TamanoMaximoPagina = 50;

    private readonly IRepuestoRepository _repositorio;
    private readonly IMapper _mapeador;

    public RepuestoService(IRepuestoRepository repositorio, IMapper mapeador)
    {
        _repositorio = repositorio;
        _mapeador = mapeador;
    }

    // CONSULTA

    public async Task<Resultado<IEnumerable<RepuestoListaDto>>> ObtenerTodosAsync(bool soloBajoMinimo = false)
    {
        var repuestos = await _repositorio.ObtenerTodosAsync(soloBajoMinimo);

        var datos = await MapearListaConConteoAsync(repuestos);

        var resultado = Resultado<IEnumerable<RepuestoListaDto>>.Exito(datos);

        return resultado;
    }

    public async Task<Resultado<IEnumerable<RepuestoListaDto>>> ObtenerPaginacionAsync(int paginaNumero, int tamanoPagina)
    {
        if (paginaNumero < 1 || tamanoPagina < 1 || tamanoPagina > TamanoMaximoPagina)
            throw new ExcepcionSolicitudInvalida($"El número de página debe ser 1 o mayor y el tamaño debe estar entre 1 y {TamanoMaximoPagina}.");

        var repuestos = await _repositorio.ObtenerPaginacionAsync(paginaNumero, tamanoPagina);
        var registrosTotales = await _repositorio.ContarAsync();

        var datos = await MapearListaConConteoAsync(repuestos);

        var resultado = Resultado<IEnumerable<RepuestoListaDto>>.Paginado(datos, paginaNumero, tamanoPagina, registrosTotales);

        return resultado;
    }

    public async Task<Resultado<RepuestoDetalleDto>> ObtenerPorIdentificadorAsync(int id)
    {
        var repuesto = await _repositorio.ObtenerPorIdentificadorAsync(id);

        if (repuesto is null)
            throw new ExcepcionNoEncontrado($"El repuesto {id} no existe.");

        var datos = _mapeador.Map<RepuestoDetalleDto>(repuesto);
        var conteos = await _repositorio.ContarProveedoresPorRepuestoAsync([id]);
        datos.CantidadProveedores = conteos.GetValueOrDefault(id);        // Coincide con datos.Proveedores.Count

        var resultado = Resultado<RepuestoDetalleDto>.Exito(datos);

        return resultado;
    }

    // CREACIÓN

    public async Task<Resultado<RepuestoDetalleDto>> AgregarAsync(RepuestoCrearDto repuesto)
    {
        // Regla de negocio: el código no se puede repetir
        if (await _repositorio.ExistePorCodigoAsync(repuesto.Codigo!))
            throw new ExcepcionConflicto($"Ya existe un repuesto con el código {repuesto.Codigo}.");

        var entidad = _mapeador.Map<Repuesto>(repuesto);
        entidad.CreadoEn = DateTime.UtcNow;

        await _repositorio.AgregarAsync(entidad);

        var creado = await ObtenerPorIdentificadorAsync(entidad.Identificador);

        var resultado = Resultado<RepuestoDetalleDto>.Exito(creado.Datos!, 201, "Created", "Registro guardado.");

        return resultado;
    }

    // ACTUALIZACIÓN

    public async Task<Resultado<RepuestoDetalleDto>> ActualizarAsync(int id, RepuestoActualizarDto repuesto)
    {
        var entidad = await _repositorio.ObtenerParaActualizarAsync(id);

        if (entidad is null)
            throw new ExcepcionNoEncontrado($"El repuesto {id} no existe.");

        if (await _repositorio.ExistePorCodigoAsync(repuesto.Codigo!, id))
            throw new ExcepcionConflicto($"Ya existe un repuesto con el código {repuesto.Codigo}.");

        _mapeador.Map(repuesto, entidad);

        await _repositorio.GuardarCambiosAsync();

        var actualizado = await ObtenerPorIdentificadorAsync(id);

        var resultado = Resultado<RepuestoDetalleDto>.Exito(actualizado.Datos!, 200, "Correcto", "Registro actualizado.");

        return resultado;
    }

    // RELACIÓN N:N REPUESTO–PROVEEDOR

    public async Task<Resultado<RepuestoDetalleDto>> AsociarProveedorAsync(int repuestoId, int proveedorId, CondicionesProveedorRepuestoDto condiciones)
    {
        if (await _repositorio.ObtenerParaActualizarAsync(repuestoId) is null)
            throw new ExcepcionNoEncontrado($"El repuesto {repuestoId} no existe.");

        var proveedorActivo = await _repositorio.ObtenerSiProveedorEstaActivoAsync(proveedorId);

        if (proveedorActivo is null)
            throw new ExcepcionNoEncontrado($"El proveedor {proveedorId} no existe.");

        if (proveedorActivo == false)
            throw new ExcepcionConflicto($"El proveedor {proveedorId} está inactivo y no puede ofrecer repuestos.");

        // Regla de negocio: un proveedor se asocia una sola vez a cada repuesto
        if (await _repositorio.ObtenerAsociacionAsync(repuestoId, proveedorId) is not null)
            throw new ExcepcionConflicto($"El proveedor {proveedorId} ya está asociado al repuesto {repuestoId}; actualice sus condiciones con PUT.");

        await _repositorio.AgregarAsociacionAsync(new RepuestoProveedor
        {
            RepuestoId = repuestoId,
            ProveedorId = proveedorId,
            PrecioOfrecido = condiciones.PrecioOfrecido,
            DiasEntregaEstimados = condiciones.DiasEntregaEstimados
        });

        var repuesto = await ObtenerPorIdentificadorAsync(repuestoId);

        var resultado = Resultado<RepuestoDetalleDto>.Exito(repuesto.Datos!, 201, "Created", "Proveedor asociado.");

        return resultado;
    }

    public async Task<Resultado<RepuestoDetalleDto>> ActualizarCondicionesProveedorAsync(int repuestoId, int proveedorId, CondicionesProveedorRepuestoDto condiciones)
    {
        var asociacion = await _repositorio.ObtenerAsociacionAsync(repuestoId, proveedorId);

        if (asociacion is null)
            throw new ExcepcionNoEncontrado($"El proveedor {proveedorId} no está asociado al repuesto {repuestoId}.");

        asociacion.PrecioOfrecido = condiciones.PrecioOfrecido;
        asociacion.DiasEntregaEstimados = condiciones.DiasEntregaEstimados;

        await _repositorio.GuardarCambiosAsync();

        var repuesto = await ObtenerPorIdentificadorAsync(repuestoId);

        var resultado = Resultado<RepuestoDetalleDto>.Exito(repuesto.Datos!, 200, "Correcto", "Condiciones actualizadas.");

        return resultado;
    }

    private async Task<IEnumerable<RepuestoListaDto>> MapearListaConConteoAsync(IEnumerable<Repuesto> repuestos)
    {
        var datos = _mapeador.Map<List<RepuestoListaDto>>(repuestos);
        var conteos = await _repositorio.ContarProveedoresPorRepuestoAsync(datos.Select(r => r.Identificador));

        foreach (var repuesto in datos)
            repuesto.CantidadProveedores = conteos.GetValueOrDefault(repuesto.Identificador);

        return datos;
    }
}
