using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.Transfer.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "transferencia",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    id_conta_corrente_origem = table.Column<Guid>(type: "TEXT", nullable: false),
                    id_conta_corrente_destino = table.Column<Guid>(type: "TEXT", nullable: false),
                    data_transferencia = table.Column<DateTime>(type: "TEXT", nullable: false),
                    valor_transferencia = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transferencia", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transferencia");
        }
    }
}
