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

        // Método que será executado de forma Assíncrona 
        private async void BtnProcessar_Click(object sender, RoutedEventArgs e)
        {
            var contas = r_Repositorio.GetContaClientes();

            BtnProcessar.IsEnabled = false;

            AtualizarView(new List<string>(), TimeSpan.Zero);

            var inicio = DateTime.Now;

            // A Thread principal só irá seguir o seu processo se a tarefa de consolidar as contas for concluída  
            // Usando await além de ter essa espera, o resultado da task pode ser atrubuida a uma variavel, ou seja o resultado da tarefa
            var resultado = await ConsolidarContas(contas);

            // Após a task for concluída o fluxo irá seguir normalmente 
            var fim = DateTime.Now;

            AtualizarView(resultado, fim - inicio);

            BtnProcessar.IsEnabled = true; 
        }

        // A tarefa de consolidar contas tambem é executada de forma assíncrona e retorna uma tarefa que retorna um array de strings 
        public async Task<string[]> ConsolidarContas(IEnumerable<ContaCliente> contas)
        {
            var resultado = new List<string>();

            // Para cada conta da lista, eu crio uma task e já a inicio, cada task irá consolidar uma conta dessa lista e retornar uma String (conta consolidada)
            // a task criada será armazenada em um array de tasks
            var tasks = contas.Select(conta =>
                Task.Factory.StartNew(() => r_Servico.ConsolidarMovimentacao(conta)));

            // com await => segue o fluxo desse método até que todas as tasks do array terminem o seu processo 
            // (Como é um array de tarefas, o resultado final será tmb um array de resultados (Strings que representa uma conta consolidada)
            return await Task.WhenAll(tasks); // Retorna uma tarefa que devolve um array de contas consolidadas
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
