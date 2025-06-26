using BankMore.CurrentAccount.Domain.Enums;

namespace BankMore.CurrentAccount.Domain.Dtos;

public sealed record LoginResponseDto(LoginStatusEnum LoginStatus = LoginStatusEnum.FailWhenUserNotFound,
    long? NumberAccount = null, string PersonName = null);