using System;
using System.Collections.Generic;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EntidadesDeFormsFrontEnd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormField",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Placeholder = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Label = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Value = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ValidationsJson = table.Column<List<FormValidation>>(type: "jsonb", nullable: false),
                    OptionsJson = table.Column<List<SelectFormOption>>(type: "jsonb", nullable: true),
                    FormularioCatalogId = table.Column<int>(type: "integer", nullable: false),
                    FormularioItemId = table.Column<int>(type: "integer", nullable: false),
                    CatalogId = table.Column<int>(type: "integer", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormField_CatalogItem_FormularioCatalogId_FormularioItemId",
                        columns: x => new { x.FormularioCatalogId, x.FormularioItemId },
                        principalTable: "CatalogItem",
                        principalColumns: new[] { "CatalogId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormField_Catalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "Catalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormField_CatalogId",
                table: "FormField",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_FormField_Formulario",
                table: "FormField",
                columns: new[] { "FormularioCatalogId", "FormularioItemId" });

            migrationBuilder.CreateIndex(
                name: "UX_FormField_Formulario_Name",
                table: "FormField",
                columns: new[] { "FormularioCatalogId", "FormularioItemId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormField");
        }
    }
}
