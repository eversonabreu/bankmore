using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.CurrentAccount.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFieldIdempotencyResponse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "conta_corrente",
                keyColumn: "id",
                keyValue: new Guid("bec58d04-9be3-421a-bc1e-5ebf632b28f8"));

            migrationBuilder.AlterColumn<string>(
                name: "resultado",
                table: "Idempotencies",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.InsertData(
                table: "conta_corrente",
                columns: new[] { "id", "ativo", "nome_correntista", "numero", "senha", "cpf" },
                values: new object[] { new Guid("07809b83-5be5-41c3-ad65-c25cd004b438"), 1, "Ana", 1234L, "81DC9BDB52D04DC20036DBD8313ED055", "05450395922" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "conta_corrente",
                keyColumn: "id",
                keyValue: new Guid("07809b83-5be5-41c3-ad65-c25cd004b438"));

            migrationBuilder.AlterColumn<string>(
                name: "resultado",
                table: "Idempotencies",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "conta_corrente",
                columns: new[] { "id", "ativo", "nome_correntista", "numero", "senha", "cpf" },
                values: new object[] { new Guid("bec58d04-9be3-421a-bc1e-5ebf632b28f8"), 1, "Ana", 1234L, "81dc9bdb52d04dc20036dbd8313ed055", "05450395922" });
        }
    }
}
