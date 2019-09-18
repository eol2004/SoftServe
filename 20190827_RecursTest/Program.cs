using System;

namespace _20190827_RecursTest
{
    class Program
    {
        static void Main(string[] args)
        {

           int[] source = new int[13600];
            Random rand = new Random();

            var a = 0;
     
            for (int j = 0; j < source.Length; ++j)
            {
                a = rand.Next(-100, 100);
                source[j] = a;
            }


            int[] destination = new int[source.Length];

            DoProcess(source, destination);

         //   DoProcess(source, source);

            Console.WriteLine("source: ");
            foreach (var item in source)
            {
                Console.Write("{0} ", item);
            }

            Console.WriteLine();
            Console.WriteLine("destination: ");
            foreach (var item in destination)
            {
                Console.Write("{0} ", item);
            }

            Console.WriteLine();

            Console.ReadKey();
        }

        public static void DoProcess(int[] source, int[] destination)
        {
            int destinationPosition = 0;

            DoProcess(source, destination, 0, ref destinationPosition);
        }

        private static void DoProcess(int[] source, int[] destination,
                int sourcePosition, ref int destinationPosition)
        {
            if ((sourcePosition >= source.Length)
                || (source == destination))
            {
                return;    // тривиальный случай
            }

            // !!! действия перед входом в рекурсию

            if (source[sourcePosition] < 0)
            {
                destination[destinationPosition/*++*/] = source[sourcePosition];
                ++destinationPosition;
            }

            DoProcess(source, destination, sourcePosition + 1, ref destinationPosition);

            // !!! действия после выхода из рекурсии

            if (source[sourcePosition] >= 0)
            {
                destination[destinationPosition/*++*/] = source[sourcePosition];
                ++destinationPosition;
            }          
        }

    }

}
