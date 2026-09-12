using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInsumosAlmacenesKardex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Almacenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdSucursal = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    TipoAlmacen = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EsPrincipal = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Almacenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Almacenes_Sucursal_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoriasInsumo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasInsumo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnidadesMedida",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnidadesMedida", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TraspasosAlmacen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Folio = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IdAlmacenOrigen = table.Column<int>(type: "integer", nullable: false),
                    IdAlmacenDestino = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IdUsuarioSolicita = table.Column<int>(type: "integer", nullable: false),
                    IdUsuarioRecibe = table.Column<int>(type: "integer", nullable: true),
                    FechaSolicitud = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaRecepcion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraspasosAlmacen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TraspasosAlmacen_Almacenes_IdAlmacenDestino",
                        column: x => x.IdAlmacenDestino,
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TraspasosAlmacen_Almacenes_IdAlmacenOrigen",
                        column: x => x.IdAlmacenOrigen,
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TraspasosAlmacen_Usuario_IdUsuarioRecibe",
                        column: x => x.IdUsuarioRecibe,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TraspasosAlmacen_Usuario_IdUsuarioSolicita",
                        column: x => x.IdUsuarioSolicita,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FactoresConversion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUnidadOrigen = table.Column<int>(type: "integer", nullable: false),
                    IdUnidadDestino = table.Column<int>(type: "integer", nullable: false),
                    Factor = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactoresConversion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactoresConversion_UnidadesMedida_IdUnidadDestino",
                        column: x => x.IdUnidadDestino,
                        principalTable: "UnidadesMedida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FactoresConversion_UnidadesMedida_IdUnidadOrigen",
                        column: x => x.IdUnidadOrigen,
                        principalTable: "UnidadesMedida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Insumos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IdCategoriaInsumo = table.Column<int>(type: "integer", nullable: false),
                    IdUnidadMedidaBase = table.Column<int>(type: "integer", nullable: false),
                    CostoPromedio = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    UltimoCosto = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    StockMinimo = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    StockMaximo = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    EsCritico = table.Column<bool>(type: "boolean", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insumos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Insumos_CategoriasInsumo_IdCategoriaInsumo",
                        column: x => x.IdCategoriaInsumo,
                        principalTable: "CategoriasInsumo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Insumos_UnidadesMedida_IdUnidadMedidaBase",
                        column: x => x.IdUnidadMedidaBase,
                        principalTable: "UnidadesMedida",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventarioExistencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdAlmacen = table.Column<int>(type: "integer", nullable: false),
                    IdInsumo = table.Column<int>(type: "integer", nullable: false),
                    StockActual = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    FechaUltimoMovimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventarioExistencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventarioExistencias_Almacenes_IdAlmacen",
                        column: x => x.IdAlmacen,
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventarioExistencias_Insumos_IdInsumo",
                        column: x => x.IdInsumo,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KardexMovimientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdAlmacen = table.Column<int>(type: "integer", nullable: false),
                    IdInsumo = table.Column<int>(type: "integer", nullable: false),
                    TipoMovimiento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Submotivo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CostoUnitario = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CostoTotal = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    SaldoAnterior = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    SaldoNuevo = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CostoPromedioResultante = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    DocumentoReferencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IdUsuario = table.Column<int>(type: "integer", nullable: true),
                    FechaHora = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KardexMovimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KardexMovimientos_Almacenes_IdAlmacen",
                        column: x => x.IdAlmacen,
                        principalTable: "Almacenes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KardexMovimientos_Insumos_IdInsumo",
                        column: x => x.IdInsumo,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KardexMovimientos_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TraspasoAlmacenDetalles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTraspasoAlmacen = table.Column<int>(type: "integer", nullable: false),
                    IdInsumo = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CantidadRecibida = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraspasoAlmacenDetalles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TraspasoAlmacenDetalles_Insumos_IdInsumo",
                        column: x => x.IdInsumo,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraspasoAlmacenDetalles_TraspasosAlmacen_IdTraspasoAlmacen",
                        column: x => x.IdTraspasoAlmacen,
                        principalTable: "TraspasosAlmacen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Almacenes_IdSucursal",
                table: "Almacenes",
                column: "IdSucursal");

            migrationBuilder.CreateIndex(
                name: "IX_FactoresConversion_IdUnidadDestino",
                table: "FactoresConversion",
                column: "IdUnidadDestino");

            migrationBuilder.CreateIndex(
                name: "IX_FactoresConversion_IdUnidadOrigen",
                table: "FactoresConversion",
                column: "IdUnidadOrigen");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_IdCategoriaInsumo",
                table: "Insumos",
                column: "IdCategoriaInsumo");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_IdUnidadMedidaBase",
                table: "Insumos",
                column: "IdUnidadMedidaBase");

            migrationBuilder.CreateIndex(
                name: "IX_InventarioExistencias_IdAlmacen_IdInsumo",
                table: "InventarioExistencias",
                columns: new[] { "IdAlmacen", "IdInsumo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventarioExistencias_IdInsumo",
                table: "InventarioExistencias",
                column: "IdInsumo");

            migrationBuilder.CreateIndex(
                name: "IX_KardexMovimientos_IdAlmacen_IdInsumo_FechaHora",
                table: "KardexMovimientos",
                columns: new[] { "IdAlmacen", "IdInsumo", "FechaHora" });

            migrationBuilder.CreateIndex(
                name: "IX_KardexMovimientos_IdInsumo",
                table: "KardexMovimientos",
                column: "IdInsumo");

            migrationBuilder.CreateIndex(
                name: "IX_KardexMovimientos_IdUsuario",
                table: "KardexMovimientos",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_TraspasoAlmacenDetalles_IdInsumo",
                table: "TraspasoAlmacenDetalles",
                column: "IdInsumo");

            migrationBuilder.CreateIndex(
                name: "IX_TraspasoAlmacenDetalles_IdTraspasoAlmacen",
                table: "TraspasoAlmacenDetalles",
                column: "IdTraspasoAlmacen");

            migrationBuilder.CreateIndex(
                name: "IX_TraspasosAlmacen_IdAlmacenDestino",
                table: "TraspasosAlmacen",
                column: "IdAlmacenDestino");

            migrationBuilder.CreateIndex(
                name: "IX_TraspasosAlmacen_IdAlmacenOrigen",
                table: "TraspasosAlmacen",
                column: "IdAlmacenOrigen");

            migrationBuilder.CreateIndex(
                name: "IX_TraspasosAlmacen_IdUsuarioRecibe",
                table: "TraspasosAlmacen",
                column: "IdUsuarioRecibe");

            migrationBuilder.CreateIndex(
                name: "IX_TraspasosAlmacen_IdUsuarioSolicita",
                table: "TraspasosAlmacen",
                column: "IdUsuarioSolicita");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FactoresConversion");

            migrationBuilder.DropTable(
                name: "InventarioExistencias");

            migrationBuilder.DropTable(
                name: "KardexMovimientos");

            migrationBuilder.DropTable(
                name: "TraspasoAlmacenDetalles");

            migrationBuilder.DropTable(
                name: "Insumos");

            migrationBuilder.DropTable(
                name: "TraspasosAlmacen");

            migrationBuilder.DropTable(
                name: "CategoriasInsumo");

            migrationBuilder.DropTable(
                name: "UnidadesMedida");

            migrationBuilder.DropTable(
                name: "Almacenes");
        }
    }
}
