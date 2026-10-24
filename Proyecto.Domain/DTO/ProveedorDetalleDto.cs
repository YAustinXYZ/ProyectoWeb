namespace Proyecto.Domain.DTO;

public class ProveedorDetalleDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("name")]
    public string? Nombre { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("taxId")]
    public string? IdentificacionFiscal { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("contactName")]
    public string? NombreContacto { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("phone")]
    public string? Telefono { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("email")]
    public string? Correo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("address")]
    public string? Direccion { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("generalConditions")]
    public string? CondicionesGenerales { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("isActive")]
    public bool EstaActivo { get; set; }
    // Repuestos que suministra, con el precio y el tiempo de entrega de cada uno
    [System.Text.Json.Serialization.JsonPropertyName("spareParts")]
    public List<ProveedorRepuestoDto> Repuestos { get; set; } = [];
}
