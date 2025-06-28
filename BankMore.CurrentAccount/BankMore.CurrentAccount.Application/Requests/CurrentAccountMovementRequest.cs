using BankMore.CurrentAccount.Application.Commands;

namespace BankMore.CurrentAccount.Application.Requests;

public sealed record CurrentAccountMovementRequest(string IdempotenceKey, CurrentAccountMovementCommand Command);