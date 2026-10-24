using Proyecto.Domain.Entities;

namespace Proyecto.Infraestructure.Interfaces;

// Catálogos de solo lectura que alimentan los formularios: ubicaciones y tipos de mantenimiento
public interface ICatalogoRepository
{
    Task<IEnumerable<Ubicacion>> ObtenerUbicacionesActivasAsync();
    Task<IEnumerable<TipoMantenimiento>> ObtenerTiposMantenimientoAsync();
}
