using BankMore.Core.Utils.Extensions;

namespace BankMore.Core.Utils.Validators;

public static class PersonDocumentValidator
{
    private static bool IsSameNumberChar(string value, int count)
    {
        for (int index = 0; index <= 9; index++)
        {
            var compareString = new string(index.ToString().Single(), count);

            if (compareString == value)
            {
                return true;
            }
        }

        return false;
    }

    public static bool CpfValidate(this string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
        {
            return false;
        }

        string cpfOnlyNumbers = cpf.OnlyNumbers();

        if (string.IsNullOrEmpty(cpfOnlyNumbers) || cpfOnlyNumbers.Length != 11 || IsSameNumberChar(cpfOnlyNumbers, 11))
        {
            return false;
        }

        var numbers = new int[11];

        for (int index = 0; index < 11; index++)
        {
            numbers[index] = int.Parse(cpfOnlyNumbers[index].ToString());
        }

        int sum = 0;

        for (int index = 0; index < 9; index++)
        {
            sum += (10 - index) * numbers[index];
        }

        int result = sum % 11;

        if (result == 1 || result == 0)
        {
            if (numbers[9] != 0)
            {
                return false;
            }
        }
        else if (numbers[9] != 11 - result)
        {
            return false;
        }

        sum = 0;

        for (int i = 0; i < 10; i++)
        {
            sum += (11 - i) * numbers[i];
        }

        result = sum % 11;

        if (result == 1 || result == 0)
        {
            if (numbers[10] != 0)
            {
                return false;
            }
        }
        else if (numbers[10] != 11 - result)
        {
            return false;
        }

        return true;
    }
}