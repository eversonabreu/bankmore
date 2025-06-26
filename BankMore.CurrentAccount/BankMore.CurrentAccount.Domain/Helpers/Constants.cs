namespace BankMore.CurrentAccount.Domain.Helpers;

public static class Constants
{
    public const string ApplicationDatabaseName = "CurrentAccount.db";

    public static class ApplicationErrors
    {
        public const string FailLoginUser = "USER_UNAUTHORIZED";
        public const string FailLoginUserWhenUserNotFound = "Credências inválidas. Usuário ou senha incorretos.";
        public const string FailLoginUserWhenManyPersonDocument = "Não foi possível autenticar, porque existe mais de uma conta-corrente registrada para o CPF informado. Por gentileza, faça login com o número da sua conta-corrente.";
    }
}