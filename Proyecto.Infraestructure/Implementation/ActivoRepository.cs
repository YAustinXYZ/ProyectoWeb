using Microsoft.EntityFrameworkCore;
using Proyecto.Domain.Entities;
using Proyecto.Infraestructure.Data;
using Proyecto.Infraestructure.Interfaces;

namespace Proyecto.Infraestructure.Implementation;

public class ActivoRepository : IActivoRepository
{
    private readonly ContextoAplicacion _contexto;

    public ActivoRepository(ContextoAplicacion contexto)
    {
        _contexto = contexto;
    }

    // Consultas de lectura sin seguimiento de cambios (AsNoTracking)

    // Estado de baja lógica: equivale al Status = false del patrón visto en clase
    private const string EstadoBaja = "Decommissioned";

    // Filtros opcionales: si un parámetro llega null, no se aplica.
    // Sin filtro de estado, el listado oculta los activos dados de baja (como Status == true en clase);
    // con status=Decommissioned se pueden consultar.
    private IQueryable<Activo> Filtrar(int? categoriaId, string? estado)
    {
        var consulta = _contexto.Activos.AsQueryable();

        if (categoriaId is not null)
            consulta = consulta.Where(a => a.CategoriaActivoId == categoriaId);

        consulta = string.IsNullOrWhiteSpace(estado)
            ? consulta.Where(a => a.Estado != EstadoBaja)
            : consulta.Where(a => a.Estado == estado);

        return consulta;
    }

    public async Task<IEnumerable<Activo>> ObtenerTodosAsync(int? categoriaId = null, string? estado = null)
    {
        var resultado = await Filtrar(categoriaId, estado)
            .AsNoTracking()
            .Include(a => a.Categoria)
            .Include(a => a.Ubicacion)
            .OrderBy(a => a.Identificador)
            .ToListAsync();

        return resultado;
    }

    public async Task<IEnumerable<Activo>> ObtenerPaginacionAsync(int paginaNumero, int tamanoPagina, int? categoriaId = null, string? estado = null)
    {
        var resultado = await Filtrar(categoriaId, estado)
            .AsNoTracking()
            .Include(a => a.Categoria)
            .Include(a => a.Ubicacion)
            .OrderBy(a => a.Identificador)                    // Orden necesario para Skip/Take
            .Skip((paginaNumero - 1) * tamanoPagina)
            .Take(tamanoPagina)
            .ToListAsync();

        return resultado;
    }

    // Cantidad total de registros, utilizada en la paginación
    // Cuenta con los mismos filtros del listado para que totalRecords sea coherente
    public async Task<int> ContarAsync(int? categoriaId = null, string? estado = null)
    {
        var resultado = await Filtrar(categoriaId, estado).CountAsync();

        return resultado;
    }

    // Un solo SELECT ... GROUP BY ActivoId: SQL Server cuenta, no se traen las órdenes a memoria.
    // Los activos sin órdenes no aparecen en el diccionario; el servicio los toma como 0.
    public async Task<Dictionary<int, int>> ContarOrdenesPorActivoAsync(IEnumerable<int> idsActivos)
    {
        var ids = idsActivos.ToList();

        var resultado = await _contexto.OrdenesMantenimiento
            .Where(o => ids.Contains(o.ActivoId))
            .GroupBy(o => o.ActivoId)
            .Select(grupo => new { ActivoId = grupo.Key, Cantidad = grupo.Count() })
            .ToDictionaryAsync(x => x.ActivoId, x => x.Cantidad);

        return resultado;
    }

    public async Task<Activo?> ObtenerPorIdentificadorAsync(int id)
    {
        var resultado = await _contexto.Activos
            .AsNoTracking()
            .Include(a => a.Categoria)
            .Include(a => a.Ubicacion)
            .Include(a => a.Proveedor)
            // Historial resumido para el detalle, de la orden más reciente a la más antigua
            .Include(a => a.OrdenesMantenimiento.OrderByDescending(o => o.SolicitadoEn))
                .ThenInclude(o => o.TipoMantenimiento)
            .FirstOrDefaultAsync(a => a.Identificador == id);

        return resultado;
    }

    // Con seguimiento de cambios: SaveChangesAsync actualiza solo las columnas modificadas.
    // Igual que GetForUpdateAsync de clase, excluye los registros dados de baja: para editar,
    // cambiar estado, dar de baja o cambiar la imagen, un activo Decommissioned ya no existe (404).
    public async Task<Activo?> ObtenerParaActualizarAsync(int id)
    {
        var resultado = await _contexto.Activos
            .FirstOrDefaultAsync(a => a.Identificador == id && a.Estado != EstadoBaja);

        return resultado;
    }

    // Verificaciones de existencia

    // Devuelve null si la categoría no existe
    public async Task<string?> ObtenerPrefijoCodigoCategoriaAsync(int categoryId)
    {
        var resultado = await _contexto.CategoriasActivos
            .Where(c => c.Identificador == categoryId)
            .Select(c => c.PrefijoCodigo)
            .FirstOrDefaultAsync();

        return resultado;
    }

    public async Task<bool> ExisteUbicacionAsync(int locationId)
    {
        var resultado = await _contexto.Ubicaciones
            .AnyAsync(l => l.Identificador == locationId);

        return resultado;
    }

    public async Task<bool> ExisteProveedorAsync(int supplierId)
    {
        var resultado = await _contexto.Proveedores
            .AnyAsync(s => s.Identificador == supplierId);

        return resultado;
    }

    // excludeId: Identificador del activo que se excluye de la verificación al editar
    public async Task<bool> ExistePorNumeroSerieAsync(string serialNumber, int? excludeId = null)
    {
        var resultado = await _contexto.Activos
            .AnyAsync(a => a.NumeroSerie == serialNumber
                        && (excludeId == null || a.Identificador != excludeId));

        return resultado;
    }

    public async Task<bool> TieneOrdenesAbiertasAsync(int id, IEnumerable<string> closedStatuses)
    {
        var resultado = await _contexto.OrdenesMantenimiento
            .AnyAsync(o => o.ActivoId == id && !closedStatuses.Contains(o.Estado));

        return resultado;
    }

    // Incluye los activos dados de baja, para que un código no se vuelva a asignar
    public async Task<string?> ObtenerUltimoCodigoActivoAsync(string codePrefix)
    {
        var resultado = await _contexto.Activos
            .Where(a => a.CodigoActivo.StartsWith(codePrefix + "-"))
            .OrderByDescending(a => a.CodigoActivo)
            .Select(a => a.CodigoActivo)
            .FirstOrDefaultAsync();

        return resultado;
    }

    // Operaciones de escritura
    public async Task AgregarAsync(Activo activo)
    {
        // Identificador generado por la base de datos (IDENTITY)
        _contexto.Activos.Add(activo);
        await _contexto.SaveChangesAsync();
    }

    public async Task GuardarCambiosAsync()
    {
        await _contexto.SaveChangesAsync();
    }
}
