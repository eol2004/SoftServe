using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20190819_OperationsOverloadingDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            MyInt a = new MyInt(2);
            MyInt b = new MyInt(22);
            MyInt b0 = new MyInt(0);


            MyInt c = MyInt.Add(a, b);

            Console.WriteLine("{0} + {1} = {2}", a, b, c);

            MyInt d = a + b;

            MyInt e = a * b;

            //MyInt f = a / b;

            if (a & b)
            {
                Console.WriteLine("a & b - true");
            }
            else
            {
                Console.WriteLine("a & b - false");
            }

            if (a & b0)
            {
                Console.WriteLine("a & b0 - true");
            }
            else
            {
                Console.WriteLine("a & b0 - false");
            }

            Console.WriteLine("{0} > {1}: {2}", a, b, a > b);

            Console.WriteLine("{0} > {1}: {2}", a, b0, a > b0);

            Console.WriteLine("{0} != {1}: {2}", a, b0, a != b0);

            int k = 10;

            double z = k;    // !!! неявное

            z += 0.5;

            int k1 = (int)z;    // ?

//            int k2 = Math.Round()

            z = (double)b;

            k1 = (int)b;

            Console.ReadKey();
        }
    }
}
