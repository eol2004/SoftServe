using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recursion
{
    class Num
    {       public void Input(int i)
        {
            Random rnd = new Random();
            Console.WriteLine("Введите количество элементов массива");
            i = Convert.ToInt32(Console.ReadLine());
        }
    }
}
