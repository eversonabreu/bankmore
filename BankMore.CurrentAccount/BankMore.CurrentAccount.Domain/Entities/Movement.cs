using BankMore.Core.Infrastructure.Entities;
using BankMore.CurrentAccount.Domain.Enums;

namespace BankMore.CurrentAccount.Domain.Entities;

public sealed class Movement : Entity
{
    public Guid CurrentAccountId { get; set; }

    public DateTime MovementDate { get; set; }

    public MovementTypeEnum MovementType { get; set; }

    public decimal Value { get; set; }

    public CurrentAccount CurrentAccount { get; set; }
}