using AutoMapper;
using Domain.Entities;
using DTO.GenericCatalog;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UseCases.Common;
using Xunit;

namespace MesaFacil.API.UnitTests.UseCases.Compras;

public class CatRegimenFiscalMappingTests
{
    private readonly IMapper _mapper;

    public CatRegimenFiscalMappingTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<GenericCatalogProfile>();
        });
        var sp = services.BuildServiceProvider();
        _mapper = sp.GetRequiredService<IMapper>();
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
