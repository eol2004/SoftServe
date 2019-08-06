using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V
{
    class Program
    {
        static void Main(string[] args)
        {
            Figure f = new Triangle();

            f.radius = 5;
            f.draw();
            f.draw2();
            Console.WriteLine(f.radius);
            Console.ReadKey();
        }
    }

}
