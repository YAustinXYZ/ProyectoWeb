namespace Proyecto.Domain.Entities;

public class TechnicianSpecialty
{
    public int TechnicianId { get; set; }
    public Technician Technician { get; set; } = null!;
    public int SpecialtyId { get; set; }
    public Specialty Specialty { get; set; } = null!;
    public string CertificationLevel { get; set; } = string.Empty;
    public DateTime CertifiedSince { get; set; }
}
