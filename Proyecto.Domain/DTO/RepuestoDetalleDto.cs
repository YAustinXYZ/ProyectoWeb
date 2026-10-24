namespace Proyecto.Domain.DTO;

public class RepuestoDetalleDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("code")]
    public string? Codigo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("description")]
    public string? Descripcion { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("unitOfMeasure")]
    public string? UnidadMedida { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("currentStock")]
    public int StockActual { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("minimumStock")]
    public int StockMinimo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("createdAt")]
    public DateTime CreadoEn { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("isBelowMinimum")]
    public bool BajoMinimo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("shortage")]
    public int Faltante { get; set; }
    // Coincide con la cantidad de elementos de suppliers
    [System.Text.Json.Serialization.JsonPropertyName("supplierCount")]
    public int CantidadProveedores { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("suppliers")]
    public List<RepuestoProveedorDto> Proveedores { get; set; } = [];
}
