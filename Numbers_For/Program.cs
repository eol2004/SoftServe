using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Numbers_For
{
    class Program
    {
        static void Main(string[] args)
        {
            int n = 0;
            int i = 0;
            int j = 0;
            int buf = 0;

            Console.Write("Введите количество чисел в массиве : ");
            n = Convert.ToUInt16(Console.ReadLine());
            int[] numbers = new int[n];
            int[] poradk = new int[n]; //массив порядковых номеров
            
            for (i = 0; i < n; i++)
            {
                Console.Write($"Введите {i+1} число : ");
                numbers[i] = Convert.ToUInt16(Console.ReadLine());
                poradk[i] = i;
            }

            Console.Write("Введенный массив : ");
            for (i = 0; i < n; i++)
            {
               Console.Write($"{numbers[i]} ");
            }
            Console.WriteLine();

            for (i = 0; i < n - 1 ; i++) // сортируем пузырьком по возрастанию, 
            {
                for (j = 0; j < n - i - 1 ; j++)
                {
                    if (numbers[j] > numbers[j + 1])
                        {
                            buf = numbers[j];
                            numbers[j] = numbers[j + 1];
                            numbers[j + 1] = buf;
                            buf = poradk[j];
                            poradk[j] = poradk[j + 1];
                            poradk[j + 1] = buf;
                        }
                }
            }

            Console.Write("Отсортированный массив : ");
            for (i = 0; i < n; i++)
            {
                Console.Write($"{numbers[i]} ");
            }
            Console.WriteLine();

            Console.WriteLine($"Минимальное число = {numbers[0]} ; Его порядковый номер = {poradk[0]+1}");
            Console.WriteLine($"Максимальное число = {numbers[n-1]} ; Его порядковый номер = {poradk[n-1]+1}");
            
            Console.ReadKey();

        }
    }
}