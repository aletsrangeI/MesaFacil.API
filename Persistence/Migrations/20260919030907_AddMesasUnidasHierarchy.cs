using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMesasUnidasHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdMesaPrincipal",
                table: "Mesa",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mesa_IdMesaPrincipal",
                table: "Mesa",
                column: "IdMesaPrincipal");

            migrationBuilder.AddForeignKey(
                name: "FK_Mesa_Mesa_IdMesaPrincipal",
                table: "Mesa",
                column: "IdMesaPrincipal",
                principalTable: "Mesa",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mesa_Mesa_IdMesaPrincipal",
                table: "Mesa");

            migrationBuilder.DropIndex(
                name: "IX_Mesa_IdMesaPrincipal",
                table: "Mesa");

            migrationBuilder.DropColumn(
                name: "IdMesaPrincipal",
                table: "Mesa");
        }
    }
}
