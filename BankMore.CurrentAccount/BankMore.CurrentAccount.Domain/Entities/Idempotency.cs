using BankMore.Core.Infrastructure.Entities;

namespace BankMore.CurrentAccount.Domain.Entities;

public class Idempotency : Entity
{
    public string Key { get; set; }

    public string PayloadRequisition { get; set; }

    public string PayloadResponse { get; set; }
}