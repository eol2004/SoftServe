using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V
{
    class Figure
    {
        public int radius;

        public Figure()
        {
            radius = 5;
        }


        public Figure(int newRadius)
        {
            setRadius(newRadius);
        }

        protected virtual void setRadius(int newRadius)
        {
            if (newRadius > 0)
            {
                radius = newRadius;
            }
        }

        public virtual void draw()
        {
            Console.WriteLine("base");
        }
        public void draw2()
        {
            Console.WriteLine("base");
        }
    }

    class Triangle : Figure
    {
        public Triangle(): base()
        {

        }

        public Triangle(int newRadius): base(newRadius)
        {
        }

        protected override void setRadius(int newRadius)
        {
            if (newRadius > 0)
            {
                radius = newRadius;
            }
        }

        public override void draw()
        {
            Console.WriteLine("triangle");
        }
        public new void draw2()
        {
            Console.WriteLine("triangle");
        }
    }
    class Circle : Figure
    {
        public Circle() : base()
        {
            
        }

        public Circle(int newRadius) : base(newRadius)
        {
        }

        public override void draw()
        {
            Console.WriteLine("circle");
        }
        public new void draw2()
        {
            Console.WriteLine("circle");
        }
    }
}
