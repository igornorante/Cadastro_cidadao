using System;

// Ponto de entrada da aplicação    
// Inicializa e inicia o servidor HTTP
class Program
{
    static void Main(string[] args)
    {
        var server = new Server();
        server.Iniciar();
    }
}