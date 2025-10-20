using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LoginClaimsAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioRol_Usuario_Id",
                table: "UsuarioRol");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioRol",
                table: "UsuarioRol");

            migrationBuilder.RenameIndex(
                name: "UX_RolAccesoRuta_Rol_Path",
                table: "RolAccesoRuta",
                newName: "UX_RolAccesoRuta_Rol_AccesoRuta");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UsuarioRol",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "UsuarioRol",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "AccesoRuta",
                type: "character varying(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMenu",
                table: "AccesoRuta",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "AccesoRuta",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioRol",
                table: "UsuarioRol",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "UX_UsuarioRol_Usuario_Rol",
                table: "UsuarioRol",
                columns: new[] { "UsuarioId", "IdRol" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_AccesoRuta_Key",
                table: "AccesoRuta",
                column: "Key",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioRol_Usuario_UsuarioId",
                table: "UsuarioRol",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioRol_Usuario_UsuarioId",
                table: "UsuarioRol");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioRol",
                table: "UsuarioRol");

            migrationBuilder.DropIndex(
                name: "UX_UsuarioRol_Usuario_Rol",
                table: "UsuarioRol");

            migrationBuilder.DropIndex(
                name: "UX_AccesoRuta_Key",
                table: "AccesoRuta");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "UsuarioRol");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "AccesoRuta");

            migrationBuilder.DropColumn(
                name: "IsMenu",
                table: "AccesoRuta");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "AccesoRuta");

            migrationBuilder.RenameIndex(
                name: "UX_RolAccesoRuta_Rol_AccesoRuta",
                table: "RolAccesoRuta",
                newName: "UX_RolAccesoRuta_Rol_Path");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UsuarioRol",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioRol",
                table: "UsuarioRol",
                columns: new[] { "Id", "IdRol" });

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioRol_Usuario_Id",
                table: "UsuarioRol",
                column: "Id",
                principalTable: "Usuario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
