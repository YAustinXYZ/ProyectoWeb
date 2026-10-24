using MapsterMapper;
using Proyecto.Application.Interfaces;
using Proyecto.Domain.DTO;
using Proyecto.Domain.DTO.ResultadoPatron;
using Proyecto.Domain.Entities;
using Proyecto.Domain.Excepciones;
using Proyecto.Infraestructure.Interfaces;

namespace Proyecto.Application.Implementation;

public class TecnicoService : ITecnicoService
{
    private readonly ITecnicoRepository _repositorio;
    private readonly IMapper _mapeador;

    public TecnicoService(ITecnicoRepository repositorio, IMapper mapeador)
    {
        _repositorio = repositorio;
        _mapeador = mapeador;
    }

    // CONSULTA

    public async Task<Resultado<IEnumerable<TecnicoListaDto>>> ObtenerTodosAsync(int? especialidadId = null, bool? estaDisponible = null)
    {
        var tecnicos = await _repositorio.ObtenerTodosAsync(especialidadId, estaDisponible);

        var datos = _mapeador.Map<List<TecnicoListaDto>>(tecnicos);
        var conteos = await _repositorio.ContarEspecialidadesPorTecnicoAsync(datos.Select(t => t.Identificador));

        foreach (var tecnico in datos)
            tecnico.CantidadEspecialidades = conteos.GetValueOrDefault(tecnico.Identificador);

        var resultado = Resultado<IEnumerable<TecnicoListaDto>>.Exito(datos);

        return resultado;
    }

    public async Task<Resultado<TecnicoDetalleDto>> ObtenerPorIdentificadorAsync(int id)
    {
        var tecnico = await _repositorio.ObtenerPorIdentificadorAsync(id);

        if (tecnico is null)
            throw new ExcepcionNoEncontrado($"El técnico {id} no existe.");

        var datos = _mapeador.Map<TecnicoDetalleDto>(tecnico);
        var conteos = await _repositorio.ContarEspecialidadesPorTecnicoAsync([id]);
        datos.CantidadEspecialidades = conteos.GetValueOrDefault(id);      // Coincide con datos.Especialidades.Count

        var resultado = Resultado<TecnicoDetalleDto>.Exito(datos);

        return resultado;
    }

    // CREACIÓN

    public async Task<Resultado<TecnicoDetalleDto>> AgregarAsync(TecnicoCrearDto tecnico)
    {
        // Regla de negocio: la identificación no se puede repetir
        if (await _repositorio.ExistePorIdentificacionAsync(tecnico.NumeroIdentificacion!))
            throw new ExcepcionConflicto($"Ya existe un técnico con la identificación {tecnico.NumeroIdentificacion}.");

        var entidad = _mapeador.Map<Tecnico>(tecnico);

        await _repositorio.AgregarAsync(entidad);        // SQL Server asigna el Identificador

        var creado = await ObtenerPorIdentificadorAsync(entidad.Identificador);

        var resultado = Resultado<TecnicoDetalleDto>.Exito(creado.Datos!, 201, "Created", "Registro guardado.");

        return resultado;
    }

    // ACTUALIZACIÓN

    public async Task<Resultado<TecnicoDetalleDto>> ActualizarAsync(int id, TecnicoActualizarDto tecnico)
    {
        var entidad = await _repositorio.ObtenerParaActualizarAsync(id);

        if (entidad is null)
            throw new ExcepcionNoEncontrado($"El técnico {id} no existe.");

        if (await _repositorio.ExistePorIdentificacionAsync(tecnico.NumeroIdentificacion!, id))
            throw new ExcepcionConflicto($"Ya existe un técnico con la identificación {tecnico.NumeroIdentificacion}.");

        _mapeador.Map(tecnico, entidad);

        await _repositorio.GuardarCambiosAsync();

        var actualizado = await ObtenerPorIdentificadorAsync(id);

        var resultado = Resultado<TecnicoDetalleDto>.Exito(actualizado.Datos!, 200, "Correcto", "Registro actualizado.");

        return resultado;
    }

    // RELACIÓN N:N TÉCNICO–ESPECIALIDAD

    public async Task<Resultado<TecnicoDetalleDto>> AsociarEspecialidadAsync(int tecnicoId, int especialidadId, SolicitudEspecialidadTecnicoDto solicitud)
    {
        if (!await _repositorio.ExisteAsync(tecnicoId))
            throw new ExcepcionNoEncontrado($"El técnico {tecnicoId} no existe.");

        if (!await _repositorio.ExisteEspecialidadAsync(especialidadId))
            throw new ExcepcionNoEncontrado($"La especialidad {especialidadId} no existe.");

        // Regla de negocio: la misma especialidad no se asocia dos veces al mismo técnico
        if (await _repositorio.ObtenerAsignacionAsync(tecnicoId, especialidadId) is not null)
            throw new ExcepcionConflicto($"El técnico {tecnicoId} ya tiene asociada la especialidad {especialidadId}.");

        await _repositorio.AgregarAsignacionAsync(new TecnicoEspecialidad
        {
            TecnicoId = tecnicoId,
            EspecialidadId = especialidadId,
            NivelCertificacion = solicitud.NivelCertificacion!,
            CertificadoDesde = solicitud.CertificadoDesde!.Value.ToDateTime(TimeOnly.MinValue)
        });

        var tecnico = await ObtenerPorIdentificadorAsync(tecnicoId);

        var resultado = Resultado<TecnicoDetalleDto>.Exito(tecnico.Datos!, 201, "Created", "Especialidad asociada.");

        return resultado;
    }

    public async Task<Resultado<TecnicoDetalleDto>> DesasociarEspecialidadAsync(int tecnicoId, int especialidadId)
    {
        if (!await _repositorio.ExisteAsync(tecnicoId))
            throw new ExcepcionNoEncontrado($"El técnico {tecnicoId} no existe.");

        var asignacion = await _repositorio.ObtenerAsignacionAsync(tecnicoId, especialidadId);

        if (asignacion is null)
            throw new ExcepcionNoEncontrado($"El técnico {tecnicoId} no tiene asociada la especialidad {especialidadId}.");

        await _repositorio.EliminarAsignacionAsync(asignacion);

        var tecnico = await ObtenerPorIdentificadorAsync(tecnicoId);

        var resultado = Resultado<TecnicoDetalleDto>.Exito(tecnico.Datos!, 200, "Correcto", "Especialidad desasociada.");

        return resultado;
    }
}
