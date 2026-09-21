using Domain.Entities;
using DTO.GenericCatalog;
using FluentAssertions;
using Interface.Mapping;
using UseCases.Common.Mapping;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Compras;

public class CatRegimenFiscalMappingTests
{
    private readonly IAppMapper _mapper;

    public CatRegimenFiscalMappingTests()
    {
        _mapper = new AppMapper();
    }

    [Fact]
    public void Map_CatRegimenFiscalToGenericCatalogDTO_MapeaCodigoYDescripcionCorrectamente()
    {
        var entity = new CatRegimenFiscal
        {
            Id = 1,
            Codigo = "601",
            Descripcion = "General de Ley Personas Morales",
            Fisica = false,
            Moral = true,
            IsActive = true
        };

        var dto = _mapper.Map<GenericCatalogDTO>(entity);

        dto.Should().NotBeNull();
        dto.Id.Should().Be(1);
        dto.Codigo.Should().Be("601");
        dto.Descripcion.Should().Be("General de Ley Personas Morales");
        dto.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Map_GenericCatalogDTOToCatRegimenFiscal_MapeaCodigoYDescripcionCorrectamente()
    {
        var dto = new GenericCatalogDTO
        {
            Id = 19,
            Codigo = "626",
            Descripcion = "Régimen Simplificado de Confianza (RESICO)",
            IsActive = true
        };

        var entity = _mapper.Map<CatRegimenFiscal>(dto);

        entity.Should().NotBeNull();
        entity.Id.Should().Be(19);
        entity.Codigo.Should().Be("626");
        entity.Descripcion.Should().Be("Régimen Simplificado de Confianza (RESICO)");
    }
}
