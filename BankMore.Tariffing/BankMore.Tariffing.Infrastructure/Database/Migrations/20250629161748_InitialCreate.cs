using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.Tariffing.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tarifacao",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    id_transferencia = table.Column<Guid>(type: "TEXT", nullable: false),
                    valor_tarifacao = table.Column<decimal>(type: "TEXT", nullable: false),
                    data_tarifacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tarifacao", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tarifacao");
        }
    }
}
