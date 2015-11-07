using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegralEquationsIndividual
{
    class Vector<T>
    {
        public T a { get; private set; }
        public T b { get; private set; }

        public Vector(T a, T b)
        {
            this.a = a;
            this.b = b;
        }
    }
}
