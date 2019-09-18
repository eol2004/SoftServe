using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _20190819_OperationsOverloadingDemo
{
    class MyInt
    {
        public MyInt(int val = 0)
        {
            _val = val;
        }

        public int Val
        {
            get { return _val; }
            set { _val = value; }
        }

        public static MyInt Add(MyInt arg1, MyInt arg2)
        {
            return new MyInt(arg1._val + arg2._val);
        }

        #region Арифметические операции

        public static MyInt operator +(MyInt arg1, MyInt arg2)
        {
            return new MyInt(arg1._val + arg2._val);
        }

        public static MyInt operator *(MyInt arg1, MyInt arg2)
        {
            return new MyInt(arg1._val * arg2._val);
        }

        #endregion

        #region Логические операции

        public static bool operator &(MyInt arg1, MyInt arg2)
        {
            bool a = (arg1._val != 0);

            bool b = (arg2._val != 0);

            return a & b;
        }

        #endregion

        #region Операции сравнения

        public static bool operator >(MyInt arg1, MyInt arg2)
        {
            return arg1._val > arg2._val;
        }

        public static bool operator <(MyInt arg1, MyInt arg2)
        {
            return arg1._val < arg2._val;
        }

        public static bool operator ==(MyInt arg1, MyInt arg2)
        {
            return arg1._val == arg2._val;
        }

        public static bool operator !=(MyInt arg1, MyInt arg2)
        {
            return !(arg1 == arg2);
        }

        #endregion

        // перегрузка операций преобразования типа

        // !!! неявное
        //public static implicit operator double(MyInt arg)
        //{
        //    return arg._val;
        //}

        // !!! явное
        public static explicit operator int(MyInt arg)
        {
            return arg._val;
        }

        public override string ToString()
        {
            return string.Format("<{0}>", _val);
        }

        private int _val;
    }
}
