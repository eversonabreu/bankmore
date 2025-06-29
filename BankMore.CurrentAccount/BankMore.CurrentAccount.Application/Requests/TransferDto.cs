namespace BankMore.CurrentAccount.Application.Requests;

public record TransferDto(long NumberAccountOrigin, long NumberAccountDestination, decimal Value);