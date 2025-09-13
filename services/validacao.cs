public class Validacao
{
    public bool validarCPF(string cpf)
    {
        // Remove caracteres não numéricos
        cpf = cpf.Replace(".", "").Replace("-", "");

        // Verifica se o CPF tem 11 dígitos
        if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
            return false;

        // Verifica se todos os dígitos são iguais
        if (new string(cpf[0], cpf.Length) == cpf)
            return false;

        // Validação do primeiro dígito verificador
        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += (cpf[i] - '0') * (10 - i);

        int digito1 = soma % 11;
        digito1 = digito1 < 2 ? 0 : 11 - digito1;
        if (digito1 != (cpf[9] - '0'))
            return false;

        // Validação do segundo dígito verificador
        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += (cpf[i] - '0') * (11 - i);

        int digito2 = soma % 11;
        digito2 = digito2 < 2 ? 0 : 11 - digito2;
        if (digito2 != (cpf[10] - '0'))
            return false;

        return true;
    }
}