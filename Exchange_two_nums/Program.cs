using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exchange_two_nums
{
    class Program
    {

        static void Main(string[] args)
        {
            //1
            byte a = 4;
            byte b = 7;
            byte s;

            s = a;
            a = b;
            b = s;
            Console.WriteLine($"a = {a} ; b = {b}");

            //2
            int sum (byte c, byte d, out byte a1)
            {
                a1 = a;
                return c + d;
            }

            byte aa1; 
            a = (byte)(sum(a, b, out aa1) - aa1);
            b = (byte)(sum(aa1, b, out aa1) - b);
            Console.WriteLine($"a = {a} ; b = {b}");

            //3
            byte[] arr1 = null;   
            arr1 = new byte[2];
            arr1[0] = 7;
            arr1[1] = 4;
            Array.Sort(arr1);
            Console.WriteLine($" {arr1[0]} ; {arr1[1]}");

            //4
            void Xchng(ref byte x, ref byte y)
            {
                byte z;
                z = x;
                x = y;
                y = z;
            }

            Xchng(ref a, ref b);
            Console.WriteLine($"a = {a} ; b = {b}");

            Console.ReadKey();
        }

    }
}

