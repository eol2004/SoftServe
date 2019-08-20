using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recursion
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите количество элементов массива : ");
            int i = Convert.ToInt32(Console.ReadLine());
            int[] array = new int[i];
            Randomize(array);
            Print(array);
            Console.WriteLine();
            Console.WriteLine("Результирующий массив");
            Output(array,0);
            Console.ReadKey();
        }

        public static void Print(int[] array)
        {
            Console.WriteLine("Сгенерированный массив");
            for (int j = 0; j < array.Length; ++j)
            {
                Console.Write("{0} ", array[j]);
            }
        }

        static void Randomize(int[] array)
        {
            Random rnd = new Random();
            for (int j = 0; j < array.Length; ++j)
            {
                array[j] = rnd.Next(-100, 100);
            }
        }

        static void Output(int[] array, int j)
        {
            if (j <= array.Length - 1) //>        max(j)=array.Length-1
            {
                {
                    if (array[j] < 0 && j <= array.Length - 1) //if negativ & <= max valid index
                    {
                        Console.Write("{0} ", array[j]);
                        Output(array, ++j);
                    }
                    else
                    {
                        Output(array, ++j);
                    }
                }
            }
            else
            {
                for (int k = 0; k < array.Length; ++k)
                {
                    if (array[k] > 0) Console.Write("{0} ", array[k]);
                }
            }
        }

    }

}
