using BankMore.CurrentAccount.Domain.Enums;
using MediatR;

namespace BankMore.CurrentAccount.Application.Commands;

public class CurrentAccountMovementCommand : IRequest<MovementOperationEnum>
{
    public long? NumberAccount { get; set; }

    public decimal Value { get; set; }

    public MovementTypeEnum MovementType { get; set; }
}