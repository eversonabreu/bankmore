using BankMore.Transfer.Domain.Dtos;
using BankMore.Transfer.Domain.Enums;

namespace BankMore.Transfer.Domain.Services.Contracts;

public interface ITransferService
{
    public Task<MovementOperationEnum> CreateTransferAsync(TransferDto transferDto);
}