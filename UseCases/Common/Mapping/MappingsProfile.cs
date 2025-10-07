using AutoMapper;
using Domain.Entities;
using DTO.Catalog;
using DTO.CatalogItem;
using DTO.Area;
using DTO.CategoriaMenu;
using DTO.Cliente;
using DTO.CorteCaja;

namespace UseCases.Common.Mapping;

public class MappingsProfile : Profile
{
    public MappingsProfile()
    {
        CreateMap<Domain.Entities.CorteCaja, DTO.CorteCaja.CorteCajaDTO>().ReverseMap();
        CreateMap<Cliente, ClienteDTO>().ReverseMap();
        CreateMap<CategoriaMenu, CategoriaMenuDTO>().ReverseMap();
        CreateMap<Area, AreaDTO>().ReverseMap();
        CreateMap<CatalogItem, CatalogItemDTO>().ReverseMap();
        CreateMap<Catalog, CatalogDTO>().ReverseMap();
    }
}
