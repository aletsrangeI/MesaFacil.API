using AutoMapper;
using Domain.Entities;
using DTO.Catalog;
using DTO.CatalogItem;
using DTO.Area;
using DTO.CategoriaMenu;
using DTO.Cliente;
using DTO.CorteCaja;
using DTO.Credencial;
using DTO.Cuenta;
using DTO.DescuentoAplicado;
using DTO.DetalleCuenta;

namespace UseCases.Common.Mapping;

public class MappingsProfile : Profile
{
    public MappingsProfile()
    {
        CreateMap<Domain.Entities.DetalleCuenta, DTO.DetalleCuenta.DetalleCuentaDTO>().ReverseMap();
        CreateMap<Domain.Entities.DetalleCuenta, DTO.DetalleCuenta.DetalleCuentaDTO>().ReverseMap();
        CreateMap<Domain.Entities.DetalleCuenta, DTO.DetalleCuenta.DetalleCuentaDTO>().ReverseMap();
        CreateMap<Domain.Entities.DetalleCuenta, DTO.DetalleCuenta.DetalleCuentaDTO>().ReverseMap();
        CreateMap<Domain.Entities.DescuentoAplicado, DTO.DescuentoAplicado.DescuentoAplicadoDTO>().ReverseMap();
        CreateMap<Domain.Entities.DescuentoAplicado, DTO.DescuentoAplicado.DescuentoAplicadoDTO>().ReverseMap();
        CreateMap<Domain.Entities.DescuentoAplicado, DTO.DescuentoAplicado.DescuentoAplicadoDTO>().ReverseMap();
        CreateMap<Domain.Entities.DescuentoAplicado, DTO.DescuentoAplicado.DescuentoAplicadoDTO>().ReverseMap();
        CreateMap<Domain.Entities.DescuentoAplicado, DTO.DescuentoAplicado.DescuentoAplicadoDTO>().ReverseMap();
        CreateMap<Domain.Entities.DescuentoAplicado, DTO.DescuentoAplicado.DescuentoAplicadoDTO>().ReverseMap();
        CreateMap<Domain.Entities.DescuentoAplicado, DTO.DescuentoAplicado.DescuentoAplicadoDTO>().ReverseMap();
        CreateMap<Domain.Entities.Cuenta, DTO.Cuenta.CuentaDTO>().ReverseMap();
        CreateMap<Domain.Entities.Credencial, DTO.Credencial.CredencialDTO>().ReverseMap();
        CreateMap<Domain.Entities.CorteCaja, DTO.CorteCaja.CorteCajaDTO>().ReverseMap();
        CreateMap<Cliente, ClienteDTO>().ReverseMap();
        CreateMap<CategoriaMenu, CategoriaMenuDTO>().ReverseMap();
        CreateMap<Area, AreaDTO>().ReverseMap();
        CreateMap<CatalogItem, CatalogItemDTO>().ReverseMap();
        CreateMap<Catalog, CatalogDTO>().ReverseMap();
    }
}
