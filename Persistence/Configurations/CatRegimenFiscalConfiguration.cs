using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatRegimenFiscalConfiguration : IEntityTypeConfiguration<CatRegimenFiscal>
{
    public void Configure(EntityTypeBuilder<CatRegimenFiscal> builder)
    {
        builder.ToTable("CatRegimenFiscal");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Descripcion).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Codigo).HasMaxLength(10);

        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        const string systemUser = "System";

        builder.HasData(
            new CatRegimenFiscal { Id = 1,  Codigo = "601", Descripcion = "General de Ley Personas Morales", Fisica = false, Moral = true, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 2,  Codigo = "603", Descripcion = "Personas Morales con Fines no Lucrativos", Fisica = false, Moral = true, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 3,  Codigo = "605", Descripcion = "Sueldos y Salarios e Ingresos Asimilados a Salarios", Fisica = true, Moral = false, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 4,  Codigo = "606", Descripcion = "Arrendamiento", Fisica = true, Moral = false, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 5,  Codigo = "607", Descripcion = "Régimen de Enajenación o Adquisición de Bienes", Fisica = true, Moral = false, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 6,  Codigo = "608", Descripcion = "Demás ingresos", Fisica = true, Moral = false, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 7,  Codigo = "610", Descripcion = "Residentes en el Extranjero sin Establecimiento Permanente en México", Fisica = true, Moral = true, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 8,  Codigo = "611", Descripcion = "Ingresos por Dividendos (socios y accionistas)", Fisica = true, Moral = false, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 9,  Codigo = "612", Descripcion = "Personas Físicas con Actividades Empresariales y Profesionales", Fisica = true, Moral = false, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 10, Codigo = "614", Descripcion = "Ingresos por intereses", Fisica = true, Moral = false, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 11, Codigo = "615", Descripcion = "Régimen de los ingresos por obtención de premios", Fisica = true, Moral = false, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 12, Codigo = "616", Descripcion = "Sin obligaciones fiscales", Fisica = true, Moral = false, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 13, Codigo = "620", Descripcion = "Sociedades Cooperativas de Producción que optan por diferir sus ingresos", Fisica = false, Moral = true, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 14, Codigo = "621", Descripcion = "Incorporación Fiscal", Fisica = true, Moral = false, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 15, Codigo = "622", Descripcion = "Actividades Agrícolas, Ganaderas, Silvícolas y Pesqueras", Fisica = true, Moral = true, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 16, Codigo = "623", Descripcion = "Opcional para Grupos de Sociedades", Fisica = false, Moral = true, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 17, Codigo = "624", Descripcion = "Coordinados", Fisica = false, Moral = true, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 18, Codigo = "625", Descripcion = "Régimen de las Actividades Empresariales con ingresos a través de Plataformas Tecnológicas", Fisica = true, Moral = false, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 19, Codigo = "626", Descripcion = "Régimen Simplificado de Confianza (RESICO)", Fisica = true, Moral = true, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 20, Codigo = "628", Descripcion = "Hidrocarburos", Fisica = false, Moral = true, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 21, Codigo = "629", Descripcion = "De los Regímenes Fiscales Preferentes y de las Empresas Multinacionales", Fisica = true, Moral = false, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" },
            new CatRegimenFiscal { Id = 22, Codigo = "630", Descripcion = "Enajenación de acciones en bolsa de valores", Fisica = true, Moral = false, IsActive = true, CreatedAt = seedDate, CreatedBy = systemUser, UpdatedBy = "" }
        );
    }
}
