using BankMore.CurrentAccount.Domain.Enums;

namespace BankMore.CurrentAccount.Application.Requests;

public sealed class MovementRequest
{
    public long? NumberAccount { get; set; }

    public decimal Value { get; set; }

    public string MovementType { get; set; }
}