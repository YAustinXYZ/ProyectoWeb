namespace Proyecto.Domain.DTO;

public class TecnicoDetalleDto
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public int Identificador { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("fullName")]
    public string? NombreCompleto { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("identificationNumber")]
    public string? NumeroIdentificacion { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("email")]
    public string? Correo { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("phone")]
    public string? Telefono { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("isAvailable")]
    public bool EstaDisponible { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("hourlyRate")]
    public decimal TarifaPorHora { get; set; }
    // Calculado: conteo hecho por SQL Server; coincide con la cantidad de elementos de specialties
    [System.Text.Json.Serialization.JsonPropertyName("specialtyCount")]
    public int CantidadEspecialidades { get; set; }
    [System.Text.Json.Serialization.JsonPropertyName("specialties")]
    public List<TecnicoEspecialidadDto> Especialidades { get; set; } = [];
}
