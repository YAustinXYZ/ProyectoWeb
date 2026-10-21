namespace Proyecto.Domain.Entities;

public class Technician
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Identification { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsAvailable { get; set; } = true;
    public decimal HourlyRate { get; set; }
    public ICollection<TechnicianSpecialty> Specialties { get; set; } = new List<TechnicianSpecialty>();
}
