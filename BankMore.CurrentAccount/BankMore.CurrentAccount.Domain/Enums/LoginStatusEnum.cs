namespace BankMore.CurrentAccount.Domain.Enums;

public enum LoginStatusEnum
{
    Success = 1,
    FailWhenUserNotFound = 2,
    FailWhenManyPersonDocument = 3
}