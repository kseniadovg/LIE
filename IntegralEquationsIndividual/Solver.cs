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

        public static double[,] Matrix(int n, Problem p)
        {
            double[,] Matrix = new double[2 * n, 2 * n];

            double param = Math.PI / n;

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

        public static double[] Vector(int n, Problem p)
        {
            double[] vector = new double[2 * n];

            for (int j = 0; j < n; j++)
            {
                vector[j] = 0;
            }
            for (int j = n; j < 2 * n - 1; j++)
            {
                vector[j] = 0;
            }

            return vector;
        }

    }
}
