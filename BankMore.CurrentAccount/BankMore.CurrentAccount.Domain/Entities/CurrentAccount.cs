using BankMore.Core.Infrastructure.Entities;
using System.Text.Json.Serialization;

namespace BankMore.CurrentAccount.Domain.Entities;

public sealed class CurrentAccount : Entity
{
    public long Number { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    [JsonIgnore]
    public string Password { get; set; }

    public string PersonDocumentNumber { get; set; }

    [JsonIgnore]
    public ICollection<Movement> Movements { get; set; } = [];
}