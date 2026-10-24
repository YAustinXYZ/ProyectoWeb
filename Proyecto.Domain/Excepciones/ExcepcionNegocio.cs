namespace Proyecto.Domain.Excepciones;

/// <summary>
/// Base de las fallas de negocio esperadas. El manejador global la traduce al código HTTP
/// que define cada subclase, así ninguna regla de negocio termina en un 500 genérico.
/// </summary>
public abstract class ExcepcionNegocio : Exception
{
    protected ExcepcionNegocio(string mensaje) : base(mensaje)
    {
    }

    public abstract int CodigoEstado { get; }

    public abstract string Titulo { get; }
}
