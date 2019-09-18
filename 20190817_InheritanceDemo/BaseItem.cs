using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20190817_InheritanceDemo
{
    class BaseItem
    {
        public void F()
        {
            Console.WriteLine("BaseItem.F()");
        }

        public virtual void FVirt()
        {
            Console.WriteLine("BaseItem.FVirt()");
        }
    }
}
