using BankMore.Core.Utils.Extensions;
using BankMore.CurrentAccount.Application.Commands;
using BankMore.CurrentAccount.Domain.Repositories;
using MediatR;

namespace BankMore.CurrentAccount.Application.Handlers;


public sealed class DeactivateCurrentAccountHandler(ICurrentAccountRepository repository) 
    : IRequestHandler<DeactivateCurrentAccountCommand, bool>
{
    public async Task<bool> Handle(DeactivateCurrentAccountCommand request, CancellationToken cancellationToken)
    {
        var currentAccount = await repository.SingleOrDefaultAsync(x => x.Number == request.NumberAccount && x.IsActive);

        if (currentAccount is null || currentAccount.Password != request.Password.ComputeMd5Hash())
        {
            return false;
        }

        // Deveria permitir inativar contas com saldo?
        // Na especificação não tem esse requisito, portanto não estou considerando se a conta tem saldo na inativação

        currentAccount.IsActive = false;
        await repository.UpdateAsync(currentAccount);
        await repository.SaveChangesAsync();
        return true;
    }
}