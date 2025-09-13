using System.Net;
using System.Text.Json;
using model;
using System.Text.Json.Serialization;

public class Server
{
    // Configuração do servidor HTTP
    private HttpListener listener;

    // Serviço de gerenciamento de cidadãos
    public CidadaoService cidadaoService = new CidadaoService();

    // Construtor
    public Server()
    {
        cidadaoService = new CidadaoService();
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
    }

    // Inicia o servidor
    public void Iniciar()
    {
        listener.Start();
        Console.WriteLine("Servidor iniciado em http://localhost:8080/");

        while (true)
        {
            var context = listener.GetContext();
            processamento_requisicao(context);
        }
    }

    // Processa as requisições HTTP
    // Lida com as rotas para servir a página HTML, pesquisar e cadastrar cidadãos
    // Responde com mensagens apropriadas embutidas na página HTML
    // As mensagens são inseridas em divs específicas na página HTML
    private void processamento_requisicao(HttpListenerContext context)
    {
        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;

        // Roteamento básico
        // Serve a página HTML na raiz
        // Processa buscas e cadastros via formulários
        if (request.Url.AbsolutePath == "/" && request.HttpMethod == "GET")
        {
            ServeFile(response, "index.html");
        }

        // Rota para pesquisar cidadão
        // Exemplo: /pesquisar?termo=nome_ou_cpf
        else if (request.Url.AbsolutePath == "/pesquisar" && request.HttpMethod == "GET")
        {
            string termo = request.QueryString["termo"];
            var cidadao = cidadaoService.PesquisarCidadao(termo);

            if (cidadao != null)
            {
                string htmlResponse = $"<p>Cidadão encontrado: {cidadao.nome}, CPF: {cidadao.cpf}</p>";
                string baseHtml = File.ReadAllText("index.html");
                string finalHtml = baseHtml.Replace("<div id=\"message-pesquisa\"></div>", $"<div id=\"message-pesquisa\">{htmlResponse}</div>");
                WriteResponse(response, finalHtml);
            }
            else
            {
                string htmlResponse = $"<p>Cidadão não encontrado! Verifique os dados.</p>";
                string baseHtml = File.ReadAllText("index.html");
                string finalHtml = baseHtml.Replace("<div id=\"message-pesquisa\"></div>", $"<div id=\"message-pesquisa\">{htmlResponse}</div>");
                WriteResponse(response, finalHtml);
            }
        }

        // Rota para cadastrar cidadão
        // Dados esperados no corpo da requisição como "nome=valor&cpf=valor
        else if (request.Url.AbsolutePath == "/cadastrar" && request.HttpMethod == "POST")
        {
            // Lê o corpo da requisição
            string body;
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                body = reader.ReadToEnd();
            }

            // Parseia os dados do formulário
            // Espera "nome=valor&cpf=valor"
            // Usa System.Web.HttpUtility para parsear query strings

            var parsed = System.Web.HttpUtility.ParseQueryString(body);
            string nome = parsed["nome"];
            string cpf = parsed["cpf"];
            int sucesso = cidadaoService.AdicionarCidadao(nome, cpf);

            // Responde com mensagem de sucesso ou erro embutida na página HTML
            // A mensagem é inserida na div com id "message-cadastro"
            if (sucesso == 1)
            {
                string htmlResponse = $"<p>Cidadão {nome} cadastrado com sucesso!</p>";
                string baseHtml = File.ReadAllText("index.html");
                string finalHtml = baseHtml.Replace("<div id=\"message-cadastro\"></div>", $"<div id=\"message-cadastro\">{htmlResponse}</div>");
                WriteResponse(response, finalHtml);
            }
            // Responde com mensagem de erro se o cadastro falhar (CPF inválido ou já existente)
            // A mensagem é inserida na div com id "message-cadastro"

            else if (sucesso == 2)
            {
                string htmlResponse = $"<p>Erro ao cadastrar cidadão. CPF já cadastrado.</p>";
                string baseHtml = File.ReadAllText("index.html");
                string finalHtml = baseHtml.Replace("<div id=\"message-cadastro\"></div>", $"<div id=\"message-cadastro\">{htmlResponse}</div>");
                WriteResponse(response, finalHtml);
            }
            else
            {
                string htmlResponse = $"<p>Erro ao cadastrar cidadão. Verifique o CPF.</p>";
                string baseHtml = File.ReadAllText("index.html");
                string finalHtml = baseHtml.Replace("<div id=\"message-cadastro\"></div>", $"<div id=\"message-cadastro\">{htmlResponse}</div>");
                WriteResponse(response, finalHtml);
            }
        }
        // Rota não encontrada
        // Responde com 404
        else
        {
            response.StatusCode = 404;
        }

        response.Close();
    }

    // Serve um arquivo estático (HTML)
    // Retorna 404 se o arquivo não for encontrado
    private void ServeFile(HttpListenerResponse response, string fileName)
    {
        // Verifica se o arquivo existe
        // Lê o conteúdo e escreve na resposta
        response.ContentType = "text/html; charset=UTF-8";

        if (File.Exists(fileName))
        {
            string content = File.ReadAllText(fileName);
            WriteResponse(response, content);
        }

        // Arquivo não encontrado
        // Responde com 404 e mensagem simples
        else
        {
            response.StatusCode = 404;
            WriteResponse(response, "<p>Arquivo não encontrado.</p>");
        }
    }

    // Escreve o conteúdo na resposta HTTP
    // Define o Content-Length e fecha o stream
    private void WriteResponse(HttpListenerResponse response, string content)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(content);
        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

}