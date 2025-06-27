using BankMore.Core.Utils.Extensions;
using BankMore.Core.Utils.Validators;
using BankMore.CurrentAccount.Domain.Dtos;
using BankMore.CurrentAccount.Domain.Enums;
using BankMore.CurrentAccount.Domain.Repositories;
using BankMore.CurrentAccount.Domain.Services.Contracts;

namespace BankMore.CurrentAccount.Domain.Services.Implementations;

internal sealed class AuthApplicationService(ICurrentAccountRepository repository) : IAuthApplicationService
{
    public async Task<LoginResponseDto> LoginAsync(string code, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return Fail();

        var hashedPassword = password.ComputeMd5Hash();

        return code.CpfValidate()
            ? await LoginByCpfAsync(code.OnlyNumbers(), hashedPassword)
            : await LoginByNumberAsync(code, hashedPassword);
    }

    private async Task<LoginResponseDto> LoginByCpfAsync(string cpf, string hashedPassword)
    {
        var accounts = await repository.GetAsync(x => x.PersonDocumentNumber == cpf);

        return accounts.Count switch
        {
            0 => Fail(),
            > 1 => Fail(LoginStatusEnum.FailWhenManyPersonDocument),
            _ => ValidatePassword(accounts.Single(), hashedPassword)
        };
    }

    private async Task<LoginResponseDto> LoginByNumberAsync(string code, string hashedPassword)
    {
        if (!long.TryParse(code, out var number))
            return Fail();

        var account = await repository.SingleOrDefaultAsync(x => x.Number == number);
        return account is null ? Fail() : ValidatePassword(account, hashedPassword);
    }

    private static LoginResponseDto ValidatePassword(Entities.CurrentAccount account, string hashedPassword)
    {
        if (account.Password != hashedPassword)
            return Fail();

        if (!account.IsActive)
            return Fail(LoginStatusEnum.FailWhenUserInactive);

        return new LoginResponseDto(LoginStatusEnum.Success, account.Number, account.Name);
    }

    private static LoginResponseDto Fail(LoginStatusEnum status = LoginStatusEnum.FailWhenUserNotFound)
        => new(status);
}