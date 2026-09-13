using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddImpresorasDiagnostico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfiguracionImpresora",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdSucursal = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    TipoConexion = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AnchoPapel = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    DireccionIp = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: true),
                    Puerto = table.Column<int>(type: "integer", nullable: false, defaultValue: 9100),
                    AperturaCajon = table.Column<bool>(type: "boolean", nullable: false),
                    Autocorte = table.Column<bool>(type: "boolean", nullable: false),
                    EstacionAsociada = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionImpresora", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracionImpresora_Sucursal_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionImpresora_IdSucursal",
                table: "ConfiguracionImpresora",
                column: "IdSucursal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfiguracionImpresora");
        }
    }
}
