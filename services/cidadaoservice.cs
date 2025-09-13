using System;
using model;

// Serviço para gerenciar cidadãos
// Permite adicionar e pesquisar cidadãos
// Utiliza a classe Validacao para validar CPFs
public class CidadaoService
{
    private List<Cidadao> lista_cidadaos = new List<Cidadao>();
    private Validacao validacao = new Validacao();

    // Adiciona um novo cidadão à lista
    // Retorna true se adicionado com sucesso, false se o CPF for inválido

    public int AdicionarCidadao(string nome, string cpf)
    {
        // Valida CPF e verifica se já existe
        if (validacao.validarCPF(cpf) && PesquisarCidadao(cpf) == null)
        {
            Cidadao novoCidadao = new Cidadao();
            novoCidadao.nome = nome;
            novoCidadao.cpf = cpf;
            lista_cidadaos.Add(novoCidadao);

            return 1;
        }
        else if (PesquisarCidadao(cpf) != null)
        {
            return 2;

        }
        else
        {
            return 0;
        }
    }

    // Pesquisa cidadão por nome ou CPF
    // Retorna o cidadão encontrado ou null se não encontrado

    public Cidadao PesquisarCidadao(string termo)
    {
        // Busca exata por CPF ou busca parcial por nome (case insensitive)
        var cidadao_encontrado = lista_cidadaos.Find(c => c.cpf == termo || c.nome.Contains(termo, StringComparison.OrdinalIgnoreCase));

        if (cidadao_encontrado != null)
        {
            return cidadao_encontrado;
        }
        else
        {
            return null;
        }
    }
}