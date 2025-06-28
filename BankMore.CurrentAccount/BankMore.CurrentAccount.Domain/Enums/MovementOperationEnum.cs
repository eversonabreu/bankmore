namespace BankMore.CurrentAccount.Domain.Enums;

public enum MovementOperationEnum
{
    Success = 1,
    InvalidAccount = 2,
    InactiveAccount = 3,
    IdempotenceRequestBodyMismatch = 4,
    WaitingFinishProccess = 5,
    FatalErrorProccessing = 6,
    IdempotenceKeyNullOrEmpty = 7,
    InvalidType = 8,
    InvalidValue = 9
}