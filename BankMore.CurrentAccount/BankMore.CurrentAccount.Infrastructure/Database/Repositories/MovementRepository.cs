using BankMore.Core.Infrastructure.Database;
using BankMore.CurrentAccount.Domain.Enums;
using BankMore.CurrentAccount.Domain.Repositories;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace BankMore.CurrentAccount.Infrastructure.Database.Repositories;

internal sealed class MovementRepository(ApplicationDbContext context, ICurrentAccountRepository currentAccountRepository)
    : DbRepository<Domain.Entities.Movement, ApplicationDbContext>(context), IMovementRepository
{
    public async Task<MovementOperationEnum> CreateMovementAsync(long numberAccount, MovementTypeEnum movementType, decimal value)
    {
        if (value <= 0m)
            return MovementOperationEnum.InvalidValue;

        var (currentAccount, movementOperation) = await currentAccountRepository
            .GetCurrentAccountByNumberAsync(numberAccount);

        if (movementOperation != null)
            return movementOperation.Value;

        await CreateAsync(new Domain.Entities.Movement
        {
            MovementDate = DateTime.UtcNow,
            MovementType = movementType,
            CurrentAccountId = currentAccount.Id,
            Value = value
        });

        await SaveChangesAsync();

        return MovementOperationEnum.Success;
    }

    public async Task<decimal> GetBalanceCurrentAcount(long numberAccount)
    {
        var sql = @"
            SELECT SUM(
                CASE 
                    WHEN tipo_movimentacao = 'C' THEN ABS(valor)
                    ELSE ABS(valor) * -1
                END
            ) 
            FROM movimentacao mov
            INNER JOIN conta_corrente cc ON mov.id_conta_corrente = cc.id
            WHERE cc.numero = @numberAccount AND cc.ativo = 1";

        var balance = await context.Database.GetDbConnection().QuerySingleOrDefaultAsync<decimal?>(
            sql,
            new { numberAccount  }) ?? 0m;

        return decimal.Round(balance, 2);
    }
}