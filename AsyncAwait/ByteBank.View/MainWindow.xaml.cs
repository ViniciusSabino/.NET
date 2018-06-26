using ByteBank.Core.Model;
using ByteBank.Core.Repository;
using ByteBank.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ByteBank.View
{
    public partial class MainWindow : Window
    {
        private readonly ContaClienteRepository r_Repositorio;
        private readonly ContaClienteService r_Servico;

        public MainWindow()
        {
            InitializeComponent();

            r_Repositorio = new ContaClienteRepository();
            r_Servico = new ContaClienteService();
        }

        // Método Assíncrono 
        private async void BtnProcessar_Click(object sender, RoutedEventArgs e)
        {
            var contas = r_Repositorio.GetContaClientes();

            BtnProcessar.IsEnabled = false;

            AtualizarView(new List<string>(), TimeSpan.Zero);

            var inicio = DateTime.Now;

            // A Thread principal só irá seguir o seu processo se a tarefa de consolidar as contas terminar 
            // Usando await além de ter essa espera, o resultado da task pode ser atrubuido a uma variavel (resultado)
            var resultado = await ConsolidarContas(contas);

            // Após a task for concluida segue o fluxo normalmente
            var fim = DateTime.Now;

            AtualizarView(resultado, fim - inicio);

            BtnProcessar.IsEnabled = true; 
        }

        // Tarefa de consolidar contas tambem é executada de forma assíncrona 
        public async Task<string[]> ConsolidarContas(IEnumerable<ContaCliente> contas)
        {
            var resultado = new List<string>();

            // Para cada conta da lista, eu crio uma task e já inicio, cada task irá consolidar uma conta da lista 
            // e retornar uma String (conta consolidada)
            // a task criada será armazenada nesse array de tasks
            var tasks = contas.Select(conta =>
                Task.Factory.StartNew(() => r_Servico.ConsolidarMovimentacao(conta)));

            // com await => Quando todas as tasks terminarem eu retorno o resultado de todas as tarefas
            // (Como é um array de tarefas, o resultado final será tmb um array de resultados (String que representa uma conta consolidada)
            return await Task.WhenAll(tasks);
        }

        private void AtualizarView(IEnumerable<string> result, TimeSpan elapsedTime)
        {
            var tempoDecorrido = $"{ elapsedTime.Seconds }.{ elapsedTime.Milliseconds} segundos!";
            var mensagem = $"Processamento de {result.Count()} clientes em {tempoDecorrido}";

            LstResultados.ItemsSource = result;
            TxtTempo.Text = mensagem;
        }
    }

    // ContinueWith => Irá encadear outra tarefa após a execução da anterior, ou seja no exemplo acima
    // só será executada quando a tarefa que espera as tasks do array forem terminadas 

    // task => task que originou a tarefa atual 
}
