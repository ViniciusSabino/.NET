using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LINQ_LAMBDA
{
    class Group
    {
        public static void Main(string[] args)
        {

            int[] listaNum = { 1, 1, 1, 1, 4, 4, 2, 2, 5, 5, 10, 9, 8 };

            // GROUP - DISTINCT 

            // Retornando uma nova lista, mas sem números repetidos
            var listaDistinct = listaNum.Distinct().Select(a => a).OrderBy(a => a);

            var listaGroup = listaNum.GroupBy(a => a).Select(a => a);

            foreach (var item in listaDistinct)
                Console.WriteLine(item);

        }
    }
}
