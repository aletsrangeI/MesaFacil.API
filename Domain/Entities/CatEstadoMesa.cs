namespace Domain.Entities;

public class CatEstadoMesa : BaseAuditableEntity, ICatalogEntity
{
    public string Descripcion { get; set; } = string.Empty;
}

public static class EstadosMesaConst
{
    public const int Disponible = 1;
    public const int Ocupada = 2;
    public const int Reservada = 3;
    public const int PidiendoCuenta = 4;
    public const int Sucia = 5;
    public const int FueraDeServicio = 6;
}