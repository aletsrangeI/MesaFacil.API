using AutoMapper;
using Domain.Entities;
using DTO.Catalog;

namespace UseCases.Common.Mapping;

public class MappingsProfile : Profile
{
    public MappingsProfile()
    {
        CreateMap<Catalog, CatalogDTO>().ReverseMap();
    }
}