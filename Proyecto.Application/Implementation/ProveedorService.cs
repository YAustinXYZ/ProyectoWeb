using MapsterMapper;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;
using Proyecto.Domain.Entities;
using Proyecto.Domain.Excepciones;
using Proyecto.Infraestructure.Interfaces;

namespace Proyecto.Application.Implementation;

public class ProveedorService : IProveedorService
{
    // Límite de registros por página
    public const int TamanoMaximoPagina = 50;

    private readonly IProveedorRepository _repositorio;
    private readonly IMapper _mapeador;

    public ProveedorService(IProveedorRepository repositorio, IMapper mapeador)
    {
        _repositorio = repositorio;
        _mapeador = mapeador;
    }

    // CONSULTA

    public async Task<Resultado<IEnumerable<ProveedorListaDto>>> ObtenerTodosAsync()
    {
        var suppliers = await _repositorio.ObtenerTodosAsync();

        var datos = _mapeador.Map<IEnumerable<ProveedorListaDto>>(suppliers);

        var resultado = Resultado<IEnumerable<ProveedorListaDto>>.Exito(datos);

        return resultado;
    }

    public async Task<Resultado<IEnumerable<ProveedorListaDto>>> ObtenerPaginacionAsync(int paginaNumero, int tamanoPagina)
    {
        if (paginaNumero < 1 || tamanoPagina < 1 || tamanoPagina > TamanoMaximoPagina)
            throw new ExcepcionSolicitudInvalida($"El número de página debe ser 1 o mayor y el tamaño debe estar entre 1 y {TamanoMaximoPagina}.");

        var suppliers = await _repositorio.ObtenerPaginacionAsync(paginaNumero, tamanoPagina);
        var registrosTotales = await _repositorio.ContarAsync();

        var datos = _mapeador.Map<IEnumerable<ProveedorListaDto>>(suppliers);

        var resultado = Resultado<IEnumerable<ProveedorListaDto>>.Paginado(datos, paginaNumero, tamanoPagina, registrosTotales);

        return resultado;
    }

    public async Task<Resultado<ProveedorDetalleDto>> ObtenerPorIdentificadorAsync(int id)
    {
        var proveedor = await _repositorio.ObtenerPorIdentificadorAsync(id);

        if (proveedor is null)
            throw new ExcepcionNoEncontrado($"El proveedor {id} no existe.");

        var datos = _mapeador.Map<ProveedorDetalleDto>(proveedor);

        var resultado = Resultado<ProveedorDetalleDto>.Exito(datos);

        return resultado;
    }

    // CREACIÓN
    public async Task<Resultado<ProveedorDetalleDto>> AgregarAsync(ProveedorCrearDto proveedor)
    {
        // Regla de negocio: la cédula jurídica no se puede repetir
        if (await _repositorio.ExistePorIdentificacionFiscalAsync(proveedor.IdentificacionFiscal!))
            throw new ExcepcionConflicto($"Ya existe un proveedor con la cédula jurídica {proveedor.IdentificacionFiscal}.");

        var entidad = _mapeador.Map<Proveedor>(proveedor);
        entidad.EstaActivo = true;                    // Todo proveedor nuevo se crea activo

        await _repositorio.AgregarAsync(entidad);        // SQL Server asigna el Identificador

        var datos = _mapeador.Map<ProveedorDetalleDto>(entidad);

        var resultado = Resultado<ProveedorDetalleDto>.Exito(datos, 201, "Created", "Registro guardado.");

        return resultado;
    }

    // ACTUALIZACIÓN

    public async Task<Resultado<ProveedorDetalleDto>> ActualizarAsync(int id, ProveedorActualizarDto proveedor)
    {
        if (!await _repositorio.ExisteAsync(id))
            throw new ExcepcionNoEncontrado($"El proveedor {id} no existe.");

        // Misma regla, excluyendo al proveedor que se edita
        if (await _repositorio.ExistePorIdentificacionFiscalAsync(proveedor.IdentificacionFiscal!, id))
            throw new ExcepcionConflicto($"Ya existe un proveedor con la cédula jurídica {proveedor.IdentificacionFiscal}.");

        var entidad = _mapeador.Map<Proveedor>(proveedor);
        entidad.Identificador = id;                            // El Identificador se toma de la ruta, no del body

        await _repositorio.ActualizarAsync(entidad);

        // Se vuelve a consultar para devolver también los repuestos que suministra
        var actualizado = await _repositorio.ObtenerPorIdentificadorAsync(id);

        var datos = _mapeador.Map<ProveedorDetalleDto>(actualizado!);

        var resultado = Resultado<ProveedorDetalleDto>.Exito(datos, 200, "Correcto", "Registro actualizado.");

        return resultado;
    }

    // ELIMINAR

    public async Task<Resultado<int>> EliminarAsync(int id)
    {
        if (!await _repositorio.ExisteAsync(id))
            throw new ExcepcionNoEncontrado($"El proveedor {id} no existe.");

        // Regla de negocio: no se elimina un proveedor con activos u órdenes de mantenimiento asociados
        if (await _repositorio.TieneRegistrosRelacionadosAsync(id))
            throw new ExcepcionConflicto($"El proveedor {id} tiene activos, órdenes de mantenimiento o repuestos asociados y no se puede eliminar.");

        await _repositorio.EliminarAsync(id);

        var resultado = Resultado<int>.Exito(id, 200, "Correcto", "Registro eliminado.");

        return resultado;
    }
}
