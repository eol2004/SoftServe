using System;

namespace My_Project
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ASCII table");
            int i = 1;
         
                for (i = 1; i < 1256; i++)
                {
                    Console.Write(Convert.ToChar(i));
                 }
                Console.WriteLine();
            
              Console.ReadKey();

        }
    }
}
