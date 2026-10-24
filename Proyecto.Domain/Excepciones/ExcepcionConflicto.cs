namespace Proyecto.Domain.Excepciones;

/// <summary>La operación choca con el estado actual de los datos (duplicados, relaciones, estados): responde 409.</summary>
public sealed class ExcepcionConflicto : ExcepcionNegocio
{
    public ExcepcionConflicto(string mensaje) : base(mensaje)
    {
    }

    public override int CodigoEstado => 409;

    public override string Titulo => "Conflicto";
}
