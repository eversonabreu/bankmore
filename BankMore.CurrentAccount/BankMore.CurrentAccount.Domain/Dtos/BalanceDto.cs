namespace BankMore.CurrentAccount.Domain.Dtos;

public sealed record BalanceDto(long NumberAccount, string PersonName, DateTime BalanceDate, decimal BalanceValue);