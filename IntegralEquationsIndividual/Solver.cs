using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegralEquationsIndividual
{
    class Solver
    {
        public Problem p;
        public double step;
        Result res;

        public Solver(Problem pr, double step)
        {
            p = pr;
            this.step = step;
            GenerateMesh();
        }

        public void Solve()
        {
            double u0 = IntegralsEvaluator.IntegralU0(p, new Vector<double>(1, 0));
        }

        public void GenerateMesh()
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

            Vector<double> min1 = FindLeftBottomCornerOfMinRectangleFor(p.Gamma1);
            Vector<double> min2 = FindLeftBottomCornerOfMinRectangleFor(p.Gamma2);
            Vector<double> max1 = FindRightTopCornerOfMinRectangleFor(p.Gamma1);
            Vector<double> max2 = FindRightTopCornerOfMinRectangleFor(p.Gamma2);

            int k = 0;
            while (k < p.mesh.Count)
            {
                if ((p.mesh[k].a >= min1.a) && (p.mesh[k].a <= max1.a) && (p.mesh[k].b >= min1.b) && (p.mesh[k].b <= max1.b)
                    || (p.mesh[k].a >= min2.a) && (p.mesh[k].a <= max2.a) && (p.mesh[k].b >= min2.b) && (p.mesh[k].b <= max2.b))
                {
                    p.mesh.RemoveAt(k);
                }
                else
                    k++;
            }
        }
        Vector<double> FindLeftBottomCornerOfMinRectangleFor(Vector<Delegate> vect)
        {
            double left = (double)vect.a.DynamicInvoke(0);
            double bottom = (double)vect.b.DynamicInvoke(0);
            double temp1, temp2;
            for (double t = 0; t < 2 * Math.PI; t += 0.1)
            {
                temp1 = (double)vect.a.DynamicInvoke(t);
                temp2 = (double)vect.b.DynamicInvoke(t);
                if (temp1 < left)
                    left = temp1;
                if (temp2 < bottom)
                    bottom = temp2;
            }

            return new Vector<double>(left, bottom);
        }
        Vector<double> FindRightTopCornerOfMinRectangleFor(Vector<Delegate> vect)
        {
            double right = (double)vect.a.DynamicInvoke(0);
            double top = (double)vect.b.DynamicInvoke(0);
            double temp1, temp2;
            for (double t = 0; t < 2 * Math.PI; t += 0.1)
            {
                temp1 = (double)vect.a.DynamicInvoke(t);
                temp2 = (double)vect.b.DynamicInvoke(t);
                if (temp1 > right)
                    right = temp1;
                if (temp2 > top)
                    top = temp2;
            }

            return new Vector<double>(right, top);
        }

        public static double[,] Matrix(int n, Problem p)
        {
            double[,] Matrix = new double[2 * n, 2 * n];

            double param = 2 * Math.PI / n;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Matrix[i, j] = GreenFunction.Aphi1(p, i * param, j * param, n);
                }
                for (int j = n; j < 2 * n - 1; j++)
                {
                    Matrix[i, j] = GreenFunction.Bphi2(p, i * param, j * param);
                }
            }

            for (int i = n; i < 2 * n - 1; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Matrix[i, j] = GreenFunction.Cphi1(p, i * param, j * param);
                }
                for (int j = n; j < 2 * n - 1; j++)
                {
                    Matrix[i, j] = GreenFunction.Dphi2(p, i * param, j * param);
                    if (i == j)
                        Matrix[i, j] += 1 / 2d;
                }
            }


            return Matrix;
        }

        public static double[] Vector(int n, Problem p, Vector<double> x)
        {
            double[] vector = new double[2 * n];

            for (int j = 0; j < n; j++)
            {
                vector[j] = IntegralsEvaluator.IntegralU0(p, x);
            }
            for (int j = n; j < 2 * n - 1; j++)
            {
                vector[j] = 0;
            }

            return vector;
        }

    }
}
