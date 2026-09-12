using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRecetasEscandallos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recetas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdProducto = table.Column<int>(type: "integer", nullable: true),
                    IdVariante = table.Column<int>(type: "integer", nullable: true),
                    IdOpcionModificador = table.Column<int>(type: "integer", nullable: true),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    EsSubReceta = table.Column<bool>(type: "boolean", nullable: false),
                    Rendimiento = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    IdUnidadMedidaRendimiento = table.Column<int>(type: "integer", nullable: false),
                    CostoEstimadoUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recetas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recetas_OpcionModificador_IdOpcionModificador",
                        column: x => x.IdOpcionModificador,
                        principalTable: "OpcionModificador",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recetas_Producto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recetas_UnidadesMedida_IdUnidadMedidaRendimiento",
                        column: x => x.IdUnidadMedidaRendimiento,
                        principalTable: "UnidadesMedida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recetas_VarianteProducto_IdVariante",
                        column: x => x.IdVariante,
                        principalTable: "VarianteProducto",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RecetaDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdReceta = table.Column<int>(type: "integer", nullable: false),
                    IdInsumo = table.Column<int>(type: "integer", nullable: true),
                    IdSubReceta = table.Column<int>(type: "integer", nullable: true),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    IdUnidadMedida = table.Column<int>(type: "integer", nullable: false),
                    PorcentajeMermaEsperada = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CostoCalculado = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecetaDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecetaDetalles_Insumos_IdInsumo",
                        column: x => x.IdInsumo,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecetaDetalles_Recetas_IdReceta",
                        column: x => x.IdReceta,
                        principalTable: "Recetas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecetaDetalles_Recetas_IdSubReceta",
                        column: x => x.IdSubReceta,
                        principalTable: "Recetas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecetaDetalles_UnidadesMedida_IdUnidadMedida",
                        column: x => x.IdUnidadMedida,
                        principalTable: "UnidadesMedida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecetaDetalles_IdInsumo",
                table: "RecetaDetalles",
                column: "IdInsumo");

            migrationBuilder.CreateIndex(
                name: "IX_RecetaDetalles_IdReceta",
                table: "RecetaDetalles",
                column: "IdReceta");

            migrationBuilder.CreateIndex(
                name: "IX_RecetaDetalles_IdSubReceta",
                table: "RecetaDetalles",
                column: "IdSubReceta");

            migrationBuilder.CreateIndex(
                name: "IX_RecetaDetalles_IdUnidadMedida",
                table: "RecetaDetalles",
                column: "IdUnidadMedida");

            migrationBuilder.CreateIndex(
                name: "IX_Recetas_EsSubReceta",
                table: "Recetas",
                column: "EsSubReceta");

            migrationBuilder.CreateIndex(
                name: "IX_Recetas_IdOpcionModificador",
                table: "Recetas",
                column: "IdOpcionModificador");

            migrationBuilder.CreateIndex(
                name: "IX_Recetas_IdProducto",
                table: "Recetas",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Recetas_IdUnidadMedidaRendimiento",
                table: "Recetas",
                column: "IdUnidadMedidaRendimiento");

            migrationBuilder.CreateIndex(
                name: "IX_Recetas_IdVariante",
                table: "Recetas",
                column: "IdVariante");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecetaDetalles");

            migrationBuilder.DropTable(
                name: "Recetas");
        }
    }
}
