namespace BankMore.CurrentAccount.Domain.Helpers;

public static class Constants
{
    public const string ApplicationDatabaseName = "CurrentAccount.db";

    public static class ApplicationMessages
    {
        public const string FailLoginUser = "USER_UNAUTHORIZED";
        public const string FailLoginUserWhenUserNotFound = "Credências inválidas. Usuário ou senha incorretos.";
        public const string FailLoginUserWhenManyPersonDocument = "Não foi possível autenticar, porque existe mais de uma conta-corrente registrada para o CPF informado. Por gentileza, faça login com o número da sua conta-corrente.";
        public const string FailLoginUserWhenUserInactive = "O seu acesso a conta-corrente está desabilitado.";
        public const string FailSavePersonDocument = "INVALID_DOCUMENT";
        public const string FailDeactivateCurrentAccount = "NOT_FOUND_CURRENT_ACCOUNT";
        public const string FailMovementValue = "INVALID_VALUE";
        public const string FailMovementType = "INVALID_TYPE";
        public const string IdempotenceKeyNullOrEmptyError = "IDEMPOTENCE_KEY_NULL_OR_EMPTY";
        public const string IdempotenceKeyNullOrEmptyErrorMessage = "A chave para idempotência não foi fornecida.";
        public const string CurrentAccountInvalid = "INVALID_ACCOUNT";
        public const string CurrentAccountInvalidMessage = "A conta-corrente não é válida.";
        public const string CurrentAccountInactive = "INACTIVE_ACCOUNT";
        public const string CurrentAccountInactiveMessage = "A conta-corrente está desativada.";
        public const string IdempotenceRequestBodyMismatchError = "IDEMPOTENCE_REQUEST_BODY_MISMATCH";
        public const string IdempotenceRequestBodyMismatchMessage = "Os dados enviados não estão de acordo com a chave de idempotência fornecida.";
        public const string MovementWaitingFinishProccess = "A solicitação de movimentação na conta-corrente já foi registrada, e o procesamento ainda está em andamento.";
        public const string FatalErrorMovementCurrentAccountError = "FATAL_ERROR_MOVEMENT_CURRENT_ACCOUNT";
        public const string FatalErrorMovementCurrentAccountErrorMessage = "Um erro inesperado ocorreu durante o processamento de movimentação na conta-corrente, e não foi possível processar a sua solicitação.";
    }
}