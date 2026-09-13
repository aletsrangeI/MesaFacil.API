using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCandadoSupervisorAuditoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PinBloqueadoHasta",
                table: "Usuario",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PinIntentosFallidos",
                table: "Usuario",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PinSupervisorHash",
                table: "Usuario",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PinSupervisorSalt",
                table: "Usuario",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdUsuarioSupervisor",
                table: "EventoPedido",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MontoCancelado",
                table: "EventoPedido",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PorcentajeDescuento",
                table: "EventoPedido",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CatMotivoCancelacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatMotivoCancelacion", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventoPedido_IdUsuarioSupervisor",
                table: "EventoPedido",
                column: "IdUsuarioSupervisor");

            migrationBuilder.AddForeignKey(
                name: "FK_EventoPedido_Usuario_IdUsuarioSupervisor",
                table: "EventoPedido",
                column: "IdUsuarioSupervisor",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventoPedido_Usuario_IdUsuarioSupervisor",
                table: "EventoPedido");

            migrationBuilder.DropTable(
                name: "CatMotivoCancelacion");

            migrationBuilder.DropIndex(
                name: "IX_EventoPedido_IdUsuarioSupervisor",
                table: "EventoPedido");

            migrationBuilder.DropColumn(
                name: "PinBloqueadoHasta",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "PinIntentosFallidos",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "PinSupervisorHash",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "PinSupervisorSalt",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "IdUsuarioSupervisor",
                table: "EventoPedido");

            migrationBuilder.DropColumn(
                name: "MontoCancelado",
                table: "EventoPedido");

            migrationBuilder.DropColumn(
                name: "PorcentajeDescuento",
                table: "EventoPedido");
        }
    }
}
