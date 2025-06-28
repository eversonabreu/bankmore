using BankMore.CurrentAccount.Domain.Enums;

namespace BankMore.CurrentAccount.Application.Requests;

public sealed class MovementRequest
{
    public long? NumberAccount { get; set; }

    public decimal Value { get; set; }

    public char MovementType { get; set; }
}