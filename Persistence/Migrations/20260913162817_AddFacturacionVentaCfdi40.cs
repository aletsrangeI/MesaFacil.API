using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFacturacionVentaCfdi40 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmpresaBolsasTimbres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false),
                    TimbresDisponibles = table.Column<int>(type: "integer", nullable: false),
                    TimbresConsumidos = table.Column<int>(type: "integer", nullable: false),
                    UltimaRecargaFecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresaBolsasTimbres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpresaBolsasTimbres_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmpresaConfiguracionesPAC",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false),
                    ProveedorPAC = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    PacApiKey = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PacApiSecret = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EsProduccion = table.Column<bool>(type: "boolean", nullable: false),
                    CertificadoCerBase64 = table.Column<string>(type: "text", nullable: true),
                    LlaveKeyBase64 = table.Column<string>(type: "text", nullable: true),
                    PasswordKeyCifrado = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    SerieFacturacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FolioSiguiente = table.Column<int>(type: "integer", nullable: false),
                    LugarExpedicionCP = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    RfcEmisor = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    RazonSocialEmisor = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    RegimenFiscalEmisor = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmpresaConfiguracionesPAC", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmpresaConfiguracionesPAC_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FacturasVenta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false),
                    IdSucursal = table.Column<int>(type: "integer", nullable: false),
                    PedidoId = table.Column<Guid>(type: "uuid", nullable: false),
                    UUID = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    Serie = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Folio = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FechaTimbrado = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RfcReceptor = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    NombreReceptor = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    RegimenFiscalReceptor = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CodigoPostalReceptor = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    UsoCfdi = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    FormaPago = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    MetodoPago = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Descuento = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Iva = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CadenaOriginalSat = table.Column<string>(type: "text", nullable: true),
                    SelloDigitalSat = table.Column<string>(type: "text", nullable: true),
                    SelloDigitalEmisor = table.Column<string>(type: "text", nullable: true),
                    NoCertificadoSat = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    XmlSellado = table.Column<string>(type: "text", nullable: true),
                    EstadoFiscal = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MotivoCancelacion = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    FechaCancelacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    TicketAutofacturaGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    VigenciaAutofactura = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GeneradaPorAutofactura = table.Column<bool>(type: "boolean", nullable: false),
                    CorreoEnviadoEn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturasVenta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacturasVenta_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FacturasVenta_Sucursal_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConsumosTimbreHistorial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false),
                    IdEmpresaBolsaTimbres = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    SaldoResultante = table.Column<int>(type: "integer", nullable: false),
                    IdFacturaVenta = table.Column<int>(type: "integer", nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    FechaMovimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumosTimbreHistorial", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsumosTimbreHistorial_EmpresaBolsasTimbres_IdEmpresaBolsa~",
                        column: x => x.IdEmpresaBolsaTimbres,
                        principalTable: "EmpresaBolsasTimbres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsumosTimbreHistorial_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsumosTimbreHistorial_FacturasVenta_IdFacturaVenta",
                        column: x => x.IdFacturaVenta,
                        principalTable: "FacturasVenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FacturaVentaDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdFacturaVenta = table.Column<int>(type: "integer", nullable: false),
                    PedidoDetalleId = table.Column<Guid>(type: "uuid", nullable: true),
                    ClaveProdServ = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    ClaveUnidad = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    ValorUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Importe = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    BaseIva = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TasaIva = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    ImporteIva = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacturaVentaDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FacturaVentaDetalles_FacturasVenta_IdFacturaVenta",
                        column: x => x.IdFacturaVenta,
                        principalTable: "FacturasVenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosTimbreHistorial_IdEmpresa_FechaMovimiento",
                table: "ConsumosTimbreHistorial",
                columns: new[] { "IdEmpresa", "FechaMovimiento" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosTimbreHistorial_IdEmpresaBolsaTimbres",
                table: "ConsumosTimbreHistorial",
                column: "IdEmpresaBolsaTimbres");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosTimbreHistorial_IdFacturaVenta",
                table: "ConsumosTimbreHistorial",
                column: "IdFacturaVenta");

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaBolsasTimbres_IdEmpresa",
                table: "EmpresaBolsasTimbres",
                column: "IdEmpresa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmpresaConfiguracionesPAC_IdEmpresa",
                table: "EmpresaConfiguracionesPAC",
                column: "IdEmpresa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacturasVenta_IdEmpresa",
                table: "FacturasVenta",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_FacturasVenta_IdSucursal",
                table: "FacturasVenta",
                column: "IdSucursal");

            migrationBuilder.CreateIndex(
                name: "IX_FacturasVenta_PedidoId",
                table: "FacturasVenta",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_FacturasVenta_TicketAutofacturaGuid",
                table: "FacturasVenta",
                column: "TicketAutofacturaGuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacturasVenta_UUID",
                table: "FacturasVenta",
                column: "UUID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacturaVentaDetalles_IdFacturaVenta",
                table: "FacturaVentaDetalles",
                column: "IdFacturaVenta");

            migrationBuilder.CreateIndex(
                name: "IX_FacturaVentaDetalles_PedidoDetalleId",
                table: "FacturaVentaDetalles",
                column: "PedidoDetalleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsumosTimbreHistorial");

            migrationBuilder.DropTable(
                name: "EmpresaConfiguracionesPAC");

            migrationBuilder.DropTable(
                name: "FacturaVentaDetalles");

            migrationBuilder.DropTable(
                name: "EmpresaBolsasTimbres");

            migrationBuilder.DropTable(
                name: "FacturasVenta");
        }
    }
}
