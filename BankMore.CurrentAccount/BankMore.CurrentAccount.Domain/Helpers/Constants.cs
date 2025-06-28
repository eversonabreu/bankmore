namespace BankMore.CurrentAccount.Domain.Helpers;

public static class Constants
{
    public const string ApplicationDatabaseName = "CurrentAccount.db";

    public static class ApplicationErrors
    {
        public const string FailLoginUser = "USER_UNAUTHORIZED";
        public const string FailLoginUserWhenUserNotFound = "Credências inválidas. Usuário ou senha incorretos.";
        public const string FailLoginUserWhenManyPersonDocument = "Não foi possível autenticar, porque existe mais de uma conta-corrente registrada para o CPF informado. Por gentileza, faça login com o número da sua conta-corrente.";
        public const string FailLoginUserWhenUserInactive = "O seu acesso a conta-corrente está desabilitado.";
        public const string FailSavePersonDocument = "INVALID_DOCUMENT";
        public const string FailDeactivateCurrentAccount = "NOT_FOUND_CURRENT_ACCOUNT";
        public const string FailMovementValue = "INVALID_VALUE";
        public const string FailMovementType = "INVALID_TYPE";
        public const string IdempotenceKeyNullOrEmpty = "IDEMPOTENCE_KEY_NULL_OR_EMPTY";
        public const string IdempotenceKeyNullOrEmptyMessage = "A chave para idempotência não foi fornecida";
        public const string CurrentAccountInvalid = "INVALID_ACCOUNT";
        public const string CurrentAccountInvalidMessage = "A conta-corrente não é válido ou está desativada";
    }
}