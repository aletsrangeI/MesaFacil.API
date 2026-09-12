using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCuentasPorPagarCxP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CuentasPorPagar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false),
                    IdSucursal = table.Column<int>(type: "integer", nullable: false),
                    IdProveedor = table.Column<int>(type: "integer", nullable: false),
                    IdCompraFactura = table.Column<int>(type: "integer", nullable: true),
                    MontoTotal = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    SaldoInsoluto = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Estado = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuentasPorPagar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuentasPorPagar_ComprasFactura_IdCompraFactura",
                        column: x => x.IdCompraFactura,
                        principalTable: "ComprasFactura",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasPorPagar_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasPorPagar_Proveedores_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuentasPorPagar_Sucursal_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PagosCuentaPorPagar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCuentaPorPagar = table.Column<int>(type: "integer", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IdMetodoPago = table.Column<int>(type: "integer", nullable: false),
                    IdMovimientoCaja = table.Column<int>(type: "integer", nullable: true),
                    ReferenciaBancaria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ComprobanteUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IdUsuario = table.Column<int>(type: "integer", nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosCuentaPorPagar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosCuentaPorPagar_CatMetodoDePago_IdMetodoPago",
                        column: x => x.IdMetodoPago,
                        principalTable: "CatMetodoDePago",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagosCuentaPorPagar_CuentasPorPagar_IdCuentaPorPagar",
                        column: x => x.IdCuentaPorPagar,
                        principalTable: "CuentasPorPagar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PagosCuentaPorPagar_MovimientoCaja_IdMovimientoCaja",
                        column: x => x.IdMovimientoCaja,
                        principalTable: "MovimientoCaja",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PagosCuentaPorPagar_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorPagar_IdCompraFactura",
                table: "CuentasPorPagar",
                column: "IdCompraFactura");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorPagar_IdEmpresa_IdSucursal_Estado_FechaVencimiento",
                table: "CuentasPorPagar",
                columns: new[] { "IdEmpresa", "IdSucursal", "Estado", "FechaVencimiento" });

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorPagar_IdProveedor",
                table: "CuentasPorPagar",
                column: "IdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_CuentasPorPagar_IdSucursal",
                table: "CuentasPorPagar",
                column: "IdSucursal");

            migrationBuilder.CreateIndex(
                name: "IX_PagosCuentaPorPagar_IdCuentaPorPagar",
                table: "PagosCuentaPorPagar",
                column: "IdCuentaPorPagar");

            migrationBuilder.CreateIndex(
                name: "IX_PagosCuentaPorPagar_IdMetodoPago",
                table: "PagosCuentaPorPagar",
                column: "IdMetodoPago");

            migrationBuilder.CreateIndex(
                name: "IX_PagosCuentaPorPagar_IdMovimientoCaja",
                table: "PagosCuentaPorPagar",
                column: "IdMovimientoCaja");

            migrationBuilder.CreateIndex(
                name: "IX_PagosCuentaPorPagar_IdUsuario",
                table: "PagosCuentaPorPagar",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagosCuentaPorPagar");

            migrationBuilder.DropTable(
                name: "CuentasPorPagar");
        }
    }
}
