using BankMore.Core.Infrastructure.Entities;

namespace BankMore.CurrentAccount.Domain.Entities;

public sealed class CurrentAccount : Entity
{
    public long Number { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public string Password { get; set; }

    public string PersonDocumentNumber { get; set; }

    public ICollection<Movement> Movements { get; set; } = [];
}