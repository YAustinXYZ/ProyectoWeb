namespace Proyecto.Domain.DTO;

public class RepuestoListaDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("code")]
    public string? Codigo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("unitOfMeasure")]
    public string? UnidadMedida { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("currentStock")]
    public int StockActual { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("minimumStock")]
    public int StockMinimo { get; set; }

    // Campos calculados en el mapper: no existen como columnas
    // Misma condición que usa la consulta de stock bajo el mínimo: StockActual < StockMinimo
    [System.Text.Json.Serialization.JsonPropertyName("isBelowMinimum")]
    public bool BajoMinimo { get; set; }
    // Unidades que faltan para llegar al mínimo; 0 si el stock ya lo alcanza
    [System.Text.Json.Serialization.JsonPropertyName("shortage")]
    public int Faltante { get; set; }
    // Conteo hecho por SQL Server con GROUP BY (servicio + repositorio)
    [System.Text.Json.Serialization.JsonPropertyName("supplierCount")]
    public int CantidadProveedores { get; set; }
}
