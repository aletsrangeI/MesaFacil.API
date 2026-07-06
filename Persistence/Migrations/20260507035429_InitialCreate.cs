using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccesoRuta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Path = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Group = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    IsMenu = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccesoRuta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatCredencial",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatCredencial", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatEstacionesCocina",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatEstacionesCocina", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatEstadoCuenta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatEstadoCuenta", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatEstadoItemKDS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatEstadoItemKDS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatEstadoMesa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatEstadoMesa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatEstadoPedido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatEstadoPedido", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatEstadoPedidoDetalle",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatEstadoPedidoDetalle", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatEstadoTicketCocina",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatEstadoTicketCocina", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatImpuesto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatImpuesto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatMetodoDePago",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatMetodoDePago", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatMoneda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatMoneda", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatTipoDescuento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatTipoDescuento", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CatTipoPedido",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatTipoPedido", x => x.Id);
                });

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
                name: "Formulario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formulario", x => x.Id);
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
                name: "FormField",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdFormulario = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Placeholder = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    ValidationsJson = table.Column<string>(type: "jsonb", nullable: true),
                    OptionsJson = table.Column<string>(type: "jsonb", nullable: true),
                    DataSource = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Orden = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormField", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormField_Formulario_IdFormulario",
                        column: x => x.IdFormulario,
                        principalTable: "Formulario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolAccesoRuta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdRol = table.Column<int>(type: "integer", nullable: false),
                    IdAccesoRuta = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolAccesoRuta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolAccesoRuta_AccesoRuta_IdAccesoRuta",
                        column: x => x.IdAccesoRuta,
                        principalTable: "AccesoRuta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolAccesoRuta_Rol_IdRol",
                        column: x => x.IdRol,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    IdCredencial = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_Credencial", x => new { x.IdUsuario, x.IdCredencial });
                    table.ForeignKey(
                        name: "FK_Credencial_CatCredencial_IdCredencial",
                        column: x => x.IdCredencial,
                        principalTable: "CatCredencial",
                        principalColumn: "Id",
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
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdRol = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioRol", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioRol_Rol_IdRol",
                        column: x => x.IdRol,
                        principalTable: "Rol",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioRol_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
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
                    IdEstadoMesa = table.Column<int>(type: "integer", nullable: false),
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
                        name: "FK_Mesa_CatEstadoMesa_IdEstadoMesa",
                        column: x => x.IdEstadoMesa,
                        principalTable: "CatEstadoMesa",
                        principalColumn: "Id",
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
                    Personas = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    AbiertoPor = table.Column<int>(type: "integer", nullable: true),
                    CerradoPor = table.Column<int>(type: "integer", nullable: true),
                    AbiertoEn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    CerradoEn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Notas = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IdTipoPedido = table.Column<int>(type: "integer", nullable: false),
                    IdEstadoPedido = table.Column<int>(type: "integer", nullable: false),
                    CargoServicioPct = table.Column<decimal>(type: "numeric(5,2)", nullable: false, defaultValue: 0m),
                    SucursalId = table.Column<int>(type: "integer", nullable: false),
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
                        name: "FK_Pedido_CatEstadoPedido_IdEstadoPedido",
                        column: x => x.IdEstadoPedido,
                        principalTable: "CatEstadoPedido",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Pedido_CatTipoPedido_IdTipoPedido",
                        column: x => x.IdTipoPedido,
                        principalTable: "CatTipoPedido",
                        principalColumn: "Id",
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
                        name: "FK_Pedido_Sucursal_SucursalId",
                        column: x => x.SucursalId,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IdEstacionCocina = table.Column<int>(type: "integer", nullable: true),
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
                        name: "FK_Producto_CatEstacionesCocina_IdEstacionCocina",
                        column: x => x.IdEstacionCocina,
                        principalTable: "CatEstacionesCocina",
                        principalColumn: "Id",
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
                    IdEstadoCuenta = table.Column<int>(type: "integer", nullable: false),
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
                        name: "FK_Cuenta_CatEstadoCuenta_IdEstadoCuenta",
                        column: x => x.IdEstadoCuenta,
                        principalTable: "CatEstadoCuenta",
                        principalColumn: "Id",
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
                    IdEstadoTicketCocina = table.Column<int>(type: "integer", nullable: false),
                    CompletadoEn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                        name: "FK_TicketCocina_CatEstadoTicketCocina_IdEstadoTicketCocina",
                        column: x => x.IdEstadoTicketCocina,
                        principalTable: "CatEstadoTicketCocina",
                        principalColumn: "Id",
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
                    IdTipoDescuento = table.Column<int>(type: "integer", nullable: false),
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
                        name: "FK_DescuentoAplicado_CatTipoDescuento_IdTipoDescuento",
                        column: x => x.IdTipoDescuento,
                        principalTable: "CatTipoDescuento",
                        principalColumn: "Id",
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
                    PagadoEn = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "now()"),
                    Referencia = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RecibidoPor = table.Column<int>(type: "integer", nullable: true),
                    IdMetodoDePago = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: true),
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
                        name: "FK_Pago_CatMetodoDePago_IdMetodoDePago",
                        column: x => x.IdMetodoDePago,
                        principalTable: "CatMetodoDePago",
                        principalColumn: "Id",
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
                    table.ForeignKey(
                        name: "FK_Pago_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "Id");
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
                    IdVariante = table.Column<int>(type: "integer", nullable: false),
                    ProductoNombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    VarianteNombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(9,2)", nullable: false, defaultValue: 1m),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    IdImpuesto = table.Column<int>(type: "integer", nullable: false),
                    TasaImpuesto = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    MontoImpuesto = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    Notas = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    IdEstadoPedidoDetalle = table.Column<int>(type: "integer", nullable: false),
                    Cancelado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    MotivoCancelacion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CanceladoPor = table.Column<int>(type: "integer", nullable: true),
                    CanceladoEn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                        name: "FK_PedidoDetalle_CatEstadoPedidoDetalle_IdEstadoPedidoDetalle",
                        column: x => x.IdEstadoPedidoDetalle,
                        principalTable: "CatEstadoPedidoDetalle",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidoDetalle_CatImpuesto_IdImpuesto",
                        column: x => x.IdImpuesto,
                        principalTable: "CatImpuesto",
                        principalColumn: "Id",
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
                        name: "FK_PedidoDetalle_Usuario_CanceladoPor",
                        column: x => x.CanceladoPor,
                        principalTable: "Usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PedidoDetalle_VarianteProducto_IdVariante",
                        column: x => x.IdVariante,
                        principalTable: "VarianteProducto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                    IdImpuesto = table.Column<int>(type: "integer", nullable: false),
                    IdMoneda = table.Column<int>(type: "integer", nullable: false),
                    ValidoDesde = table.Column<DateTime>(type: "date", nullable: true),
                    ValidoHasta = table.Column<DateTime>(type: "date", nullable: true),
                    Dias = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true, comment: "Ej: L-M-M-J-V-S-D"),
                    Horario = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true, comment: "Ej: 12:00-16:00"),
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
                        name: "FK_Precio_CatImpuesto_IdImpuesto",
                        column: x => x.IdImpuesto,
                        principalTable: "CatImpuesto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Precio_CatMoneda_IdMoneda",
                        column: x => x.IdMoneda,
                        principalTable: "CatMoneda",
                        principalColumn: "Id",
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
                    OpcionNombre = table.Column<string>(type: "text", nullable: false),
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
                    IdEstadoItemKDS = table.Column<int>(type: "integer", nullable: false),
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
                        name: "FK_TicketDetalle_CatEstadoItemKDS_IdEstadoItemKDS",
                        column: x => x.IdEstadoItemKDS,
                        principalTable: "CatEstadoItemKDS",
                        principalColumn: "Id",
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
                name: "UX_AccesoRuta_Key",
                table: "AccesoRuta",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_AccesoRuta_Path",
                table: "AccesoRuta",
                column: "Path",
                unique: true);

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
                name: "IX_Credencial_IdCredencial",
                table: "Credencial",
                column: "IdCredencial");

            migrationBuilder.CreateIndex(
                name: "IX_Cuenta_IdEstadoCuenta",
                table: "Cuenta",
                column: "IdEstadoCuenta");

            migrationBuilder.CreateIndex(
                name: "IX_Cuenta_IdPedido",
                table: "Cuenta",
                column: "IdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_DescuentoAplicado_IdCuenta",
                table: "DescuentoAplicado",
                column: "IdCuenta");

            migrationBuilder.CreateIndex(
                name: "IX_DescuentoAplicado_IdTipoDescuento",
                table: "DescuentoAplicado",
                column: "IdTipoDescuento");

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
                name: "IX_FormField_IdFormulario",
                table: "FormField",
                column: "IdFormulario");

            migrationBuilder.CreateIndex(
                name: "UX_FormField_Formulario_Name",
                table: "FormField",
                columns: new[] { "IdFormulario", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Formulario_Codigo",
                table: "Formulario",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GrupoModificador_IdProducto",
                table: "GrupoModificador",
                column: "IdProducto");

            migrationBuilder.CreateIndex(
                name: "IX_Menu_IdSucursal",
                table: "Menu",
                column: "IdSucursal");

            migrationBuilder.CreateIndex(
                name: "IX_Mesa_IdArea",
                table: "Mesa",
                column: "IdArea");

            migrationBuilder.CreateIndex(
                name: "IX_Mesa_IdEstadoMesa",
                table: "Mesa",
                column: "IdEstadoMesa");

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
                name: "IX_Pago_IdMetodoDePago",
                table: "Pago",
                column: "IdMetodoDePago");

            migrationBuilder.CreateIndex(
                name: "IX_Pago_RecibidoPor",
                table: "Pago",
                column: "RecibidoPor");

            migrationBuilder.CreateIndex(
                name: "IX_Pago_UsuarioId",
                table: "Pago",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_AbiertoPor",
                table: "Pedido",
                column: "AbiertoPor");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_CerradoPor",
                table: "Pedido",
                column: "CerradoPor");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdCliente",
                table: "Pedido",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdEmpresa",
                table: "Pedido",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdEstadoPedido",
                table: "Pedido",
                column: "IdEstadoPedido");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdMesa",
                table: "Pedido",
                column: "IdMesa");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdTipoPedido",
                table: "Pedido",
                column: "IdTipoPedido");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_SucursalId",
                table: "Pedido",
                column: "SucursalId");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoAsiento_IdPedido_NumeroAsiento",
                table: "PedidoAsiento",
                columns: new[] { "IdPedido", "NumeroAsiento" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalle_CanceladoPor",
                table: "PedidoDetalle",
                column: "CanceladoPor");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalle_IdAsiento",
                table: "PedidoDetalle",
                column: "IdAsiento");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalle_IdEstadoPedidoDetalle",
                table: "PedidoDetalle",
                column: "IdEstadoPedidoDetalle");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoDetalle_IdImpuesto",
                table: "PedidoDetalle",
                column: "IdImpuesto");

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
                name: "IX_PedidoModificador_IdDetalle",
                table: "PedidoModificador",
                column: "IdDetalle");

            migrationBuilder.CreateIndex(
                name: "IX_PedidoModificador_IdOpcion",
                table: "PedidoModificador",
                column: "IdOpcion");

            migrationBuilder.CreateIndex(
                name: "IX_Precio_IdImpuesto",
                table: "Precio",
                column: "IdImpuesto");

            migrationBuilder.CreateIndex(
                name: "IX_Precio_IdMoneda",
                table: "Precio",
                column: "IdMoneda");

            migrationBuilder.CreateIndex(
                name: "IX_Precio_IdVariante",
                table: "Precio",
                column: "IdVariante");

            migrationBuilder.CreateIndex(
                name: "IX_Precio_ProductoId",
                table: "Precio",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdCategoria",
                table: "Producto",
                column: "IdCategoria");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_IdEstacionCocina",
                table: "Producto",
                column: "IdEstacionCocina");

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
                name: "IX_RolAccesoRuta_IdAccesoRuta",
                table: "RolAccesoRuta",
                column: "IdAccesoRuta");

            migrationBuilder.CreateIndex(
                name: "UX_RolAccesoRuta_Rol_AccesoRuta",
                table: "RolAccesoRuta",
                columns: new[] { "IdRol", "IdAccesoRuta" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sucursal_IdEmpresa",
                table: "Sucursal",
                column: "IdEmpresa");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCocina_IdEstacion",
                table: "TicketCocina",
                column: "IdEstacion");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCocina_IdEstadoTicketCocina",
                table: "TicketCocina",
                column: "IdEstadoTicketCocina");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCocina_IdPedido",
                table: "TicketCocina",
                column: "IdPedido");

            migrationBuilder.CreateIndex(
                name: "IX_TicketDetalle_IdDetalle",
                table: "TicketDetalle",
                column: "IdDetalle");

            migrationBuilder.CreateIndex(
                name: "IX_TicketDetalle_IdEstadoItemKDS",
                table: "TicketDetalle",
                column: "IdEstadoItemKDS");

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
                name: "UX_UsuarioRol_Usuario_Rol",
                table: "UsuarioRol",
                columns: new[] { "UsuarioId", "IdRol" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VarianteProducto_IdProducto",
                table: "VarianteProducto",
                column: "IdProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "FormField");

            migrationBuilder.DropTable(
                name: "MovimientoCaja");

            migrationBuilder.DropTable(
                name: "Pago");

            migrationBuilder.DropTable(
                name: "PedidoModificador");

            migrationBuilder.DropTable(
                name: "Precio");

            migrationBuilder.DropTable(
                name: "RolAccesoRuta");

            migrationBuilder.DropTable(
                name: "TicketDetalle");

            migrationBuilder.DropTable(
                name: "UsuarioRol");

            migrationBuilder.DropTable(
                name: "CatCredencial");

            migrationBuilder.DropTable(
                name: "CatTipoDescuento");

            migrationBuilder.DropTable(
                name: "Formulario");

            migrationBuilder.DropTable(
                name: "Turno");

            migrationBuilder.DropTable(
                name: "CatMetodoDePago");

            migrationBuilder.DropTable(
                name: "Cuenta");

            migrationBuilder.DropTable(
                name: "OpcionModificador");

            migrationBuilder.DropTable(
                name: "CatMoneda");

            migrationBuilder.DropTable(
                name: "AccesoRuta");

            migrationBuilder.DropTable(
                name: "CatEstadoItemKDS");

            migrationBuilder.DropTable(
                name: "PedidoDetalle");

            migrationBuilder.DropTable(
                name: "TicketCocina");

            migrationBuilder.DropTable(
                name: "Rol");

            migrationBuilder.DropTable(
                name: "CatEstadoCuenta");

            migrationBuilder.DropTable(
                name: "GrupoModificador");

            migrationBuilder.DropTable(
                name: "CatEstadoPedidoDetalle");

            migrationBuilder.DropTable(
                name: "CatImpuesto");

            migrationBuilder.DropTable(
                name: "PedidoAsiento");

            migrationBuilder.DropTable(
                name: "VarianteProducto");

            migrationBuilder.DropTable(
                name: "CatEstadoTicketCocina");

            migrationBuilder.DropTable(
                name: "EstacionCocina");

            migrationBuilder.DropTable(
                name: "Pedido");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "CatEstadoPedido");

            migrationBuilder.DropTable(
                name: "CatTipoPedido");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Mesa");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "CatEstacionesCocina");

            migrationBuilder.DropTable(
                name: "CategoriaMenus");

            migrationBuilder.DropTable(
                name: "Areas");

            migrationBuilder.DropTable(
                name: "CatEstadoMesa");

            migrationBuilder.DropTable(
                name: "Menu");

            migrationBuilder.DropTable(
                name: "Sucursal");

            migrationBuilder.DropTable(
                name: "Empresa");
        }
    }
}
