using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddKdsRushAndHistoryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRecuperacion",
                table: "TicketCocina",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioRecuperacion",
                table: "TicketCocina",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinutosAmbar",
                table: "EstacionCocina",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinutosRojo",
                table: "EstacionCocina",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaRecuperacion",
                table: "TicketCocina");

            migrationBuilder.DropColumn(
                name: "UsuarioRecuperacion",
                table: "TicketCocina");

            migrationBuilder.DropColumn(
                name: "MinutosAmbar",
                table: "EstacionCocina");

            migrationBuilder.DropColumn(
                name: "MinutosRojo",
                table: "EstacionCocina");
        }
    }
}
