using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMore.CurrentAccount.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class CurrentAccountInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "conta_corrente",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    numero = table.Column<long>(type: "INTEGER", nullable: false),
                    nome_correntista = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    ativo = table.Column<int>(type: "INTEGER", nullable: false),
                    senha = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    cpf = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conta_corrente", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Idempotencies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    chave = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    requisicao = table.Column<string>(type: "text", nullable: false),
                    resultado = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Idempotencies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "movimentacao",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "TEXT", nullable: false),
                    id_conta_corrente = table.Column<Guid>(type: "TEXT", nullable: false),
                    data_movimentacao = table.Column<DateTime>(type: "TEXT", nullable: false),
                    tipo_movimentacao = table.Column<char>(type: "TEXT", maxLength: 1, nullable: false),
                    valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_movimentacao", x => x.id);
                    table.CheckConstraint("check_constraint_tipo_movimentacao_invalido", "tipo_movimentacao IN ('C','D')");
                    table.ForeignKey(
                        name: "FK_movimentacao_conta_corrente_id_conta_corrente",
                        column: x => x.id_conta_corrente,
                        principalTable: "conta_corrente",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_numero_conta_corrente",
                table: "conta_corrente",
                column: "numero",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_chave_idempotencia",
                table: "Idempotencies",
                column: "chave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movimentacao_id_conta_corrente",
                table: "movimentacao",
                column: "id_conta_corrente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Idempotencies");

            migrationBuilder.DropTable(
                name: "movimentacao");

            migrationBuilder.DropTable(
                name: "conta_corrente");
        }
    }
}
