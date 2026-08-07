using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixSucursalFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedido_Sucursal_SucursalId",
                table: "Pedido");

            migrationBuilder.DropIndex(
                name: "IX_Pedido_SucursalId",
                table: "Pedido");

            migrationBuilder.DropColumn(
                name: "SucursalId",
                table: "Pedido");

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_IdSucursal",
                table: "Pedido",
                column: "IdSucursal");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedido_Sucursal_IdSucursal",
                table: "Pedido",
                column: "IdSucursal",
                principalTable: "Sucursal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pedido_Sucursal_IdSucursal",
                table: "Pedido");

            migrationBuilder.DropIndex(
                name: "IX_Pedido_IdSucursal",
                table: "Pedido");

            migrationBuilder.AddColumn<int>(
                name: "SucursalId",
                table: "Pedido",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Pedido_SucursalId",
                table: "Pedido",
                column: "SucursalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pedido_Sucursal_SucursalId",
                table: "Pedido",
                column: "SucursalId",
                principalTable: "Sucursal",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
