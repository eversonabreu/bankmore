using BankMore.CurrentAccount.Domain.Entities;

namespace BankMore.CurrentAccount.Domain.Services.Contracts;

public interface IIdempotencyService
{
    public Task<Idempotency> GetCompletedAsync(string key);
}