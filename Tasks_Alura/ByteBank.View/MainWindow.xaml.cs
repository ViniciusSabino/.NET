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

        private void BtnProcessar_Click(object sender, RoutedEventArgs e)
        {
            var contas = r_Repositorio.GetContaClientes();

            // Irá retornar o gerenciador de tarefas que está atuando no momento, no caso na thread principal
            var TaskScheduler_UI = TaskScheduler.FromCurrentSynchronizationContext();

            BtnProcessar.IsEnabled = false;

            AtualizarView(new List<string>(), TimeSpan.Zero);

            var inicio = DateTime.Now;

            // Quando a tarefa que consilida todas as contas for concluida novas tarefas serão criadas para seguir o processo corretamente
            ConsolidarContas(contas)
                .ContinueWith(task =>               // Tarefa 1 => (Após Consolidar as Contas) 
                {  
                    var fim = DateTime.Now;

                    // Irá recuperar o resultado da tarefa que originou essa nova tarefa atual, 
                    // no caso o retorno da função ConsolidarContas(contas)
                    var resultado = task.Result; 

                    // E tmb irá atualizar a view mostrando o desempenho obtido
                    AtualizarView(resultado, fim - inicio);

                }, TaskScheduler_UI) // Especificando que as tarefas precisam ser executas no gerenciador de tasks da Thread Principal

                .ContinueWith(task =>           // Tarefa 2 => (Após o conclusão da Tarefa 1) 
                {
                    BtnProcessar.IsEnabled = true; 

                }, TaskScheduler_UI); 

            // Task.WaitAll(contasTarefas); => Não segue o processo da Thread atual até que todas as tasks do array de tasks sejam concluidos

        }

        // Retornara uma Tarefa que irá retornar as contas consolidadas
        private Task<List<string>> ConsolidarContas(IEnumerable<ContaCliente> contas) {

            var resultado = new List<string>();

            // Realizando o mapeamento para que cada conta seja consolidada por uma task, a task será armazenada no array
            var tasks = contas.Select(conta =>
            {
                // Para cada conta uma Task será criada e iniciada
                 return Task.Factory.StartNew(() =>
                {
                    // Realizará a consolidação dessa conta e adicionara o resultado na lista de contas
                    var resultadoConta = r_Servico.ConsolidarMovimentacao(conta);
                    resultado.Add(resultadoConta);
                });
            });

            // Quando todas as tarefas do array de tasks anterior forem terminadas 
            // uma nova task sera executada, que apenas irá retornar o resultado da consolidação
            // de todas as contas
            
            // Task.WhenAll() => retornará uma outra tarefa que só tem a função de esperar as tarefas que são passadas por parametro terminarem (array)
            return Task.WhenAll(tasks).ContinueWith(task =>
            {
                // O retorno da tarefa será o resultado da consolidação
                return resultado;
            });

            // ContinueWith => Irá encadear outra tarefa após a execução da anterior, ou seja no exemplo acima
            // só será executada quando a tarefa que espera as tasks do array forem terminadas 

            // task => task que originou a tarefa atual 
                       
        } 

        private void AtualizarView(List<String> result, TimeSpan elapsedTime)
        {
            var tempoDecorrido = $"{ elapsedTime.Seconds }.{ elapsedTime.Milliseconds} segundos!";
            var mensagem = $"Processamento de {result.Count} clientes em {tempoDecorrido}";

            LstResultados.ItemsSource = result;
            TxtTempo.Text = mensagem;
        }
    }
}
