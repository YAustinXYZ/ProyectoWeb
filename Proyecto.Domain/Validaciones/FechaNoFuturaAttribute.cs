using System.ComponentModel.DataAnnotations;

namespace Proyecto.Domain.Validaciones;

/// <summary>
/// DataAnnotation propia: la fecha no puede ser posterior a hoy (UTC).
/// Es una validación de forma del dato (no consulta la base de datos), por eso vive junto a
/// las demás DataAnnotations y la responde ModelState con 400, igual que [Required] o [Range].
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class FechaNoFuturaAttribute : ValidationAttribute
{
    public FechaNoFuturaAttribute() : base("La fecha no puede estar en el futuro.")
    {
    }

    public override bool IsValid(object? value) => value switch
    {
        null => true,                                       // La obligatoriedad la decide [Required]
        DateOnly fecha => fecha <= DateOnly.FromDateTime(DateTime.UtcNow),
        DateTime fecha => fecha.Date <= DateTime.UtcNow.Date,
        _ => false
    };
}
