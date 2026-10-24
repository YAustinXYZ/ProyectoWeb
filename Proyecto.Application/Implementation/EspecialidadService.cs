using MapsterMapper;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;
using Proyecto.Domain.Entities;
using Proyecto.Domain.Excepciones;
using Proyecto.Infraestructure.Interfaces;

namespace Proyecto.Application.Implementation;

public class EspecialidadService : IEspecialidadService
{
    private readonly IEspecialidadRepository _repositorio;
    private readonly IMapper _mapeador;

    public EspecialidadService(IEspecialidadRepository repositorio, IMapper mapeador)
    {
        _repositorio = repositorio;
        _mapeador = mapeador;
    }

    public async Task<Resultado<IEnumerable<EspecialidadListaDto>>> ObtenerTodosAsync()
    {
        var especialidades = await _repositorio.ObtenerTodosAsync();

        var datos = _mapeador.Map<List<EspecialidadListaDto>>(especialidades);
        var conteos = await _repositorio.ContarTecnicosPorEspecialidadAsync(datos.Select(e => e.Identificador));

        foreach (var especialidad in datos)
            especialidad.CantidadTecnicos = conteos.GetValueOrDefault(especialidad.Identificador);

        var resultado = Resultado<IEnumerable<EspecialidadListaDto>>.Exito(datos);

        return resultado;
    }

    public async Task<Resultado<EspecialidadDetalleDto>> ObtenerPorIdentificadorAsync(int id)
    {
        var especialidad = await _repositorio.ObtenerPorIdentificadorAsync(id);

        if (especialidad is null)
            throw new ExcepcionNoEncontrado($"La especialidad {id} no existe.");

        var datos = _mapeador.Map<EspecialidadDetalleDto>(especialidad);
        var conteos = await _repositorio.ContarTecnicosPorEspecialidadAsync([id]);
        datos.CantidadTecnicos = conteos.GetValueOrDefault(id);

        var resultado = Resultado<EspecialidadDetalleDto>.Exito(datos);

        return resultado;
    }

    public async Task<Resultado<EspecialidadDetalleDto>> AgregarAsync(EspecialidadCrearDto especialidad)
    {
        // Regla de negocio: el nombre no se puede repetir (también lo garantiza el índice único)
        if (await _repositorio.ExistePorNombreAsync(especialidad.Nombre!))
            throw new ExcepcionConflicto($"Ya existe una especialidad con el nombre {especialidad.Nombre}.");

        var entidad = _mapeador.Map<Especialidad>(especialidad);

        await _repositorio.AgregarAsync(entidad);

        var creada = await ObtenerPorIdentificadorAsync(entidad.Identificador);

        var resultado = Resultado<EspecialidadDetalleDto>.Exito(creada.Datos!, 201, "Created", "Registro guardado.");

        return resultado;
    }

    public async Task<Resultado<EspecialidadDetalleDto>> ActualizarAsync(int id, EspecialidadActualizarDto especialidad)
    {
        var entidad = await _repositorio.ObtenerParaActualizarAsync(id);

        if (entidad is null)
            throw new ExcepcionNoEncontrado($"La especialidad {id} no existe.");

        if (await _repositorio.ExistePorNombreAsync(especialidad.Nombre!, id))
            throw new ExcepcionConflicto($"Ya existe una especialidad con el nombre {especialidad.Nombre}.");

        _mapeador.Map(especialidad, entidad);

        await _repositorio.GuardarCambiosAsync();

        var actualizada = await ObtenerPorIdentificadorAsync(id);

        var resultado = Resultado<EspecialidadDetalleDto>.Exito(actualizada.Datos!, 200, "Correcto", "Registro actualizado.");

        return resultado;
    }
}
