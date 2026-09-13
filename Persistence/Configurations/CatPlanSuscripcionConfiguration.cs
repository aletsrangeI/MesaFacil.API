using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class CatPlanSuscripcionConfiguration : IEntityTypeConfiguration<CatPlanSuscripcion>
{
    public void Configure(EntityTypeBuilder<CatPlanSuscripcion> builder)
    {
        builder.ToTable("CatPlanesSuscripcion");
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Codigo).IsUnique();

        // Spec 021: Seed de los 3 tiers comerciales (ver 03_Estrategia_Precios_y_Packaging.md).
        // Se mantiene desactivado el gating por defecto (FeatureGating:Enabled = false), este
        // catálogo únicamente queda listo en BD para cuando el negocio decida activarlo.
        var creado = new DateTime(2026, 9, 13, 0, 0, 0, DateTimeKind.Utc);
        builder.HasData(
            new CatPlanSuscripcion
            {
                Id = 1,
                Codigo = CodigosPlanSuscripcion.Tier1_Barra,
                Nombre = "Barra & Café",
                PrecioMensualMxn = 699m,
                PrecioAnualMxn = 6710.40m, // 699 * 12 * 0.80 (20% descuento anual)
                MaxSucursales = 1,
                MaxKdsBase = 0,
                PermiteMesas = false,
                PermiteSplitBill = false,
                PermiteRecetas = false,
                PermiteCfdiXml = false,
                PermiteCxP = false,
                IsActive = true,
                CreatedAt = creado,
                CreatedBy = "System",
                UpdatedBy = ""
            },
            new CatPlanSuscripcion
            {
                Id = 2,
                Codigo = CodigosPlanSuscripcion.Tier2_Pro,
                Nombre = "Restaurante Pro",
                PrecioMensualMxn = 1499m,
                PrecioAnualMxn = 14390.40m, // 1499 * 12 * 0.80
                MaxSucursales = 1,
                MaxKdsBase = 1,
                PermiteMesas = true,
                PermiteSplitBill = true,
                PermiteRecetas = true,
                PermiteCfdiXml = false,
                PermiteCxP = false,
                IsActive = true,
                CreatedAt = creado,
                CreatedBy = "System",
                UpdatedBy = ""
            },
            new CatPlanSuscripcion
            {
                Id = 3,
                Codigo = CodigosPlanSuscripcion.Tier3_Multi,
                Nombre = "Multi-Sucursal",
                PrecioMensualMxn = 2699m,
                PrecioAnualMxn = 25910.40m, // 2699 * 12 * 0.80
                MaxSucursales = 0, // 0 = ilimitadas/multi-sucursal con descuento en extras
                MaxKdsBase = 3,
                PermiteMesas = true,
                PermiteSplitBill = true,
                PermiteRecetas = true,
                PermiteCfdiXml = true,
                PermiteCxP = true,
                IsActive = true,
                CreatedAt = creado,
                CreatedBy = "System",
                UpdatedBy = ""
            });
    }
}
