using AutoMapper;
using Domain.Entities;
using DTO.Catalog;
using DTO.CatalogItem;

namespace UseCases.Common.Mapping;

public class MappingsProfile : Profile
{
    public MappingsProfile()
    {
        CreateMap<Domain.Entities.CatalogItem, DTO.CatalogItem.CatalogItemDTO>().ReverseMap();
        CreateMap<Catalog, CatalogDTO>().ReverseMap();
    }
}
