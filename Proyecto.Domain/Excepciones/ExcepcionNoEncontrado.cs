namespace Proyecto.Domain.Excepciones;

/// <summary>El recurso pedido o uno de sus relacionados no existe: responde 404.</summary>
public sealed class ExcepcionNoEncontrado : ExcepcionNegocio
{
    public ExcepcionNoEncontrado(string mensaje) : base(mensaje)
    {
    }

    public override int CodigoEstado => 404;

    public override string Titulo => "No encontrado";
}
