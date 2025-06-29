using BankMore.Core.Infrastructure.Entities;

namespace BankMore.Tariffing.Domain.Entities;

public sealed class Tariffing : Entity
{
    public Guid TransferId { get; set; }

    public decimal RateValue { get; set; }

    public DateTime DateTransaction { get; set; }
}