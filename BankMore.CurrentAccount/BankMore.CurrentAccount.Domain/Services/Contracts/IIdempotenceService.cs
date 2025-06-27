using BankMore.CurrentAccount.Domain.Entities;

namespace BankMore.CurrentAccount.Domain.Services.Contracts;

public interface IIdempotenceService
{
    public Task<Idempotence> GetCompletedAsync(string key);
}