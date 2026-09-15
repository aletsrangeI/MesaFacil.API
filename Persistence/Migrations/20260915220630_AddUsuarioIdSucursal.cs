using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioIdSucursal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdSucursal",
                table: "Usuario",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("UPDATE \"Usuario\" SET \"IdSucursal\" = 2 WHERE \"IdSucursal\" IS NULL;");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdSucursal",
                table: "Usuario",
                column: "IdSucursal");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuario_Sucursal_IdSucursal",
                table: "Usuario",
                column: "IdSucursal",
                principalTable: "Sucursal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuario_Sucursal_IdSucursal",
                table: "Usuario");

            migrationBuilder.DropIndex(
                name: "IX_Usuario_IdSucursal",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "IdSucursal",
                table: "Usuario");
        }
    }
}
