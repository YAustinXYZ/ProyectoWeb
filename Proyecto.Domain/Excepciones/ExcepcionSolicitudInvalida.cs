namespace Proyecto.Domain.Excepciones;

/// <summary>Datos válidos en formato pero que incumplen una regla de negocio: responde 400.</summary>
public sealed class ExcepcionSolicitudInvalida : ExcepcionNegocio
{
    public ExcepcionSolicitudInvalida(string mensaje) : base(mensaje)
    {
    }

    public override int CodigoEstado => 400;

    public override string Titulo => "Solicitud incorrecta";
}
