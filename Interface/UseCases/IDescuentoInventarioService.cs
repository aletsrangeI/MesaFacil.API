namespace Interface.UseCases;

public interface IDescuentoInventarioService
{
    Task<bool> DescontarPorCuentaPagadaAsync(int idCuenta);
    Task<bool> DescontarPorPedidoAsync(Guid idPedido);
}
