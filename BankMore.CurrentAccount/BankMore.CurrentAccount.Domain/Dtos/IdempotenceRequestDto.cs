namespace BankMore.CurrentAccount.Domain.Dtos;

public sealed record IdempotenceRequestDto<TValue>(string IdempotenceKey, TValue PayloadRequisition);