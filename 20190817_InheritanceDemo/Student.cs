using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20190817_InheritanceDemo
{
    // Person - base class
    // Student - derived class
    sealed class Student : Person
    {

        public Student(string firstName, string lastName, int recordBookNumber)
            : base(firstName, lastName)    // вызов конструктора ч 2-мя параметрами базового класса Person
        {
            Console.WriteLine("ctor Student({0},{1},{2})", firstName, lastName, recordBookNumber);
            //FirstName = firstName;
            //LastName = lastName;
            RecordBookNumber = recordBookNumber;
        }

        ~Student()
        {
            Console.WriteLine("dtor Student({0},{1},{2})", FirstName, LastName, RecordBookNumber);
        }

        public void F()
        {
            //base.F();    // вызов реализации из базового класса
            Console.WriteLine("Student.F()");
        }

        public override void FVirt()
        {
            Console.WriteLine("Student.FVirt()");
        }


        public void FFromBase()
        {
            base.F();    // вызов реализации из базового класса
            //Console.WriteLine("Student.F()");
        }

        public int RecordBookNumber { get; set; }

        public override string ToString()
        {
            return string.Format("({0} {1}, {2})", FirstName, LastName, RecordBookNumber);
        }
    }
}
