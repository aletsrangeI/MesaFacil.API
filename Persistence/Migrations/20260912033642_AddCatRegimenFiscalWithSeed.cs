using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCatRegimenFiscalWithSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatRegimenFiscal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Fisica = table.Column<bool>(type: "boolean", nullable: false),
                    Moral = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatRegimenFiscal", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CatRegimenFiscal",
                columns: new[] { "Id", "Codigo", "CreatedAt", "CreatedBy", "Descripcion", "Fisica", "IsActive", "Moral", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "601", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "General de Ley Personas Morales", false, true, true, null, "" },
                    { 2, "603", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Personas Morales con Fines no Lucrativos", false, true, true, null, "" },
                    { 3, "605", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Sueldos y Salarios e Ingresos Asimilados a Salarios", true, true, false, null, "" },
                    { 4, "606", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Arrendamiento", true, true, false, null, "" },
                    { 5, "607", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Régimen de Enajenación o Adquisición de Bienes", true, true, false, null, "" },
                    { 6, "608", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Demás ingresos", true, true, false, null, "" },
                    { 7, "610", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Residentes en el Extranjero sin Establecimiento Permanente en México", true, true, true, null, "" },
                    { 8, "611", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ingresos por Dividendos (socios y accionistas)", true, true, false, null, "" },
                    { 9, "612", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Personas Físicas con Actividades Empresariales y Profesionales", true, true, false, null, "" },
                    { 10, "614", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ingresos por intereses", true, true, false, null, "" },
                    { 11, "615", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Régimen de los ingresos por obtención de premios", true, true, false, null, "" },
                    { 12, "616", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Sin obligaciones fiscales", true, true, false, null, "" },
                    { 13, "620", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Sociedades Cooperativas de Producción que optan por diferir sus ingresos", false, true, true, null, "" },
                    { 14, "621", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Incorporación Fiscal", true, true, false, null, "" },
                    { 15, "622", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Actividades Agrícolas, Ganaderas, Silvícolas y Pesqueras", true, true, true, null, "" },
                    { 16, "623", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Opcional para Grupos de Sociedades", false, true, true, null, "" },
                    { 17, "624", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Coordinados", false, true, true, null, "" },
                    { 18, "625", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Régimen de las Actividades Empresariales con ingresos a través de Plataformas Tecnológicas", true, true, false, null, "" },
                    { 19, "626", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Régimen Simplificado de Confianza (RESICO)", true, true, true, null, "" },
                    { 20, "628", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Hidrocarburos", false, true, true, null, "" },
                    { 21, "629", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "De los Regímenes Fiscales Preferentes y de las Empresas Multinacionales", true, true, false, null, "" },
                    { 22, "630", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Enajenación de acciones en bolsa de valores", true, true, false, null, "" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatRegimenFiscal");
        }
    }
}
