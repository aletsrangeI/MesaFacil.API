using AutoMapper;
using Domain.Entities;
using DTO.Catalog;
using DTO.CatalogItem;
using DTO.Area;
using DTO.CategoriaMenu;

namespace UseCases.Common.Mapping;

public class MappingsProfile : Profile
{
    public MappingsProfile()
    {
        CreateMap<CategoriaMenu, CategoriaMenuDTO>().ReverseMap();
        CreateMap<Area, AreaDTO>().ReverseMap();
        CreateMap<CatalogItem, CatalogItemDTO>().ReverseMap();
        CreateMap<Catalog, CatalogDTO>().ReverseMap();
    }
}