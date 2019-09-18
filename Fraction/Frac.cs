using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraction
{
    class Frac
    {
        public int Value { get; set; }
        public static Frac operator +(Frac c1, Frac c2)
        {
            return new Frac { Value = c1.Value + c2.Value };
        }
        public static bool operator >(Frac c1, Frac c2)
        {
            return c1.Value > c2.Value;
        }
        public static bool operator <(Frac c1, Frac c2)
        {
            return c1.Value < c2.Value;
        }
    }
}
