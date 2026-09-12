using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProveedoresComprasCFDI : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false),
                    RFC = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: false),
                    RazonSocial = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NombreComercial = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Contacto = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    RegimenFiscal = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    DiasCredito = table.Column<int>(type: "integer", nullable: false),
                    Banco = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CuentaBancaria = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proveedores_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComprasFactura",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false),
                    IdSucursal = table.Column<int>(type: "integer", nullable: false),
                    IdAlmacen = table.Column<int>(type: "integer", nullable: false),
                    IdProveedor = table.Column<int>(type: "integer", nullable: false),
                    UUID = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    Serie = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Folio = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaRecepcion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    EsCredito = table.Column<bool>(type: "boolean", nullable: false),
                    DiasCredito = table.Column<int>(type: "integer", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Subtotal = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TotalDescuento = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TotalIVA = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TotalIEPS = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Estado = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RutaArchivoXML = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RutaArchivoPDF = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IdUsuario = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprasFactura", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprasFactura_Almacenes_IdAlmacen",
                        column: x => x.IdAlmacen,
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprasFactura_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComprasFactura_Proveedores_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprasFactura_Sucursal_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ComprasFactura_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MapeosInsumoProveedor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdProveedor = table.Column<int>(type: "integer", nullable: false),
                    DescripcionSAT = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    ClaveProdServ = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    UnidadSAT = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IdInsumo = table.Column<int>(type: "integer", nullable: false),
                    FactorConversion = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaUltimaCompra = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapeosInsumoProveedor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MapeosInsumoProveedor_Insumos_IdInsumo",
                        column: x => x.IdInsumo,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MapeosInsumoProveedor_Proveedores_IdProveedor",
                        column: x => x.IdProveedor,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompraFacturaDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCompraFactura = table.Column<int>(type: "integer", nullable: false),
                    IdInsumo = table.Column<int>(type: "integer", nullable: false),
                    ClaveProdServ = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DescripcionOriginal = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    UnidadSAT = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    IdUnidadMedida = table.Column<int>(type: "integer", nullable: true),
                    FactorConversion = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CantidadInsumo = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Importe = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Descuento = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TasaIVA = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    ImporteIVA = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TasaIEPS = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    ImporteIEPS = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    ImporteTotal = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompraFacturaDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompraFacturaDetalles_ComprasFactura_IdCompraFactura",
                        column: x => x.IdCompraFactura,
                        principalTable: "ComprasFactura",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompraFacturaDetalles_Insumos_IdInsumo",
                        column: x => x.IdInsumo,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompraFacturaDetalles_UnidadesMedida_IdUnidadMedida",
                        column: x => x.IdUnidadMedida,
                        principalTable: "UnidadesMedida",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompraFacturaDetalles_IdCompraFactura",
                table: "CompraFacturaDetalles",
                column: "IdCompraFactura");

            migrationBuilder.CreateIndex(
                name: "IX_CompraFacturaDetalles_IdInsumo",
                table: "CompraFacturaDetalles",
                column: "IdInsumo");

            migrationBuilder.CreateIndex(
                name: "IX_CompraFacturaDetalles_IdUnidadMedida",
                table: "CompraFacturaDetalles",
                column: "IdUnidadMedida");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasFactura_IdAlmacen",
                table: "ComprasFactura",
                column: "IdAlmacen");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasFactura_IdEmpresa_IdSucursal_FechaEmision",
                table: "ComprasFactura",
                columns: new[] { "IdEmpresa", "IdSucursal", "FechaEmision" });

            migrationBuilder.CreateIndex(
                name: "IX_ComprasFactura_IdProveedor",
                table: "ComprasFactura",
                column: "IdProveedor");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasFactura_IdSucursal",
                table: "ComprasFactura",
                column: "IdSucursal");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasFactura_IdUsuario",
                table: "ComprasFactura",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasFactura_UUID",
                table: "ComprasFactura",
                column: "UUID");

            migrationBuilder.CreateIndex(
                name: "IX_MapeosInsumoProveedor_IdInsumo",
                table: "MapeosInsumoProveedor",
                column: "IdInsumo");

            migrationBuilder.CreateIndex(
                name: "IX_MapeosInsumoProveedor_IdProveedor_ClaveProdServ_Descripcion~",
                table: "MapeosInsumoProveedor",
                columns: new[] { "IdProveedor", "ClaveProdServ", "DescripcionSAT" });

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_IdEmpresa_RFC",
                table: "Proveedores",
                columns: new[] { "IdEmpresa", "RFC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompraFacturaDetalles");

            migrationBuilder.DropTable(
                name: "MapeosInsumoProveedor");

            migrationBuilder.DropTable(
                name: "ComprasFactura");

            migrationBuilder.DropTable(
                name: "Proveedores");
        }
    }
}
