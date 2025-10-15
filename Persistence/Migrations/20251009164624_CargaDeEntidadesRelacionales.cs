using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CargaDeEntidadesRelacionales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItems_Catalogs_CatalogId",
                table: "CatalogItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CatalogItems",
                table: "CatalogItems");

            migrationBuilder.DropIndex(
                name: "IX_CatalogItems_CatalogId_Name",
                table: "CatalogItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CatalogItems_ValidRange",
                table: "CatalogItems");

            migrationBuilder.RenameTable(
                name: "CatalogItems",
                newName: "CatalogItem");

            migrationBuilder.RenameIndex(
                name: "IX_CatalogItems_CatalogId_Code",
                table: "CatalogItem",
                newName: "IX_CatalogItem_CatalogId_Code");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                table: "CatalogItem",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom",
                table: "CatalogItem",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "CatalogItem",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "CatalogItem",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "ExtraJson",
                table: "CatalogItem",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "CatalogItem",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CatalogItem",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CatalogItem",
                table: "CatalogItem",
                columns: new[] { "CatalogId", "Id" });

            migrationBuilder.CreateTable(
                name: "Empresa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Rfc = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsAssignable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rol", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Correo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clientes_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sucursal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    ZonaHoraria = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sucursal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sucursal_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false),
                    NombreCompleto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Correo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuario_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdSucursal = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Orden = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Areas_Sucursal_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstacionCocina",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdSucursal = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstacionCocina", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstacionCocina_Sucursal_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Menu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdSucursal = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menu_Sucursal_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Credencial",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    TipoCatalogId = table.Column<int>(type: "integer", nullable: false),
                    TipoItemId = table.Column<int>(type: "integer", nullable: false),
                    Hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Salt = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credencial", x => new { x.IdUsuario, x.TipoCatalogId, x.TipoItemId });
                    table.ForeignKey(
                        name: "FK_Credencial_CatalogItem_TipoCatalogId_TipoItemId",
                        columns: x => new { x.TipoCatalogId, x.TipoItemId },
                        principalTable: "CatalogItem",
                        principalColumns: new[] { "CatalogId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Credencial_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Turno",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdUsuario = table.Column<int>(type: "integer", nullable: false),
                    IdSucursal = table.Column<int>(type: "integer", nullable: false),
                    Apertura = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    Cierre = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    CajaInicial = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    CajaFinal = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Turno", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Turno_Sucursal_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Turno_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioRol",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    IdRol = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRol", x => new { x.Id, x.IdRol });
                    table.ForeignKey(
                        name: "FK_UsuarioRol_Rol_IdRol",
                        column: x => x.IdRol,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioRol_Usuario_Id",
                        column: x => x.Id,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mesa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdSucursal = table.Column<int>(type: "integer", nullable: false),
                    IdArea = table.Column<int>(type: "integer", nullable: true),
                    Codigo = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Asientos = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    EstadoCatalogId = table.Column<int>(type: "integer", nullable: false),
                    EstadoItemId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mesa_Areas_IdArea",
                        column: x => x.IdArea,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Mesa_CatalogItem_EstadoCatalogId_EstadoItemId",
                        columns: x => new { x.EstadoCatalogId, x.EstadoItemId },
                        principalTable: "CatalogItem",
                        principalColumns: new[] { "CatalogId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mesa_Sucursal_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CategoriaMenus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdMenu = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Orden = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriaMenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoriaMenus_Menu_IdMenu",
                        column: x => x.IdMenu,
                        principalTable: "Menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorteCaja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTurno = table.Column<int>(type: "integer", nullable: true),
                    IdSucursal = table.Column<int>(type: "integer", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    TotalVentas = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    TotalPagos = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    TotalEfectivo = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    TotalTarjeta = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    TotalEgresos = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    CajaEsperada = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    Declarado = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    Diferencia = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    CreadoEn = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    CreadoPor = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorteCaja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorteCaja_Sucursal_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CorteCaja_Turno_IdTurno",
                        column: x => x.IdTurno,
                        principalTable: "Turno",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CorteCaja_Usuario_CreadoPor",
                        column: x => x.CreadoPor,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MovimientoCaja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTurno = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    Nota = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientoCaja", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovimientoCaja_Turno_IdTurno",
                        column: x => x.IdTurno,
                        principalTable: "Turno",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pedido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEmpresa = table.Column<int>(type: "integer", nullable: false),
                    IdSucursal = table.Column<int>(type: "integer", nullable: false),
                    IdMesa = table.Column<int>(type: "integer", nullable: true),
                    IdCliente = table.Column<int>(type: "integer", nullable: true),
                    AbiertoPor = table.Column<int>(type: "integer", nullable: true),
                    CerradoPor = table.Column<int>(type: "integer", nullable: true),
                    AbiertoEn = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    CerradoEn = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    Notas = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TipoCatalogId = table.Column<int>(type: "integer", nullable: false),
                    TipoItemId = table.Column<int>(type: "integer", nullable: false),
                    EstadoCatalogId = table.Column<int>(type: "integer", nullable: false),
                    EstadoItemId = table.Column<int>(type: "integer", nullable: false),
                    CargoServicioPct = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pedido_CatalogItem_EstadoCatalogId_EstadoItemId",
                        columns: x => new { x.EstadoCatalogId, x.EstadoItemId },
                        principalTable: "CatalogItem",
                        principalColumns: new[] { "CatalogId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedido_CatalogItem_TipoCatalogId_TipoItemId",
                        columns: x => new { x.TipoCatalogId, x.TipoItemId },
                        principalTable: "CatalogItem",
                        principalColumns: new[] { "CatalogId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedido_Clientes_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pedido_Empresa_IdEmpresa",
                        column: x => x.IdEmpresa,
                        principalTable: "Empresa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedido_Mesa_IdMesa",
                        column: x => x.IdMesa,
                        principalTable: "Mesa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pedido_Sucursal_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedido_Usuario_AbiertoPor",
                        column: x => x.AbiertoPor,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Pedido_Usuario_CerradoPor",
                        column: x => x.CerradoPor,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdMenu = table.Column<int>(type: "integer", nullable: false),
                    IdCategoria = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    EstacionCatalogId = table.Column<int>(type: "integer", nullable: true),
                    EstacionItemId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Producto_CatalogItem_EstacionCatalogId_EstacionItemId",
                        columns: x => new { x.EstacionCatalogId, x.EstacionItemId },
                        principalTable: "CatalogItem",
                        principalColumns: new[] { "CatalogId", "Id" },
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Producto_CategoriaMenus_IdCategoria",
                        column: x => x.IdCategoria,
                        principalTable: "CategoriaMenus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Producto_Menu_IdMenu",
                        column: x => x.IdMenu,
                        principalTable: "Menu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cuenta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdPedido = table.Column<int>(type: "integer", nullable: false),
                    Subtotal = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    DescuentoTotal = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    CargoServicio = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    ImpuestoTotal = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    Total = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    EstadoCatalogId = table.Column<int>(type: "integer", nullable: false),
                    EstadoItemId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuenta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cuenta_CatalogItem_EstadoCatalogId_EstadoItemId",
                        columns: x => new { x.EstadoCatalogId, x.EstadoItemId },
                        principalTable: "CatalogItem",
                        principalColumns: new[] { "CatalogId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cuenta_Pedido_IdPedido",
                        column: x => x.IdPedido,
                        principalTable: "Pedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventoPedido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdPedido = table.Column<int>(type: "integer", nullable: false),
                    IdUsuario = table.Column<int>(type: "integer", nullable: true),
                    TipoEvento = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Payload = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoPedido", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventoPedido_Pedido_IdPedido",
                        column: x => x.IdPedido,
                        principalTable: "Pedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventoPedido_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PedidoAsiento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdPedido = table.Column<int>(type: "integer", nullable: false),
                    NumeroAsiento = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoAsiento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoAsiento_Pedido_IdPedido",
                        column: x => x.IdPedido,
                        principalTable: "Pedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketCocina",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdEstacion = table.Column<int>(type: "integer", nullable: false),
                    IdPedido = table.Column<int>(type: "integer", nullable: false),
                    EstadoCatalogId = table.Column<int>(type: "integer", nullable: false),
                    EstadoItemId = table.Column<int>(type: "integer", nullable: false),
                    CompletadoEn = table.Column<DateTime>(type: "timestamptz", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCocina", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketCocina_CatalogItem_EstadoCatalogId_EstadoItemId",
                        columns: x => new { x.EstadoCatalogId, x.EstadoItemId },
                        principalTable: "CatalogItem",
                        principalColumns: new[] { "CatalogId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketCocina_EstacionCocina_IdEstacion",
                        column: x => x.IdEstacion,
                        principalTable: "EstacionCocina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketCocina_Pedido_IdPedido",
                        column: x => x.IdPedido,
                        principalTable: "Pedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GrupoModificador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdProducto = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    MinSeleccion = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    MaxSeleccion = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    Obligatorio = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoModificador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrupoModificador_Producto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VarianteProducto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdProducto = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Codigo = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    EsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VarianteProducto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VarianteProducto_Producto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DescuentoAplicado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCuenta = table.Column<int>(type: "integer", nullable: false),
                    TipoCatalogId = table.Column<int>(type: "integer", nullable: false),
                    TipoItemId = table.Column<int>(type: "integer", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    Alcance = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Condiciones = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DescuentoAplicado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DescuentoAplicado_CatalogItem_TipoCatalogId_TipoItemId",
                        columns: x => new { x.TipoCatalogId, x.TipoItemId },
                        principalTable: "CatalogItem",
                        principalColumns: new[] { "CatalogId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DescuentoAplicado_Cuenta_IdCuenta",
                        column: x => x.IdCuenta,
                        principalTable: "Cuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetalleCuenta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCuenta = table.Column<int>(type: "integer", nullable: false),
                    TipoOrigen = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    IdOrigen = table.Column<int>(type: "integer", nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Monto = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleCuenta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleCuenta_Cuenta_IdCuenta",
                        column: x => x.IdCuenta,
                        principalTable: "Cuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdCuenta = table.Column<int>(type: "integer", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    Moneda = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false, defaultValue: "MXN"),
                    Propina = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    PagadoEn = table.Column<DateTime>(type: "timestamptz", nullable: false, defaultValueSql: "now()"),
                    Referencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RecibidoPor = table.Column<int>(type: "integer", nullable: true),
                    MetodoCatalogId = table.Column<int>(type: "integer", nullable: false),
                    MetodoItemId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pago", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pago_CatalogItem_MetodoCatalogId_MetodoItemId",
                        columns: x => new { x.MetodoCatalogId, x.MetodoItemId },
                        principalTable: "CatalogItem",
                        principalColumns: new[] { "CatalogId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pago_Cuenta_IdCuenta",
                        column: x => x.IdCuenta,
                        principalTable: "Cuenta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Pago_Usuario_RecibidoPor",
                        column: x => x.RecibidoPor,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OpcionModificador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdGrupo = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    PrecioExtra = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    EsDefault = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpcionModificador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpcionModificador_GrupoModificador_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "GrupoModificador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdPedido = table.Column<int>(type: "integer", nullable: false),
                    IdAsiento = table.Column<int>(type: "integer", nullable: true),
                    IdProducto = table.Column<int>(type: "integer", nullable: false),
                    IdVariante = table.Column<int>(type: "integer", nullable: true),
                    Cantidad = table.Column<decimal>(type: "numeric(9,2)", nullable: false, defaultValue: 1m),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    Notas = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    EstadoCatalogId = table.Column<int>(type: "integer", nullable: false),
                    EstadoItemId = table.Column<int>(type: "integer", nullable: false),
                    ImpuestoCatalogId = table.Column<int>(type: "integer", nullable: false),
                    ImpuestoItemId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoDetalle_CatalogItem_EstadoCatalogId_EstadoItemId",
                        columns: x => new { x.EstadoCatalogId, x.EstadoItemId },
                        principalTable: "CatalogItem",
                        principalColumns: new[] { "CatalogId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidoDetalle_CatalogItem_ImpuestoCatalogId_ImpuestoItemId",
                        columns: x => new { x.ImpuestoCatalogId, x.ImpuestoItemId },
                        principalTable: "CatalogItem",
                        principalColumns: new[] { "CatalogId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidoDetalle_PedidoAsiento_IdAsiento",
                        column: x => x.IdAsiento,
                        principalTable: "PedidoAsiento",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PedidoDetalle_Pedido_IdPedido",
                        column: x => x.IdPedido,
                        principalTable: "Pedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PedidoDetalle_Producto_IdProducto",
                        column: x => x.IdProducto,
                        principalTable: "Producto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidoDetalle_VarianteProducto_IdVariante",
                        column: x => x.IdVariante,
                        principalTable: "VarianteProducto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Precio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdVariante = table.Column<int>(type: "integer", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    Moneda = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false, defaultValue: "MXN"),
                    ImpuestoCatalogId = table.Column<int>(type: "integer", nullable: false),
                    ImpuestoItemId = table.Column<int>(type: "integer", nullable: false),
                    ValidoDesde = table.Column<DateTime>(type: "date", nullable: true),
                    ValidoHasta = table.Column<DateTime>(type: "date", nullable: true),
                    Dias = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    Horario = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ProductoId = table.Column<int>(type: "integer", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Precio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Precio_CatalogItem_ImpuestoCatalogId_ImpuestoItemId",
                        columns: x => new { x.ImpuestoCatalogId, x.ImpuestoItemId },
                        principalTable: "CatalogItem",
                        principalColumns: new[] { "CatalogId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Precio_Producto_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Producto",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Precio_VarianteProducto_IdVariante",
                        column: x => x.IdVariante,
                        principalTable: "VarianteProducto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PedidoModificador",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdDetalle = table.Column<int>(type: "integer", nullable: false),
                    IdOpcion = table.Column<int>(type: "integer", nullable: false),
                    PrecioExtra = table.Column<decimal>(type: "numeric(12,2)", nullable: false, defaultValue: 0m),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PedidoModificador", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PedidoModificador_OpcionModificador_IdOpcion",
                        column: x => x.IdOpcion,
                        principalTable: "OpcionModificador",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidoModificador_PedidoDetalle_IdDetalle",
                        column: x => x.IdDetalle,
                        principalTable: "PedidoDetalle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TicketDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdTicket = table.Column<int>(type: "integer", nullable: false),
                    IdDetalle = table.Column<int>(type: "integer", nullable: false),
                    EstadoCatalogId = table.Column<int>(type: "integer", nullable: false),
                    EstadoItemId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketDetalle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketDetalle_CatalogItem_EstadoCatalogId_EstadoItemId",
                        columns: x => new { x.EstadoCatalogId, x.EstadoItemId },
                        principalTable: "CatalogItem",
                        principalColumns: new[] { "CatalogId", "Id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketDetalle_PedidoDetalle_IdDetalle",
                        column: x => x.IdDetalle,
                        principalTable: "PedidoDetalle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TicketDetalle_TicketCocina_IdTicket",
                        column: x => x.IdTicket,
                        principalTable: "TicketCocina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItem_CatalogId_IsActive_SortOrder",
                table: "CatalogItem",
                columns: new[] { "CatalogId", "IsActive", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Areas_IdSucursal",
                table: "Areas",
                column: "IdSucursal");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriaMenus_IdMenu",
                table: "CategoriaMenus",
                column: "IdMenu");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_IdEmpresa",
                table: "Clientes",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_CorteCaja_CreadoPor",
                table: "CorteCaja",
                column: "CreadoPor");

            migrationBuilder.CreateIndex(
                name: "IX_CorteCaja_IdSucursal_FechaInicio_FechaFin",
                table: "CorteCaja",
                columns: new[] { "IdSucursal", "FechaInicio", "FechaFin" });

            migrationBuilder.CreateIndex(
                name: "IX_CorteCaja_IdTurno",
                table: "CorteCaja",
                column: "IdTurno");

            migrationBuilder.CreateIndex(
                name: "IX_Credencial_TipoCatalogId_TipoItemId",
                table: "Credencial",
                columns: new[] { "TipoCatalogId", "TipoItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_Cuenta_EstadoCatalogId_EstadoItemId",
                table: "Cuenta",
                columns: new[] { "EstadoCatalogId", "EstadoItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_Cuenta_IdPedido",
                table: "Cuenta",
                column: "IdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_DescuentoAplicado_IdCuenta",
                table: "DescuentoAplicado",
                column: "IdCuenta");

            migrationBuilder.CreateIndex(
                name: "IX_DescuentoAplicado_TipoCatalogId_TipoItemId",
                table: "DescuentoAplicado",
                columns: new[] { "TipoCatalogId", "TipoItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleCuenta_IdCuenta",
                table: "DetalleCuenta",
                column: "IdCuenta");

            migrationBuilder.CreateIndex(
                name: "IX_EstacionCocina_IdSucursal",
                table: "EstacionCocina",
                column: "IdSucursal");

            migrationBuilder.CreateIndex(
                name: "IX_EventoPedido_IdPedido",
                table: "EventoPedido",
                column: "IdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_EventoPedido_IdUsuario",
                table: "EventoPedido",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoModificador_IdProducto",
                table: "GrupoModificador",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_IdSucursal",
                table: "Menu",
                column: "IdSucursal");

            migrationBuilder.CreateIndex(
                name: "IX_Mesa_EstadoCatalogId_EstadoItemId",
                table: "Mesa",
                columns: new[] { "EstadoCatalogId", "EstadoItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_Mesa_IdArea",
                table: "Mesa",
                column: "IdArea");

            migrationBuilder.CreateIndex(
                name: "IX_Mesa_IdSucursal_Codigo",
                table: "Mesa",
                columns: new[] { "IdSucursal", "Codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovimientoCaja_IdTurno",
                table: "MovimientoCaja",
                column: "IdTurno");

            migrationBuilder.CreateIndex(
                name: "IX_OpcionModificador_IdGrupo",
                table: "OpcionModificador",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_Pago_IdCuenta",
                table: "Pago",
                column: "IdCuenta");

            migrationBuilder.CreateIndex(
                name: "IX_Pago_MetodoCatalogId_MetodoItemId",
                table: "Pago",
                columns: new[] { "MetodoCatalogId", "MetodoItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_Pago_RecibidoPor",
                table: "Pago",
                column: "RecibidoPor");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_AbiertoPor",
                table: "Pedido",
                column: "AbiertoPor");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_CerradoPor",
                table: "Pedido",
                column: "CerradoPor");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_EstadoCatalogId_EstadoItemId",
                table: "Pedido",
                columns: new[] { "EstadoCatalogId", "EstadoItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdCliente",
                table: "Pedido",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdEmpresa",
                table: "Pedido",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdMesa",
                table: "Pedido",
                column: "IdMesa");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdSucursal",
                table: "Pedido",
                column: "IdSucursal");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_TipoCatalogId_TipoItemId",
                table: "Pedido",
                columns: new[] { "TipoCatalogId", "TipoItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_PedidoAsiento_IdPedido_NumeroAsiento",
                table: "PedidoAsiento",
                columns: new[] { "IdPedido", "NumeroAsiento" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalle_EstadoCatalogId_EstadoItemId",
                table: "PedidoDetalle",
                columns: new[] { "EstadoCatalogId", "EstadoItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalle_IdAsiento",
                table: "PedidoDetalle",
                column: "IdAsiento");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalle_IdPedido",
                table: "PedidoDetalle",
                column: "IdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalle_IdProducto",
                table: "PedidoDetalle",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalle_IdVariante",
                table: "PedidoDetalle",
                column: "IdVariante");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalle_ImpuestoCatalogId_ImpuestoItemId",
                table: "PedidoDetalle",
                columns: new[] { "ImpuestoCatalogId", "ImpuestoItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_PedidoModificador_IdDetalle",
                table: "PedidoModificador",
                column: "IdDetalle");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoModificador_IdOpcion",
                table: "PedidoModificador",
                column: "IdOpcion");

            migrationBuilder.CreateIndex(
                name: "IX_Precio_IdVariante",
                table: "Precio",
                column: "IdVariante");

            migrationBuilder.CreateIndex(
                name: "IX_Precio_ImpuestoCatalogId_ImpuestoItemId",
                table: "Precio",
                columns: new[] { "ImpuestoCatalogId", "ImpuestoItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_Precio_ProductoId",
                table: "Precio",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_EstacionCatalogId_EstacionItemId",
                table: "Producto",
                columns: new[] { "EstacionCatalogId", "EstacionItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdCategoria",
                table: "Producto",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdMenu",
                table: "Producto",
                column: "IdMenu");

            migrationBuilder.CreateIndex(
                name: "IX_Rol_Nombre",
                table: "Rol",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sucursal_IdEmpresa",
                table: "Sucursal",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCocina_EstadoCatalogId_EstadoItemId",
                table: "TicketCocina",
                columns: new[] { "EstadoCatalogId", "EstadoItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketCocina_IdEstacion",
                table: "TicketCocina",
                column: "IdEstacion");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCocina_IdPedido",
                table: "TicketCocina",
                column: "IdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_TicketDetalle_EstadoCatalogId_EstadoItemId",
                table: "TicketDetalle",
                columns: new[] { "EstadoCatalogId", "EstadoItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_TicketDetalle_IdDetalle",
                table: "TicketDetalle",
                column: "IdDetalle");

            migrationBuilder.CreateIndex(
                name: "IX_TicketDetalle_IdTicket",
                table: "TicketDetalle",
                column: "IdTicket");

            migrationBuilder.CreateIndex(
                name: "IX_Turno_IdSucursal",
                table: "Turno",
                column: "IdSucursal");

            migrationBuilder.CreateIndex(
                name: "IX_Turno_IdUsuario",
                table: "Turno",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdEmpresa",
                table: "Usuario",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioRol_IdRol",
                table: "UsuarioRol",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_VarianteProducto_IdProducto",
                table: "VarianteProducto",
                column: "IdProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItem_Catalogs_CatalogId",
                table: "CatalogItem",
                column: "CatalogId",
                principalTable: "Catalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItem_Catalogs_CatalogId",
                table: "CatalogItem");

            migrationBuilder.DropTable(
                name: "CorteCaja");

            migrationBuilder.DropTable(
                name: "Credencial");

            migrationBuilder.DropTable(
                name: "DescuentoAplicado");

            migrationBuilder.DropTable(
                name: "DetalleCuenta");

            migrationBuilder.DropTable(
                name: "EventoPedido");

            migrationBuilder.DropTable(
                name: "MovimientoCaja");

            migrationBuilder.DropTable(
                name: "Pago");

            migrationBuilder.DropTable(
                name: "PedidoModificador");

            migrationBuilder.DropTable(
                name: "Precio");

            migrationBuilder.DropTable(
                name: "TicketDetalle");

            migrationBuilder.DropTable(
                name: "UsuarioRol");

            migrationBuilder.DropTable(
                name: "Turno");

            migrationBuilder.DropTable(
                name: "Cuenta");

            migrationBuilder.DropTable(
                name: "OpcionModificador");

            migrationBuilder.DropTable(
                name: "PedidoDetalle");

            migrationBuilder.DropTable(
                name: "TicketCocina");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "GrupoModificador");

            migrationBuilder.DropTable(
                name: "PedidoAsiento");

            migrationBuilder.DropTable(
                name: "VarianteProducto");

            migrationBuilder.DropTable(
                name: "EstacionCocina");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Mesa");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "CategoriaMenus");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "Menu");

            migrationBuilder.DropTable(
                name: "Sucursal");

            migrationBuilder.DropTable(
                name: "Empresa");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CatalogItem",
                table: "CatalogItem");

            migrationBuilder.DropIndex(
                name: "IX_CatalogItem_CatalogId_IsActive_SortOrder",
                table: "CatalogItem");

            migrationBuilder.RenameTable(
                name: "CatalogItem",
                newName: "CatalogItems");

            migrationBuilder.RenameIndex(
                name: "IX_CatalogItem_CatalogId_Code",
                table: "CatalogItems",
                newName: "IX_CatalogItems_CatalogId_Code");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                table: "CatalogItems",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom",
                table: "CatalogItems",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UpdatedBy",
                table: "CatalogItems",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "CatalogItems",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExtraJson",
                table: "CatalogItems",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "CatalogItems",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CatalogItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CatalogItems",
                table: "CatalogItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_CatalogId_Name",
                table: "CatalogItems",
                columns: new[] { "CatalogId", "Name" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_CatalogItems_ValidRange",
                table: "CatalogItems",
                sql: "(\"ValidFrom\" IS NULL OR \"ValidTo\" IS NULL OR \"ValidFrom\" <= \"ValidTo\")");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItems_Catalogs_CatalogId",
                table: "CatalogItems",
                column: "CatalogId",
                principalTable: "Catalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
