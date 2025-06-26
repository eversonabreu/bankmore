using BankMore.Core.Utils.Extensions;
using BankMore.CurrentAccount.Application.Commands;
using BankMore.CurrentAccount.Domain.Repositories;
using MediatR;

namespace BankMore.CurrentAccount.Application.Handlers;


public sealed class CreateCurrentAccountHandler(ICurrentAccountRepository repository) 
    : IRequestHandler<CreateCurrentAccountCommand, Domain.Entities.CurrentAccount>
{
    public async Task<Domain.Entities.CurrentAccount> Handle(CreateCurrentAccountCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.CurrentAccount
        {
            Number = await GetNewNumberAccountAsync(),
            Name = request.Name,
            IsActive = true,
            // criptografia simples, apenas para fins demonstrativos
            Password = request.Password.ComputeMd5Hash(),
            PersonDocumentNumber = request.PersonDocumentNumber
        };

        var currentAccount = await repository.CreateAsync(entity);
        await repository.SaveChangesAsync();
        return currentAccount;
    }

    private async Task<long> GetNewNumberAccountAsync()
    {
        Random random = new();
        long number;

        do
        {
            number = random.Next(10000, 1000000); 
        }
        while (await repository.SingleOrDefaultAsync(x => x.Number == number) is not null);

        return number;
    }
}