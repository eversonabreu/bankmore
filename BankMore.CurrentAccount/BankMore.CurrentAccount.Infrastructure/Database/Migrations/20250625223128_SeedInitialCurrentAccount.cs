using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.CurrentAccount.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialCurrentAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "conta_corrente",
                columns: new[] { "id", "ativo", "nome_correntista", "numero", "senha", "cpf" },
                values: new object[] { new Guid("bec58d04-9be3-421a-bc1e-5ebf632b28f8"), 1, "Ana", 1234L, "81dc9bdb52d04dc20036dbd8313ed055", "05450395922" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "conta_corrente",
                keyColumn: "id",
                keyValue: new Guid("bec58d04-9be3-421a-bc1e-5ebf632b28f8"));
        }
    }
}
