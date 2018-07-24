using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQ_LAMBDA
{
    public class Program
    {
        public static void Main2(string[] args)
        {

           Console.WriteLine("Hello World!");

            int[] lista = { 1, 2, 5, 60, 20, 50, 10, 40 };

            List<Pessoa> listaPessoas = new List<Pessoa>
            {
                new Pessoa { idade = 22, nome = "Teste 1", sexo = 'M', id = 1 },
                new Pessoa { idade = 10, nome = "João", sexo = 'M', id = 2 },
                new Pessoa { idade = 57, nome = "Maria", sexo = 'F', id = 3 },
                new Pessoa { idade = 33, nome = "Pedro", sexo = 'M', id = 4 },
                new Pessoa { idade = 41, nome = "Renata", sexo = 'F', id = 5 },
                new Pessoa { idade = 7, nome = "Camila", sexo = 'F', id = 6 }
            };

            List<Livro> listaLivros = new List<Livro>
            {
                new Livro { pessoa_id = 2, nome = "Livro 1" },
                new Livro { pessoa_id = 5, nome = "Livro 2" },
                new Livro { pessoa_id = 3, nome = "Livro 3" },
                new Livro { pessoa_id = 2, nome = "Livro 4" },
                new Livro { pessoa_id = 1, nome = "Livro 5" },
                new Livro { pessoa_id = 6, nome = "Livro 6" },
                new Livro { pessoa_id = 3, nome = "Livro 7" }
            };


            var listaFiltroMaior20 = listaPessoas.Where(p => p.idade > 20); // Resultado é um IEnumerable<Pessoas> => pessoas com idade maior do que 20;

            var listaFiltroMaior20Nome = listaPessoas.Where(p => p.idade > 20).Select(p => p.sexo);
            // Resultado é um IEnumerable<char> => coleção contendo apenas o sexo das pessoas com idade
            // maior do que 20

            var listaOrdenada = listaPessoas.Where(p => p.idade > 20).OrderByDescending(p => p.idade); // Ordem Decrescente

            // OrderBy => Ordem crescente 

            var listaAltenativa = from a in listaPessoas where a.idade > 10 orderby a.idade descending select a.nome;
            // from a in listaPessoas => para cada a (Pessoa) da lista de Pessoas 
            // where a.idade > 10 => Filtre pelas pessoas com idade maior do que 10
            // order by a.idade descending => Ordene pela idade em order decrescente 
            // select a.nome => o resultado precisa ser somente um IEnumerable<string> com o nome da      pessoa

            // Para Cada Livro será exibido uma pessoa 
            var listaJoin = from livros in listaLivros
                            join pessoas 
                                in listaPessoas 
                                    on livros.pessoa_id equals pessoas.id
                            select new { livros, pessoas }; 
            
            foreach(var item in listaJoin)
            {
                Console.WriteLine("Livro: " + item.livros.nome + "--" + "Pessoa " + item.pessoas.nome);
            }

        }

        public class Pessoa
        {
            public string nome;
            public int idade;
            public char sexo;
            public string CPF;
            public int id;
        }

        public class Livro
        {
            public int id;
            public int pessoa_id;
            public string nome;
        }
    }
}
