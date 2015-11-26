using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegralEquationsIndividual
{
    class IntegralsEvaluator
    {
        public static double IntegralU0(Problem p, Vector<double> x)
        {
            double sum = 0;
            int count = 1;

            double a = 0.5 * ((double)p.f0.DynamicInvoke(p.Gamma0(0).a, p.Gamma0(0).b))
                * GreenFunction.NormalDerivativeYforU0(p, x, 0);

            for (double i = 0.05; i < 2 * Math.PI; i += 0.05)
            {
                sum += (double)p.f0.DynamicInvoke(p.Gamma0(i).a, p.Gamma0(i).b)
                * GreenFunction.NormalDerivativeYforU0(p, x, i);
                count++;
            }

            double b = 0.5 * ((double)p.f0.DynamicInvoke(p.Gamma0(2 * Math.PI).a, p.Gamma0(2 * Math.PI).b))
                * GreenFunction.NormalDerivativeYforU0(p, x, 2 * Math.PI);

            //2*pi*R/count
            return -(a + sum + b) * ((4 * Math.PI) / (double)count);
        }


        public static double[,] Matrix(int n, Problem p, Vector<double> x)
        {
            double[,] Matrix = new double[2 * n, 2 * n];

            double param = Math.PI / n;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Matrix[i, j] = GreenFunction.Aphi1(p, x, i * param, j * param, n);
                }
                for (int j = n; j < 2 * n - 1; j++)
                {
                    Matrix[i, j] = GreenFunction.Bphi2(p, x, i * param, j * param);
                }
            }

            for (int i = n; i < 2 * n - 1; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Matrix[i, j] = GreenFunction.Cphi1(p, x, i * param, j * param);
                }
                for (int j = n; j < 2 * n - 1; j++)
                {
                    Matrix[i, j] = GreenFunction.Dphi2(p, x, i * param, j * param);
                }
            }


            return Matrix;
        }

    }
}
