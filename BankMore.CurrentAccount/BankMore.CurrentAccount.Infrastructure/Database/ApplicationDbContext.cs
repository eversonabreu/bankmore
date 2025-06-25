using BankMore.Core.Infrastructure.Database;
using BankMore.CurrentAccount.Domain.Entities;
using BankMore.CurrentAccount.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BankMore.CurrentAccount.Infrastructure.Database;

public sealed class ApplicationDbContext : DatabaseContext<ApplicationDbContext>
{
    public ApplicationDbContext(): base(new()) { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) { }

    public DbSet<Domain.Entities.CurrentAccount> CurrentAccounts { get; set; }

    public DbSet<Movement> Movements { get; set; }

    public DbSet<Idempotency> Idempotencies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.CurrentAccount>(entity =>
        {
            entity.ToTable("conta_corrente");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd().IsRequired();
            entity.Property(e => e.Number).HasColumnName("numero").IsRequired();
            entity.HasIndex(e => e.Number).IsUnique().HasDatabaseName("idx_numero_conta_corrente");
            entity.Property(e => e.Name).HasColumnName("nome_correntista").HasMaxLength(150).IsRequired();
            entity.Property(e => e.IsActive).HasColumnName("ativo").HasConversion<int>().IsRequired();
            entity.Property(e => e.Password).HasColumnName("senha").HasMaxLength(255).IsRequired();
            entity.Property(e => e.PersonDocumentNumber).HasColumnName("cpf").HasMaxLength(11).IsRequired();
        });

        modelBuilder.Entity<Movement>(entity =>
        {
            entity.ToTable("movimentacao");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd().IsRequired();
            entity.Property(e => e.CurrentAccountId).HasColumnName("id_conta_corrente").IsRequired();

            entity.HasOne(e => e.CurrentAccount)
                .WithMany(a => a.Movements)
                .HasForeignKey(e => e.CurrentAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.MovementDate).HasColumnName("data_movimentacao").IsRequired();
            entity.Property(e => e.MovementType)
                    .HasColumnName("tipo_movimentacao")
                    .HasConversion(
                        v => (char)v,              
                        v => (MovementTypeEnum)v     
                    )
                    .HasMaxLength(1)
                    .IsRequired();

            entity.Property(e => e.Value)
            .HasColumnName("valor")
            .HasColumnType("decimal(18,2)")
            .HasConversion(v => Math.Round(v, 2), v => v)
            .IsRequired();

            entity.ToTable(tb => tb.HasCheckConstraint("check_constraint_tipo_movimentacao_invalido", "tipo_movimentacao IN ('C','D')"));
        });

        modelBuilder.Entity<Idempotency>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd().IsRequired();
            entity.Property(e => e.Key).HasColumnName("chave").HasMaxLength(100).IsRequired();
            entity.HasIndex(e => e.Key).IsUnique().HasDatabaseName("idx_chave_idempotencia");
            entity.Property(e => e.PayloadRequisition).HasColumnName("requisicao").HasColumnType("text").IsRequired();
            entity.Property(e => e.PayloadResponse).HasColumnName("resultado").HasColumnType("text").IsRequired();
        });

        SeedInitialData(modelBuilder);
    }

    // Este método foi necessário para criar um registro inicial no banco de dados de conta corrente
    // A senha está com criptografia simples em MD5 (o valor é "1234"). Obviamente que num cenário real, ela deve ser melhor protegida
    private static void SeedInitialData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Domain.Entities.CurrentAccount>().HasData(new Domain.Entities.CurrentAccount
        {
            Number = 1234,
            Name = "Ana",
            IsActive = true,
            Password = "81dc9bdb52d04dc20036dbd8313ed055",
            PersonDocumentNumber = "05450395922"
        });
    }
}