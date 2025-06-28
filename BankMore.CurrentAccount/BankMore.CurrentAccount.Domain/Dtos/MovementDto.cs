using BankMore.CurrentAccount.Domain.Enums;

namespace BankMore.CurrentAccount.Domain.Dtos;

public record MovementDto(long NumberAccount, MovementTypeEnum MovementType, decimal Value);