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
                * GreenFunction.NormalDerivative_Y_forU0(p, x, 0);

            for (double i = 0.05; i < 2 * Math.PI; i += 0.05)
            {
                sum += (double)p.f0.DynamicInvoke(p.Gamma0(i).a, p.Gamma0(i).b)
                * GreenFunction.NormalDerivative_Y_forU0(p, x, i);
                count++;
            }

            double b = 0.5 * ((double)p.f0.DynamicInvoke(p.Gamma0(2 * Math.PI).a, p.Gamma0(2 * Math.PI).b))
                * GreenFunction.NormalDerivative_Y_forU0(p, x, 2 * Math.PI);

            //2*pi*R/count
            return -(a + sum + b) * ((2 * Math.PI * p.R) / (double)count);
        }

        public static double IntegralU0Der(Problem p, Vector<double> x)
        {
            double sum = 0;
            int count = 1;

            double a = 0.5 * ((double)p.f0.DynamicInvoke(p.Gamma0(0).a, p.Gamma0(0).b))
                * GreenFunction.NormalDerivative_Y_forU0(p, x, 0);

            for (double i = 0.05; i < 2 * Math.PI; i += 0.05)
            {
                sum += (double)p.f0.DynamicInvoke(p.Gamma0(i).a, p.Gamma0(i).b)
                * GreenFunction.NormalDerivative_Y_forU0(p, x, i);
                count++;
            }

            double b = 0.5 * ((double)p.f0.DynamicInvoke(p.Gamma0(2 * Math.PI).a, p.Gamma0(2 * Math.PI).b))
                * GreenFunction.NormalDerivative_Y_forU0(p, x, 2 * Math.PI);

            //2*pi*R/count
            return -(a + sum + b) * ((2 * Math.PI * p.R) / (double)count);
        }

        public static double IntegralMju1GreenOnGamma1(Problem p, Vector<double> x)
        {
            return 0;
        }
        public static double IntegralMju2GreenOnGamma2(Problem p, Vector<double> x)
        {
            return 0;
        }
    }
}
