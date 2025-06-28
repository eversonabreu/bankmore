namespace BankMore.CurrentAccount.Application.Requests;

public sealed record CurrentAccountMovementRequest(string IdempotenceKey, MovementRequest Command);