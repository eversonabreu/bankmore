using BankMore.Core.Infrastructure.Entities;
using BankMore.CurrentAccount.Domain.Enums;
using System.Text.Json.Serialization;

namespace BankMore.CurrentAccount.Domain.Entities;

public sealed class Movement : Entity
{
    public Guid CurrentAccountId { get; set; }

    public DateTime MovementDate { get; set; }

    public MovementTypeEnum MovementType { get; set; }

    public decimal Value { get; set; }

    [JsonIgnore]
    public CurrentAccount CurrentAccount { get; set; }
}