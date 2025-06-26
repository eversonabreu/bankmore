using BankMore.CurrentAccount.Domain.Dtos;

namespace BankMore.CurrentAccount.Domain.Services.Contracts;

public interface IAuthApplicationService
{
    public Task<LoginResponseDto> LoginAsync(string code, string password);
}