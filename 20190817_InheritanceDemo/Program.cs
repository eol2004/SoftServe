using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20190817_InheritanceDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Person p1 = new Person("Vasya", "Pupkin") /*{ FirstName = "Vasya", LastName = "Pupkin" }*/;

            Console.WriteLine(p1);

            p1.F();

            Student s1 = new Student("Alex", "Turkin", 123) /*{ FirstName = "Alex", LastName = "Turkin", RecordBookNumber = 123 }*/;

            Console.WriteLine(s1);

            s1.F();

            s1.FFromBase();

            // Polymorphism !!!

            BaseItem item1 = new Student("Kolya", "Demidov", 333);

            BaseItem item3 = s1;

            object item2 = new Student("Kolya", "Ivanov", 333);

            Console.ForegroundColor = ConsoleColor.Green;

            if (item2 is Student)
            {
                //Student s7 = (Student)item2;
                //s7.F();

                ((Student)item2).F();
            }

            Student s8 = item2 as Student;
            if (s8 != null)
            {                
                s8.F();
            }

            // bad
            if (item2 is Student)
            {
                Student s7 = item2 as Student;
                s7.F();
            }

            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine();
            Console.WriteLine();

            FCall(p1);

            item1.FVirt();

            Console.WriteLine();

            FCall(s1);

            item3.FVirt();

            Console.ReadKey();
        }

        public static void FCall(object item)
        {
            if (item is Student)
            {
                ((Student)item).F();
                return;
            }

            if (item is Person)
            {
                ((Person)item).F();
                return;
            }

            if (item is BaseItem)
            {
                ((BaseItem)item).F();
                return;
            }
        }
    }
}
