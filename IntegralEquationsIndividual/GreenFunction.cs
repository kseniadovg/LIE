using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegralEquationsIndividual
{
    class GreenFunction
    {
        //can't be evaluated on Г0
        public static double NormalDerivativeYforU0(Problem p, Vector<double> x , double t)
        {
            double GreenFunctionDer;
            double a;
            double b;
            Vector<double> yVect = p.Gamma0(t);
            Vector<double> yVectDeriv = p.Gamma0Derivative(t);

            //Green Function Variables            
            double rZero = Math.Sqrt((Math.Pow(yVect.a, 2) + Math.Pow(yVect.b, 2)));
            double rAP = Math.Sqrt(Math.Pow(x.a - yVect.a, 2) + Math.Pow(x.b - yVect.b, 2));
            double rAStarP = Math.Sqrt(Math.Pow(x.a - (Math.Pow(p.R / rZero, 2)) * yVect.a, 2)
                             + Math.Pow(x.b - (Math.Pow(p.R / rZero, 2)) * yVect.b, 2));

            double rZero2 = rZero * rZero;
            double rAP2 = rAP * rAP;
            double rAStarP2 = rAStarP * rAStarP;

            //Constructing elements of the vector
            //if (Math.Abs(rAP) > 1e-10)
            //{
                a = -(yVect.a / rZero2)
                    + ((Math.Pow(p.R / rZero, 2)) * (x.a - (Math.Pow(p.R / rZero, 2)) * yVect.a)) / rAStarP2
                    - (x.a - yVect.a) / rAP2;
                b = -(yVect.b / rZero2)
                    + ((Math.Pow(p.R / rZero, 2)) * (x.b - (Math.Pow(p.R / rZero, 2)) * yVect.b)) / rAStarP2
                    - (x.b - yVect.b) / rAP2;
            //}
            //else
            //{

            //    double yDeriv2A = (p.Gamma0Derivative(yVect.a + 0.001).a - p.Gamma0Derivative(yVect.a - 0.001).a) * 2000;
            //    double yDeriv2B = (p.Gamma0Derivative(yVect.b + 0.001).b - p.Gamma0Derivative(yVect.b - 0.001).b) * 2000;
            //    double yDerivNorm2 = yVectDeriv.a * yVectDeriv.a + yVectDeriv.b * yVectDeriv.b;

            //    a = -(yVect.a / rZero2)
            //        + ((Math.Pow(p.R / rZero, 2)) * (x.a - (Math.Pow(p.R / rZero, 2)) * yVect.a)) / rAStarP2
            //        - (0.5 * yDeriv2A) / yDerivNorm2;
            //    b = -(yVect.b / rZero2)
            //        + ((Math.Pow(p.R / rZero, 2)) * (x.b - (Math.Pow(p.R / rZero, 2)) * yVect.b)) / rAStarP2
            //        - (0.5 * yDeriv2B) / yDerivNorm2;
            //}

            //Count Mju function
            Vector<double> mju = p.NormalDerivative(yVectDeriv);

            //Count Green function
            GreenFunctionDer = (1 / (2 * Math.PI)) * (a * mju.a + b * mju.b);

            return GreenFunctionDer;
        }
    }
}
