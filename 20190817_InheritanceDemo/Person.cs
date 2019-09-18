using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20190817_InheritanceDemo
{
    class Person : BaseItem
    {
        public Person(string firstName, string lastName)
        {
            Console.WriteLine("ctor Person({0},{1})", firstName, lastName);
            FirstName = firstName;
            LastName = lastName;
        }

        ~Person()
        {
            Console.WriteLine("dtor Person({0},{1})", FirstName, LastName);
        }

        // protected - a-la private + acces in child classes
        public string FirstName { get; protected set; }
        public string LastName { get; protected set; }

        public void F()
        {
            Console.WriteLine("Person.F()");
        }

        public override void FVirt()
        {
            Console.WriteLine("Person.FVirt()");
        }

        public override string ToString()
        {
            return string.Format("({0} {1})", FirstName, LastName);
        }

    }
}
