using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHostessReservacionesWaitlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FilaEsperaItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false),
                    IdSucursal = table.Column<int>(type: "integer", nullable: false),
                    NombreCliente = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TelefonoCliente = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    NumeroPersonas = table.Column<int>(type: "integer", nullable: false),
                    ZonaPreferencia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MinutosEstimados = table.Column<int>(type: "integer", nullable: false),
                    RegistradoEn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    NotificadoEn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IdMesaAsignada = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilaEsperaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilaEsperaItems_Mesa_IdMesaAsignada",
                        column: x => x.IdMesaAsignada,
                        principalTable: "Mesa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FilaEsperaItems_Sucursal_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReservasMesa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false),
                    IdSucursal = table.Column<int>(type: "integer", nullable: false),
                    IdMesa = table.Column<int>(type: "integer", nullable: true),
                    NombreCliente = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TelefonoCliente = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaHoraReserva = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    NumeroPersonas = table.Column<int>(type: "integer", nullable: false),
                    ZonaPreferencia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EstadoReserva = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    AnticipoPagado = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Notas = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservasMesa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservasMesa_Mesa_IdMesa",
                        column: x => x.IdMesa,
                        principalTable: "Mesa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ReservasMesa_Sucursal_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilaEsperaItems_IdEmpresa_IdSucursal_Estado_RegistradoEn",
                table: "FilaEsperaItems",
                columns: new[] { "IdEmpresa", "IdSucursal", "Estado", "RegistradoEn" });

            migrationBuilder.CreateIndex(
                name: "IX_FilaEsperaItems_IdMesaAsignada",
                table: "FilaEsperaItems",
                column: "IdMesaAsignada");

            migrationBuilder.CreateIndex(
                name: "IX_FilaEsperaItems_IdSucursal",
                table: "FilaEsperaItems",
                column: "IdSucursal");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasMesa_IdEmpresa_IdSucursal_FechaHoraReserva_EstadoRe~",
                table: "ReservasMesa",
                columns: new[] { "IdEmpresa", "IdSucursal", "FechaHoraReserva", "EstadoReserva" });

            migrationBuilder.CreateIndex(
                name: "IX_ReservasMesa_IdMesa",
                table: "ReservasMesa",
                column: "IdMesa");

            migrationBuilder.CreateIndex(
                name: "IX_ReservasMesa_IdSucursal",
                table: "ReservasMesa",
                column: "IdSucursal");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilaEsperaItems");

            migrationBuilder.DropTable(
                name: "ReservasMesa");
        }
    }
}
