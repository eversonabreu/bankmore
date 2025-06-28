using BankMore.Core.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Transfer.Infrastructure.Database;

public sealed class ApplicationDbContext : DatabaseContext<ApplicationDbContext>
{
    public ApplicationDbContext() : base(new()) { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Domain.Entities.Transfer> Transfers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.Transfer>(entity =>
        {
            entity.ToTable("transferencia");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd().IsRequired();
            entity.Property(e => e.CurrentAccountOriginId).HasColumnName("id_conta_corrente_origem").IsRequired();
            entity.Property(e => e.CurrentAccountDestinationId).HasColumnName("id_conta_corrente_destino").IsRequired();
            entity.Property(e => e.TransferDate).HasColumnName("data_transferencia").IsRequired();
            entity.Property(e => e.TransferValue).HasColumnName("valor_transferencia").IsRequired();
        });
    }
}