using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Spec019_GuidPksOutboxFoliador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Spec 019: la BD actual es solo datos de prueba internos (el producto aún no sale a
            // mercado), por lo que se descartan explícitamente los registros operativos antes de
            // retipar las llaves de int a uuid. Postgres no permite un cast implícito int->uuid,
            // así que en vez de un ALTER COLUMN TYPE con USING inventado, truncamos primero
            // (permiso explícito del usuario) y luego recreamos las columnas ya vacías.
            migrationBuilder.Sql(@"TRUNCATE TABLE
                ""PagosCuentaPorPagar"",
                ""TicketDetalle"",
                ""TicketCocina"",
                ""EventoPedido"",
                ""DetalleCuenta"",
                ""DescuentoAplicado"",
                ""Pago"",
                ""Cuenta"",
                ""PedidoModificador"",
                ""PedidoDetalle"",
                ""PedidoAsiento"",
                ""MovimientoCaja"",
                ""Pedido""
                RESTART IDENTITY CASCADE;");

            // Postgres exige que ambos lados de una FK tengan el mismo tipo: hay que soltar las
            // constraints antes de retipar las columnas y volver a crearlas después.
            migrationBuilder.Sql(@"ALTER TABLE ""TicketDetalle"" DROP CONSTRAINT IF EXISTS ""FK_TicketDetalle_TicketCocina_IdTicket"";");
            migrationBuilder.Sql(@"ALTER TABLE ""TicketDetalle"" DROP CONSTRAINT IF EXISTS ""FK_TicketDetalle_PedidoDetalle_IdDetalle"";");
            migrationBuilder.Sql(@"ALTER TABLE ""TicketCocina"" DROP CONSTRAINT IF EXISTS ""FK_TicketCocina_Pedido_IdPedido"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoModificador"" DROP CONSTRAINT IF EXISTS ""FK_PedidoModificador_PedidoDetalle_IdDetalle"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoDetalle"" DROP CONSTRAINT IF EXISTS ""FK_PedidoDetalle_Pedido_IdPedido"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoDetalle"" DROP CONSTRAINT IF EXISTS ""FK_PedidoDetalle_PedidoAsiento_IdAsiento"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoAsiento"" DROP CONSTRAINT IF EXISTS ""FK_PedidoAsiento_Pedido_IdPedido"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PagosCuentaPorPagar"" DROP CONSTRAINT IF EXISTS ""FK_PagosCuentaPorPagar_MovimientoCaja_IdMovimientoCaja"";");
            migrationBuilder.Sql(@"ALTER TABLE ""EventoPedido"" DROP CONSTRAINT IF EXISTS ""FK_EventoPedido_Pedido_IdPedido"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Cuenta"" DROP CONSTRAINT IF EXISTS ""FK_Cuenta_Pedido_IdPedido"";");

            // Las columnas Id que eran IDENTITY deben perder esa propiedad antes de cambiar de tipo.
            migrationBuilder.Sql(@"ALTER TABLE ""TicketCocina"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoModificador"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoDetalle"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoAsiento"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;");
            migrationBuilder.Sql(@"ALTER TABLE ""Pedido"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;");
            migrationBuilder.Sql(@"ALTER TABLE ""Pago"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;");
            migrationBuilder.Sql(@"ALTER TABLE ""MovimientoCaja"" ALTER COLUMN ""Id"" DROP IDENTITY IF EXISTS;");

            // Con las tablas vacías, retipamos cada columna a uuid (USING con literal fijo:
            // nunca se ejecuta contra una fila real porque no quedan filas tras el TRUNCATE).
            migrationBuilder.Sql(@"ALTER TABLE ""TicketDetalle"" ALTER COLUMN ""IdTicket"" TYPE uuid USING ('00000000-0000-0000-0000-000000000000'::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""TicketDetalle"" ALTER COLUMN ""IdDetalle"" TYPE uuid USING ('00000000-0000-0000-0000-000000000000'::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""TicketCocina"" ALTER COLUMN ""IdPedido"" TYPE uuid USING ('00000000-0000-0000-0000-000000000000'::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""TicketCocina"" ALTER COLUMN ""Id"" TYPE uuid USING ('00000000-0000-0000-0000-000000000000'::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoModificador"" ALTER COLUMN ""IdDetalle"" TYPE uuid USING ('00000000-0000-0000-0000-000000000000'::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoModificador"" ALTER COLUMN ""Id"" TYPE uuid USING ('00000000-0000-0000-0000-000000000000'::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoDetalle"" ALTER COLUMN ""IdPedido"" TYPE uuid USING ('00000000-0000-0000-0000-000000000000'::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoDetalle"" ALTER COLUMN ""IdAsiento"" TYPE uuid USING (NULL::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoDetalle"" ALTER COLUMN ""Id"" TYPE uuid USING ('00000000-0000-0000-0000-000000000000'::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoAsiento"" ALTER COLUMN ""IdPedido"" TYPE uuid USING ('00000000-0000-0000-0000-000000000000'::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoAsiento"" ALTER COLUMN ""Id"" TYPE uuid USING ('00000000-0000-0000-0000-000000000000'::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""Pedido"" ALTER COLUMN ""Id"" TYPE uuid USING ('00000000-0000-0000-0000-000000000000'::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""PagosCuentaPorPagar"" ALTER COLUMN ""IdMovimientoCaja"" TYPE uuid USING (NULL::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""Pago"" ALTER COLUMN ""Id"" TYPE uuid USING ('00000000-0000-0000-0000-000000000000'::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""MovimientoCaja"" ALTER COLUMN ""Id"" TYPE uuid USING ('00000000-0000-0000-0000-000000000000'::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""EventoPedido"" ALTER COLUMN ""IdPedido"" TYPE uuid USING ('00000000-0000-0000-0000-000000000000'::uuid);");
            migrationBuilder.Sql(@"ALTER TABLE ""Cuenta"" ALTER COLUMN ""IdPedido"" TYPE uuid USING ('00000000-0000-0000-0000-000000000000'::uuid);");

            // Recrear las constraints de llave foránea ahora que ambos lados son uuid.
            migrationBuilder.Sql(@"ALTER TABLE ""TicketDetalle"" ADD CONSTRAINT ""FK_TicketDetalle_TicketCocina_IdTicket"" FOREIGN KEY (""IdTicket"") REFERENCES ""TicketCocina"" (""Id"") ON DELETE CASCADE;");
            migrationBuilder.Sql(@"ALTER TABLE ""TicketDetalle"" ADD CONSTRAINT ""FK_TicketDetalle_PedidoDetalle_IdDetalle"" FOREIGN KEY (""IdDetalle"") REFERENCES ""PedidoDetalle"" (""Id"") ON DELETE RESTRICT;");
            migrationBuilder.Sql(@"ALTER TABLE ""TicketCocina"" ADD CONSTRAINT ""FK_TicketCocina_Pedido_IdPedido"" FOREIGN KEY (""IdPedido"") REFERENCES ""Pedido"" (""Id"") ON DELETE CASCADE;");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoModificador"" ADD CONSTRAINT ""FK_PedidoModificador_PedidoDetalle_IdDetalle"" FOREIGN KEY (""IdDetalle"") REFERENCES ""PedidoDetalle"" (""Id"") ON DELETE CASCADE;");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoDetalle"" ADD CONSTRAINT ""FK_PedidoDetalle_Pedido_IdPedido"" FOREIGN KEY (""IdPedido"") REFERENCES ""Pedido"" (""Id"") ON DELETE CASCADE;");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoDetalle"" ADD CONSTRAINT ""FK_PedidoDetalle_PedidoAsiento_IdAsiento"" FOREIGN KEY (""IdAsiento"") REFERENCES ""PedidoAsiento"" (""Id"") ON DELETE SET NULL;");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoAsiento"" ADD CONSTRAINT ""FK_PedidoAsiento_Pedido_IdPedido"" FOREIGN KEY (""IdPedido"") REFERENCES ""Pedido"" (""Id"") ON DELETE CASCADE;");
            migrationBuilder.Sql(@"ALTER TABLE ""PagosCuentaPorPagar"" ADD CONSTRAINT ""FK_PagosCuentaPorPagar_MovimientoCaja_IdMovimientoCaja"" FOREIGN KEY (""IdMovimientoCaja"") REFERENCES ""MovimientoCaja"" (""Id"") ON DELETE SET NULL;");
            migrationBuilder.Sql(@"ALTER TABLE ""EventoPedido"" ADD CONSTRAINT ""FK_EventoPedido_Pedido_IdPedido"" FOREIGN KEY (""IdPedido"") REFERENCES ""Pedido"" (""Id"") ON DELETE CASCADE;");
            migrationBuilder.Sql(@"ALTER TABLE ""Cuenta"" ADD CONSTRAINT ""FK_Cuenta_Pedido_IdPedido"" FOREIGN KEY (""IdPedido"") REFERENCES ""Pedido"" (""Id"") ON DELETE CASCADE;");

            migrationBuilder.AddColumn<int>(
                name: "FolioDiario",
                table: "Pedido",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FoliadorSucursal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdSucursal = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    UltimoFolio = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoliadorSucursal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoliadorSucursal_Sucursal_IdSucursal",
                        column: x => x.IdSucursal,
                        principalTable: "Sucursal",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutboxEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AggregateType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AggregateId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PayloadJson = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    SyncStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    RetryCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxEvents", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoliadorSucursal_IdSucursal_Fecha",
                table: "FoliadorSucursal",
                columns: new[] { "IdSucursal", "Fecha" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_AggregateId",
                table: "OutboxEvents",
                column: "AggregateId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxEvents_SyncStatus_CreatedAt",
                table: "OutboxEvents",
                columns: new[] { "SyncStatus", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback destructivo (a propósito): esta migración es irreversible sin pérdida de
            // datos porque uuid->int no tiene mapeo significativo. Se descartan los registros
            // operativos igual que en Up() antes de retipar de regreso a int.
            migrationBuilder.Sql(@"TRUNCATE TABLE
                ""PagosCuentaPorPagar"",
                ""TicketDetalle"",
                ""TicketCocina"",
                ""EventoPedido"",
                ""DetalleCuenta"",
                ""DescuentoAplicado"",
                ""Pago"",
                ""Cuenta"",
                ""PedidoModificador"",
                ""PedidoDetalle"",
                ""PedidoAsiento"",
                ""MovimientoCaja"",
                ""Pedido""
                RESTART IDENTITY CASCADE;");

            migrationBuilder.Sql(@"ALTER TABLE ""TicketDetalle"" DROP CONSTRAINT IF EXISTS ""FK_TicketDetalle_TicketCocina_IdTicket"";");
            migrationBuilder.Sql(@"ALTER TABLE ""TicketDetalle"" DROP CONSTRAINT IF EXISTS ""FK_TicketDetalle_PedidoDetalle_IdDetalle"";");
            migrationBuilder.Sql(@"ALTER TABLE ""TicketCocina"" DROP CONSTRAINT IF EXISTS ""FK_TicketCocina_Pedido_IdPedido"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoModificador"" DROP CONSTRAINT IF EXISTS ""FK_PedidoModificador_PedidoDetalle_IdDetalle"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoDetalle"" DROP CONSTRAINT IF EXISTS ""FK_PedidoDetalle_Pedido_IdPedido"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoDetalle"" DROP CONSTRAINT IF EXISTS ""FK_PedidoDetalle_PedidoAsiento_IdAsiento"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoAsiento"" DROP CONSTRAINT IF EXISTS ""FK_PedidoAsiento_Pedido_IdPedido"";");
            migrationBuilder.Sql(@"ALTER TABLE ""PagosCuentaPorPagar"" DROP CONSTRAINT IF EXISTS ""FK_PagosCuentaPorPagar_MovimientoCaja_IdMovimientoCaja"";");
            migrationBuilder.Sql(@"ALTER TABLE ""EventoPedido"" DROP CONSTRAINT IF EXISTS ""FK_EventoPedido_Pedido_IdPedido"";");
            migrationBuilder.Sql(@"ALTER TABLE ""Cuenta"" DROP CONSTRAINT IF EXISTS ""FK_Cuenta_Pedido_IdPedido"";");

            migrationBuilder.DropTable(
                name: "FoliadorSucursal");

            migrationBuilder.DropTable(
                name: "OutboxEvents");

            migrationBuilder.DropColumn(
                name: "FolioDiario",
                table: "Pedido");

            migrationBuilder.AlterColumn<int>(
                name: "IdTicket",
                table: "TicketDetalle",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "IdDetalle",
                table: "TicketDetalle",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "IdPedido",
                table: "TicketCocina",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "TicketCocina",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "IdDetalle",
                table: "PedidoModificador",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PedidoModificador",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "IdPedido",
                table: "PedidoDetalle",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "IdAsiento",
                table: "PedidoDetalle",
                type: "integer",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PedidoDetalle",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "IdPedido",
                table: "PedidoAsiento",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "PedidoAsiento",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Pedido",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "IdMovimientoCaja",
                table: "PagosCuentaPorPagar",
                type: "integer",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Pago",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "MovimientoCaja",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<int>(
                name: "IdPedido",
                table: "EventoPedido",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<int>(
                name: "IdPedido",
                table: "Cuenta",
                type: "integer",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.Sql(@"ALTER TABLE ""TicketDetalle"" ADD CONSTRAINT ""FK_TicketDetalle_TicketCocina_IdTicket"" FOREIGN KEY (""IdTicket"") REFERENCES ""TicketCocina"" (""Id"") ON DELETE CASCADE;");
            migrationBuilder.Sql(@"ALTER TABLE ""TicketDetalle"" ADD CONSTRAINT ""FK_TicketDetalle_PedidoDetalle_IdDetalle"" FOREIGN KEY (""IdDetalle"") REFERENCES ""PedidoDetalle"" (""Id"") ON DELETE RESTRICT;");
            migrationBuilder.Sql(@"ALTER TABLE ""TicketCocina"" ADD CONSTRAINT ""FK_TicketCocina_Pedido_IdPedido"" FOREIGN KEY (""IdPedido"") REFERENCES ""Pedido"" (""Id"") ON DELETE CASCADE;");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoModificador"" ADD CONSTRAINT ""FK_PedidoModificador_PedidoDetalle_IdDetalle"" FOREIGN KEY (""IdDetalle"") REFERENCES ""PedidoDetalle"" (""Id"") ON DELETE CASCADE;");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoDetalle"" ADD CONSTRAINT ""FK_PedidoDetalle_Pedido_IdPedido"" FOREIGN KEY (""IdPedido"") REFERENCES ""Pedido"" (""Id"") ON DELETE CASCADE;");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoDetalle"" ADD CONSTRAINT ""FK_PedidoDetalle_PedidoAsiento_IdAsiento"" FOREIGN KEY (""IdAsiento"") REFERENCES ""PedidoAsiento"" (""Id"") ON DELETE SET NULL;");
            migrationBuilder.Sql(@"ALTER TABLE ""PedidoAsiento"" ADD CONSTRAINT ""FK_PedidoAsiento_Pedido_IdPedido"" FOREIGN KEY (""IdPedido"") REFERENCES ""Pedido"" (""Id"") ON DELETE CASCADE;");
            migrationBuilder.Sql(@"ALTER TABLE ""PagosCuentaPorPagar"" ADD CONSTRAINT ""FK_PagosCuentaPorPagar_MovimientoCaja_IdMovimientoCaja"" FOREIGN KEY (""IdMovimientoCaja"") REFERENCES ""MovimientoCaja"" (""Id"") ON DELETE SET NULL;");
            migrationBuilder.Sql(@"ALTER TABLE ""EventoPedido"" ADD CONSTRAINT ""FK_EventoPedido_Pedido_IdPedido"" FOREIGN KEY (""IdPedido"") REFERENCES ""Pedido"" (""Id"") ON DELETE CASCADE;");
            migrationBuilder.Sql(@"ALTER TABLE ""Cuenta"" ADD CONSTRAINT ""FK_Cuenta_Pedido_IdPedido"" FOREIGN KEY (""IdPedido"") REFERENCES ""Pedido"" (""Id"") ON DELETE CASCADE;");
        }
    }
}
