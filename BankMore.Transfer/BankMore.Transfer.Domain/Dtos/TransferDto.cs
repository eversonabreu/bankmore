namespace BankMore.Transfer.Domain.Dtos;

public record TransferDto(Guid CurrentAccountOriginId, Guid CurrentAccountDestinationId, decimal TransferValue);