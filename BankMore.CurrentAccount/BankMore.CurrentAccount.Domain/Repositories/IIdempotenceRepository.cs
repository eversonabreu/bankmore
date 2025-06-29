﻿using BankMore.Core.Infrastructure.Database;

namespace BankMore.CurrentAccount.Domain.Repositories;

public interface IIdempotenceRepository : IDbRepository<Entities.Idempotence>
{
}