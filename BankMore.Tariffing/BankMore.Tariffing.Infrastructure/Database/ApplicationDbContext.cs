using BankMore.Core.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BankMore.Tariffing.Infrastructure.Database;

public sealed class ApplicationDbContext : DatabaseContext<ApplicationDbContext>
{
    public ApplicationDbContext() : base(new()) { }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Domain.Entities.Tariffing> Tariffings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Domain.Entities.Tariffing>(entity =>
        {
            entity.ToTable("tarifacao");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd().IsRequired();
            entity.Property(e => e.TransferId).HasColumnName("id_transferencia").IsRequired();
            entity.Property(e => e.RateValue).HasColumnName("valor_tarifacao").IsRequired();
            entity.Property(e => e.DateTransaction).HasColumnName("data_tarifacao").IsRequired();
        });
    }
}