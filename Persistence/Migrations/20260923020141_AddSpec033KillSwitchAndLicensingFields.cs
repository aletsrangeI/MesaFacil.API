using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSpec033KillSwitchAndLicensingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactoWhatsApp",
                table: "EmpresasSuscripcion",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoSuspension",
                table: "EmpresasSuscripcion",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimaActualizacionHub",
                table: "EmpresasSuscripcion",
                type: "timestamp without time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactoWhatsApp",
                table: "EmpresasSuscripcion");

            migrationBuilder.DropColumn(
                name: "MotivoSuspension",
                table: "EmpresasSuscripcion");

            migrationBuilder.DropColumn(
                name: "UltimaActualizacionHub",
                table: "EmpresasSuscripcion");
        }
    }
}
