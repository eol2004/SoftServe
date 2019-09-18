using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraction
{
    class Program
    {
        static void Main(string[] args)
        {
            Frac c1 = new Frac { Value = 23 };
            Frac c2 = new Frac { Value = 45 };
            Console.WriteLine(c1.Value+c2.Value);
            Console.ReadKey();

        }
    }
}
