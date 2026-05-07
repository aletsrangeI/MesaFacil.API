using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFormularioTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FormularioId",
                table: "FormField",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Formulario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formulario", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormField_FormularioId",
                table: "FormField",
                column: "FormularioId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormField_Formulario_FormularioId",
                table: "FormField",
                column: "FormularioId",
                principalTable: "Formulario",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormField_Formulario_FormularioId",
                table: "FormField");

            migrationBuilder.DropTable(
                name: "Formulario");

            migrationBuilder.DropIndex(
                name: "IX_FormField_FormularioId",
                table: "FormField");

            migrationBuilder.DropColumn(
                name: "FormularioId",
                table: "FormField");
        }
    }
}
