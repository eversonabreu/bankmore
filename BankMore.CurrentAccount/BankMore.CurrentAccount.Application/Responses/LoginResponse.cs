using BankMore.CurrentAccount.Domain.Enums;

namespace BankMore.CurrentAccount.Application.Responses;

public sealed record LoginResponse(LoginStatusEnum LoginStatus, string JwtToken,
    long? NumberAccount, string PersonName);