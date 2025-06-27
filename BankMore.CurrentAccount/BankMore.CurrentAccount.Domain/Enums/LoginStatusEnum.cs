namespace BankMore.CurrentAccount.Domain.Enums;

public enum LoginStatusEnum
{
    Success = 1,
    FailWhenUserNotFound = 2,
    FailWhenUserInactive = 3,
    FailWhenManyPersonDocument = 4
}