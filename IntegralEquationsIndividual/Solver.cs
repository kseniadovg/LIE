using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegralEquationsIndividual
{
    class Solver
    {
        Problem p;
        public double step;
        Result res;

        public Solver(Problem pr)
        {
            p = pr;
        }

        public void Solve()
        {
            double u0 = IntegralsEvaluator.IntegralU0(p, new Vector<double>(1, 0));
        }

        void GenerateMesh()
        {
            p.mesh = new List<Vector<double>>();

            for (double i = 0; i <= p.R; i += step)
            {
                for (double j = 0; j <= p.R; j += step)
                    if (i * i + j * j < p.R * p.R)
                    {
                        p.mesh.Add(new Vector<double>(i, j));
                        p.mesh.Add(new Vector<double>(-i, j));
                        p.mesh.Add(new Vector<double>(i, -j));
                        p.mesh.Add(new Vector<double>(-i, -j));
                    }
            }


        }
        Vector<double> FindLeftBottomCornerOfMinRectangleFor(Vector<Delegate> vect)
        {
            return null;
        }
        Vector<double> FindRightTopCornerOfMinRectangleFor(Vector<Delegate> vect)
        {
            return null;
        }

    }
}
