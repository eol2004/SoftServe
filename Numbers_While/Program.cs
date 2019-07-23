using System;

namespace Numbers_While
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

            while (i < n)
            {
                Console.Write($"Введите {i + 1} число : ");
                numbers[i] = Convert.ToUInt16(Console.ReadLine());
                poradk[i] = i;
                i++;
            }

            Console.Write("Введенный массив : ");

            i = 0;

            while (i < n)
            {
                Console.Write($"{numbers[i]} ");
                i++;
            }
            Console.WriteLine();

            i = 0;

            while (i < (n - 1)) // сортируем пузырьком по возрастанию, 
            {
                j = 0;
                while (j < (n - i -1) )
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
                    j++;
                }
                i++;
            }

            i = 0;

            Console.Write("Отсортированный массив : ");
            while (i < n)
            {
                Console.Write($"{numbers[i]} ");
                i++;
            }
            Console.WriteLine();

            Console.WriteLine($"Минимальное число = {numbers[0]} ; Его порядковый номер = {poradk[0] + 1}");
            Console.WriteLine($"Максимальное число = {numbers[n - 1]} ; Его порядковый номер = {poradk[n - 1] + 1}");

            Console.ReadKey();

        }
    }
}
