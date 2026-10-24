using Mapster;
using Proyecto.Domain.DTO;
using Proyecto.Domain.Entities;

namespace Proyecto.Application.Mapper;

public static class ConfiguracionMapster
{
    // Ruta pública donde se sirven las imágenes de los activos
    public const string UrlImagenesActivos = "/images/assets/";

    // Imagen que se muestra cuando el activo no tiene una propia
    public const string ImagenPredeterminadaActivo = "default.png";

    // Meses completos entre una fecha y hoy; nunca negativo si la fecha es futura
    public static int MesesTranscurridos(DateTime desde)
    {
        var hoy = DateTime.UtcNow.Date;
        var meses = (hoy.Year - desde.Year) * 12 + hoy.Month - desde.Month;

        if (hoy.Day < desde.Day)
            meses--;                                    // El mes en curso todavía no se completa

        return Math.Max(meses, 0);
    }

    // Días calendario entre una fecha y hoy; nunca negativo
    public static int DiasTranscurridos(DateTime desde) =>
        Math.Max((DateTime.UtcNow.Date - desde.Date).Days, 0);

    public static void RegistrarMapeos()
    {
        // CategoriaActivo
        // Las propiedades de entidad a DTO de salida coinciden por nombre y no requieren configuración.
        // En los DTOs de entrada se ignora Identificador para que no se asigne desde el cliente.
        TypeAdapterConfig<CategoriaActivoCrearDto, CategoriaActivo>
            .NewConfig()
            .Ignore(dest => dest.Identificador);              // Generado por la base de datos (IDENTITY)

        TypeAdapterConfig<CategoriaActivoActualizarDto, CategoriaActivo>
            .NewConfig()
            .Ignore(dest => dest.Identificador);              // Se toma del parámetro de la ruta

        // Activo
        // Los nombres de las relaciones requieren Include en el repositorio.
        TypeAdapterConfig<Activo, ActivoListaDto>
            .NewConfig()
            .Map(dest => dest.NombreCategoria, src => src.Categoria.Nombre)
            .Map(dest => dest.NombreUbicacion, src => src.Ubicacion.Nombre)
            .Map(dest => dest.UrlImagen, src => UrlImagenesActivos + (string.IsNullOrEmpty(src.Imagen) ? ImagenPredeterminadaActivo : src.Imagen))
            .Map(dest => dest.MesesAntiguedad, src => MesesTranscurridos(src.FechaAdquisicion))
            .Ignore(dest => dest.CantidadOrdenes);           // Lo asigna el servicio con un conteo en SQL

        TypeAdapterConfig<OrdenMantenimiento, OrdenResumenActivoDto>
            .NewConfig()
            .Map(dest => dest.NombreTipoMantenimiento, src => src.TipoMantenimiento == null ? null : src.TipoMantenimiento.Nombre);

        TypeAdapterConfig<Activo, ActivoDetalleDto>
            .NewConfig()
            .Map(dest => dest.NombreCategoria, src => src.Categoria.Nombre)
            .Map(dest => dest.Categoria, src => src.Categoria)
            .Map(dest => dest.NombreUbicacion, src => src.Ubicacion.Nombre)
            .Map(dest => dest.NombreProveedor, src => src.Proveedor == null ? null : src.Proveedor.Nombre)
            .Map(dest => dest.UrlImagen, src => UrlImagenesActivos + (string.IsNullOrEmpty(src.Imagen) ? ImagenPredeterminadaActivo : src.Imagen))
            .Map(dest => dest.MesesAntiguedad, src => MesesTranscurridos(src.FechaAdquisicion))
            .Map(dest => dest.HorasDesdeUltimoMantenimiento, src => src.HorasUso - src.HorasUsoAlUltimoMantenimiento)
            // Mantenimiento preventivo por uso real: se alcanza el umbral de horas de la categoría
            .Map(dest => dest.RequierePreventivo, src => src.Categoria != null
                                                        && src.Categoria.UmbralPreventivoHoras > 0
                                                        && src.HorasUso - src.HorasUsoAlUltimoMantenimiento >= src.Categoria.UmbralPreventivoHoras)
            .Map(dest => dest.OrdenesMantenimiento, src => src.OrdenesMantenimiento)
            .Ignore(dest => dest.CantidadOrdenes);           // Lo asigna el servicio con un conteo en SQL

        // En los DTOs de entrada se ignoran los campos que asigna el servidor
        TypeAdapterConfig<ActivoCrearDto, Activo>
            .NewConfig()
            .Ignore(            dest => dest.Identificador,
            dest => dest.CodigoActivo,
            dest => dest.Estado,
            dest => dest.HorasUsoAlUltimoMantenimiento,
            dest => dest.CreadoEn,
            dest => dest.Categoria,
            dest => dest.Ubicacion,
            dest => dest.Proveedor!,
            dest => dest.OrdenesMantenimiento);

        TypeAdapterConfig<ActivoActualizarDto, Activo>
            .NewConfig()
            .Ignore(            dest => dest.Identificador,
            dest => dest.CodigoActivo,
            dest => dest.Estado,
            dest => dest.Imagen!,
            dest => dest.HorasUsoAlUltimoMantenimiento,
            dest => dest.CreadoEn,
            dest => dest.Categoria,
            dest => dest.Ubicacion,
            dest => dest.Proveedor!,
            dest => dest.OrdenesMantenimiento);

        // Proveedor
        // Las propiedades de entidad a DTO de salida coinciden por nombre y no requieren configuración.
        TypeAdapterConfig<ProveedorCrearDto, Proveedor>
            .NewConfig()
            .Ignore(dest => dest.Identificador,                // Generado por la base de datos (IDENTITY)
                    dest => dest.EstaActivo,          // Lo asigna el servicio
                    dest => dest.Activos,
                    dest => dest.OrdenesMantenimiento,
                    dest => dest.RepuestosProveedor);

        TypeAdapterConfig<ProveedorActualizarDto, Proveedor>
            .NewConfig()
            .Ignore(dest => dest.Identificador,                // Se toma del parámetro de la ruta
                    dest => dest.Activos,
                    dest => dest.OrdenesMantenimiento,
                    dest => dest.RepuestosProveedor);

        // OrdenMantenimiento
        // Los datos del activo, el tipo y el proveedor requieren Include en el repositorio.
        TypeAdapterConfig<OrdenMantenimiento, OrdenMantenimientoListaDto>
            .NewConfig()
            .Map(dest => dest.CodigoActivo, src => src.Activo.CodigoActivo)
            .Map(dest => dest.NombreActivo, src => src.Activo.Nombre)
            .Map(dest => dest.NombreTipoMantenimiento, src => src.TipoMantenimiento.Nombre)
            .Map(dest => dest.NombreProveedor, src => src.Proveedor == null ? null : src.Proveedor.Nombre)
            .Map(dest => dest.DiasDesdeSolicitud, src => DiasTranscurridos(src.SolicitadoEn));

        TypeAdapterConfig<Activo, ActivoResumenOrdenDto>
            .NewConfig()
            .Map(dest => dest.NombreCategoria, src => src.Categoria == null ? null : src.Categoria.Nombre);

        TypeAdapterConfig<OrdenMantenimiento, OrdenMantenimientoDetalleDto>
            .NewConfig()
            .Map(dest => dest.CodigoActivo, src => src.Activo.CodigoActivo)
            .Map(dest => dest.NombreActivo, src => src.Activo.Nombre)
            .Map(dest => dest.Activo, src => src.Activo)
            .Map(dest => dest.NombreTipoMantenimiento, src => src.TipoMantenimiento.Nombre)
            .Map(dest => dest.EsEmergencia, src => src.TipoMantenimiento.EsEmergencia)
            .Map(dest => dest.NombreProveedor, src => src.Proveedor == null ? null : src.Proveedor.Nombre)
            .Map(dest => dest.DiasDesdeSolicitud, src => DiasTranscurridos(src.SolicitadoEn));

        // En el DTO de entrada se ignoran los campos que asigna el servidor o que se definen en etapas posteriores
        TypeAdapterConfig<OrdenMantenimientoCrearDto, OrdenMantenimiento>
            .NewConfig()
            .Ignore(dest => dest.Identificador,
                    dest => dest.CodigoOrdenTrabajo,
                    dest => dest.Estado,
                    dest => dest.SolicitadoEn,
                    dest => dest.ProgramadoPara!,
                    dest => dest.IniciadoEn!,
                    dest => dest.CerradoEn!,
                    dest => dest.CostoTotal!,
                    dest => dest.Hallazgos!,
                    dest => dest.CreadoEn,
                    dest => dest.Activo,
                    dest => dest.TipoMantenimiento,
                    dest => dest.Proveedor!);

        // Proveedor: repuestos que suministra (requiere Include en el repositorio)
        TypeAdapterConfig<RepuestoProveedor, ProveedorRepuestoDto>
            .NewConfig()
            .Map(dest => dest.Identificador, src => src.RepuestoId)
            .Map(dest => dest.Codigo, src => src.Repuesto.Codigo)
            .Map(dest => dest.Nombre, src => src.Repuesto.Nombre);

        TypeAdapterConfig<Proveedor, ProveedorDetalleDto>
            .NewConfig()
            .Map(dest => dest.Repuestos, src => src.RepuestosProveedor.OrderBy(enlace => enlace.Repuesto.Codigo));

        // Catálogos
        TypeAdapterConfig<TipoMantenimiento, TipoMantenimientoListaDto>
            .NewConfig()
            .Map(dest => dest.NombreEspecialidadRequerida, src => src.EspecialidadRequerida == null ? null : src.EspecialidadRequerida.Nombre);

        // Técnico
        // Los conteos (CantidadEspecialidades) los asigna el servicio con un GROUP BY en SQL
        TypeAdapterConfig<TecnicoEspecialidad, TecnicoEspecialidadDto>
            .NewConfig()
            .Map(dest => dest.Identificador, src => src.EspecialidadId)
            .Map(dest => dest.Nombre, src => src.Especialidad.Nombre)
            .Map(dest => dest.CertificadoDesde, src => DateOnly.FromDateTime(src.CertificadoDesde))
            .Map(dest => dest.MesesAntiguedad, src => MesesTranscurridos(src.CertificadoDesde));

        TypeAdapterConfig<Tecnico, TecnicoListaDto>
            .NewConfig()
            .Ignore(dest => dest.CantidadEspecialidades);

        TypeAdapterConfig<Tecnico, TecnicoDetalleDto>
            .NewConfig()
            .Map(dest => dest.Especialidades, src => src.EspecialidadesTecnico.OrderBy(enlace => enlace.Especialidad.Nombre))
            .Ignore(dest => dest.CantidadEspecialidades);

        TypeAdapterConfig<TecnicoCrearDto, Tecnico>
            .NewConfig()
            .Ignore(dest => dest.Identificador, dest => dest.EspecialidadesTecnico);

        TypeAdapterConfig<TecnicoActualizarDto, Tecnico>
            .NewConfig()
            .Ignore(dest => dest.Identificador, dest => dest.EspecialidadesTecnico);

        // Especialidad
        TypeAdapterConfig<TecnicoEspecialidad, EspecialidadTecnicoDto>
            .NewConfig()
            .Map(dest => dest.Identificador, src => src.TecnicoId)
            .Map(dest => dest.NombreCompleto, src => src.Tecnico.NombreCompleto)
            .Map(dest => dest.EstaDisponible, src => src.Tecnico.EstaDisponible)
            .Map(dest => dest.CertificadoDesde, src => DateOnly.FromDateTime(src.CertificadoDesde));

        TypeAdapterConfig<Especialidad, EspecialidadListaDto>
            .NewConfig()
            .Ignore(dest => dest.CantidadTecnicos);

        TypeAdapterConfig<Especialidad, EspecialidadDetalleDto>
            .NewConfig()
            .Map(dest => dest.Tecnicos, src => src.EspecialidadesTecnico.OrderBy(enlace => enlace.Tecnico.NombreCompleto))
            .Ignore(dest => dest.CantidadTecnicos);

        TypeAdapterConfig<EspecialidadCrearDto, Especialidad>
            .NewConfig()
            .Ignore(dest => dest.Identificador, dest => dest.EspecialidadesTecnico, dest => dest.TiposMantenimiento);

        TypeAdapterConfig<EspecialidadActualizarDto, Especialidad>
            .NewConfig()
            .Ignore(dest => dest.Identificador, dest => dest.EspecialidadesTecnico, dest => dest.TiposMantenimiento);

        // Repuesto
        // BajoMinimo usa la misma condición que la consulta de stock bajo el mínimo del repositorio
        TypeAdapterConfig<Repuesto, RepuestoListaDto>
            .NewConfig()
            .Map(dest => dest.BajoMinimo, src => src.StockActual < src.StockMinimo)
            .Map(dest => dest.Faltante, src => Math.Max(src.StockMinimo - src.StockActual, 0))
            .Ignore(dest => dest.CantidadProveedores);

        TypeAdapterConfig<RepuestoProveedor, RepuestoProveedorDto>
            .NewConfig()
            .Map(dest => dest.Identificador, src => src.ProveedorId)
            .Map(dest => dest.Nombre, src => src.Proveedor.Nombre);

        TypeAdapterConfig<Repuesto, RepuestoDetalleDto>
            .NewConfig()
            .Map(dest => dest.BajoMinimo, src => src.StockActual < src.StockMinimo)
            .Map(dest => dest.Faltante, src => Math.Max(src.StockMinimo - src.StockActual, 0))
            .Map(dest => dest.Proveedores, src => src.ProveedoresRepuesto.OrderBy(enlace => enlace.PrecioOfrecido))
            .Ignore(dest => dest.CantidadProveedores);

        TypeAdapterConfig<RepuestoCrearDto, Repuesto>
            .NewConfig()
            .Ignore(dest => dest.Identificador, dest => dest.CreadoEn, dest => dest.ProveedoresRepuesto);

        TypeAdapterConfig<RepuestoActualizarDto, Repuesto>
            .NewConfig()
            .Ignore(dest => dest.Identificador, dest => dest.CreadoEn, dest => dest.ProveedoresRepuesto);
    }
}
