using BankMore.Core.Infrastructure.Entities;

namespace BankMore.Transfer.Domain.Entities;

public sealed class Transfer : Entity
{
    public Guid CurrentAccountOriginId { get; set; }

    public Guid CurrentAccountDestinationId { get; set; }

    public DateTime TransferDate { get; set; }

    public decimal TransferValue { get; set; }
}