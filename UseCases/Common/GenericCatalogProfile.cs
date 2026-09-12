using AutoMapper;
using Domain.Entities;
using DTO.GenericCatalog;

namespace UseCases.Common;

/// <summary>
/// Perfil de AutoMapper que cubre todas las entidades Cat* → GenericCatalogDTO
/// y viceversa. Un solo perfil gracias a la interfaz ICatalogEntity.
/// </summary>
public class GenericCatalogProfile : Profile
{
    public GenericCatalogProfile()
    {
        // Mapeo bidireccional para cada entidad Cat*
        CreateCatalogMap<CatCredencial>();
        CreateCatalogMap<CatEstacionesCocina>();
        CreateCatalogMap<CatEstadoCuenta>();
        CreateCatalogMap<CatEstadoItemKDS>();
        CreateCatalogMap<CatEstadoMesa>();
        CreateCatalogMap<CatEstadoPedido>();
        CreateCatalogMap<CatEstadoPedidoDetalle>();
        CreateCatalogMap<CatEstadoTicketCocina>();
        CreateCatalogMap<CatImpuesto>();
        CreateCatalogMap<CatMetodoDePago>();
        CreateCatalogMap<CatMoneda>();
        CreateCatalogMap<CatTipoDescuento>();
        CreateCatalogMap<CatTipoPedido>();
        CreateCatalogMap<CatTipoAlmacen>();
        CreateCatalogMap<CatMotivoMovimientoInventario>();
        CreateCatalogMap<CatConceptoMovimientoCaja>();
        CreateCatalogMap<CatMotivoCancelacionPedido>();
        CreateCatalogMap<CatCanalVenta>();
        CreateCatalogMap<CatRegimenFiscal>();
    }

    private void CreateCatalogMap<TEntity>()
        where TEntity : BaseAuditableEntity, ICatalogEntity, new()
    {
        CreateMap<TEntity, GenericCatalogDTO>().ReverseMap();
    }
}
