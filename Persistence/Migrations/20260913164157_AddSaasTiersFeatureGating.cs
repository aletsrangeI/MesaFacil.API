using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSaasTiersFeatureGating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatPlanesSuscripcion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PrecioMensualMxn = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PrecioAnualMxn = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MaxSucursales = table.Column<int>(type: "integer", nullable: false),
                    MaxKdsBase = table.Column<int>(type: "integer", nullable: false),
                    PermiteMesas = table.Column<bool>(type: "boolean", nullable: false),
                    PermiteSplitBill = table.Column<bool>(type: "boolean", nullable: false),
                    PermiteRecetas = table.Column<bool>(type: "boolean", nullable: false),
                    PermiteCfdiXml = table.Column<bool>(type: "boolean", nullable: false),
                    PermiteCxP = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatPlanesSuscripcion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmpresasSuscripcion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false),
                    IdPlan = table.Column<int>(type: "integer", nullable: false),
                    EsPagoAnual = table.Column<bool>(type: "boolean", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFinVigencia = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EstadoSuscripcion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    KdsAddonsContratados = table.Column<int>(type: "integer", nullable: false),
                    ComanderosAddons = table.Column<int>(type: "integer", nullable: false),
                    EnPeriodoGracia = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresasSuscripcion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpresasSuscripcion_CatPlanesSuscripcion_IdPlan",
                        column: x => x.IdPlan,
                        principalTable: "CatPlanesSuscripcion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmpresasSuscripcion_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CatPlanesSuscripcion",
                columns: new[] { "Id", "Codigo", "CreatedAt", "CreatedBy", "IsActive", "MaxKdsBase", "MaxSucursales", "Nombre", "PermiteCfdiXml", "PermiteCxP", "PermiteMesas", "PermiteRecetas", "PermiteSplitBill", "PrecioAnualMxn", "PrecioMensualMxn", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "Tier1_Barra", new DateTime(2026, 9, 13, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 0, 1, "Barra & Café", false, false, false, false, false, 6710.40m, 699m, null, "" },
                    { 2, "Tier2_Pro", new DateTime(2026, 9, 13, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 1, 1, "Restaurante Pro", false, false, true, true, true, 14390.40m, 1499m, null, "" },
                    { 3, "Tier3_Multi", new DateTime(2026, 9, 13, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, 3, 0, "Multi-Sucursal", true, true, true, true, true, 25910.40m, 2699m, null, "" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatPlanesSuscripcion_Codigo",
                table: "CatPlanesSuscripcion",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpresasSuscripcion_IdEmpresa",
                table: "EmpresasSuscripcion",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresasSuscripcion_IdPlan",
                table: "EmpresasSuscripcion",
                column: "IdPlan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmpresasSuscripcion");

            migrationBuilder.DropTable(
                name: "CatPlanesSuscripcion");
        }
    }
}
