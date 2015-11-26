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

        public static double Aphi1(Problem p, Vector<double> x, double t,double tau, double n)
        {
            Vector<double> yVect = new Vector<double>();
            yVect.a = (double)p.Gamma1.a.DynamicInvoke(tau);
            yVect.b = (double)p.Gamma1.b.DynamicInvoke(tau);

            double rZero = Math.Sqrt((Math.Pow(yVect.a, 2) + Math.Pow(yVect.b, 2)));
            double rAStarP = Math.Sqrt(Math.Pow(x.a - (Math.Pow(p.R / rZero, 2)) * yVect.a, 2)
                             + Math.Pow(x.b - (Math.Pow(p.R / rZero, 2)) * yVect.b, 2));

            return Math.Log(p.R / rZero, Math.Exp(1)) - Math.Log(rAStarP, Math.Exp(1)) - H1(p, x, t, tau, n);
        }

        public static double Bphi2(Problem p, Vector<double> x, double t, double tau)
        {
            Vector<double> yVect = new Vector<double>();
            yVect.a = (double)p.Gamma2.a.DynamicInvoke(tau);
            yVect.b = (double)p.Gamma2.b.DynamicInvoke(tau);
            double rZero;
            double rAP;
            double rAStarP;
            if (yVect.a == 0 && yVect.b == 0)
                rZero = 0;
            else
                rZero = Math.Sqrt((Math.Pow(yVect.a, 2) + Math.Pow(yVect.b, 2)));
            if ((x.a == yVect.a) && (x.b == yVect.b))
                rAP = 0;
            else
                rAP = Math.Sqrt(Math.Pow(x.a - yVect.a, 2) + Math.Pow(x.b - yVect.b, 2));
            rAStarP = Math.Sqrt(Math.Pow(x.a - (Math.Pow(p.R / rZero, 2)) * yVect.a, 2)
                             + Math.Pow(x.b - (Math.Pow(p.R / rZero, 2)) * yVect.b, 2));

            return Math.Log(p.R, Math.Exp(1)) - Math.Log(rZero, Math.Exp(1)) - Math.Log(rAStarP, Math.Exp(1)) + Math.Log(rAP, Math.Exp(1));
        }

        public static double Cphi1(Problem p, Vector<double> x, double t, double tau)
        {
            Vector<double> yVectDeriv = new Vector<double>();
            Vector<double> yVect = new Vector<double>();
            yVect.a = (double)p.Gamma1.a.DynamicInvoke(tau);
            yVect.b = (double)p.Gamma1.b.DynamicInvoke(tau);
            yVectDeriv.a = (double)p.Gamma1Derivative.a.DynamicInvoke(tau);
            yVectDeriv.b = (double)p.Gamma1Derivative.b.DynamicInvoke(tau);

            double a;
            double b;

            double rZero = Math.Sqrt((Math.Pow(yVect.a, 2) + Math.Pow(yVect.b, 2)));
            double rAP = Math.Sqrt(Math.Pow(x.a - yVect.a, 2) + Math.Pow(x.b - yVect.b, 2));
            double rAStarP = Math.Sqrt(Math.Pow(x.a - (Math.Pow(p.R / rZero, 2)) * yVect.a, 2)
                             + Math.Pow(x.b - (Math.Pow(p.R / rZero, 2)) * yVect.b, 2));

            double rZero2 = rZero * rZero;
            double rAP2 = rAP * rAP;
            double rAStarP2 = rAStarP * rAStarP;


            a = -(x.a - (Math.Pow(p.R / rZero, 2)) * yVect.a) / rAStarP2
                - (x.a - yVect.a) / rAP2;
            b = -(x.b - (Math.Pow(p.R / rZero, 2)) * yVect.b) / rAStarP2
                - (x.b - yVect.b) / rAP2;


            Vector<double> mju = p.NormalDerivative(x);
            return (1/(2*Math.PI)) * (a * mju.a + b * mju.b);
        }

        public static double Dphi2(Problem p, Vector<double> x, double t, double tau)
        {
            Vector<double> xDeriv = new Vector<double>();
            Vector<double> yVect = new Vector<double>();
            yVect.a = (double)p.Gamma2.a.DynamicInvoke(tau);
            yVect.b = (double)p.Gamma2.b.DynamicInvoke(tau);
            xDeriv.a = (double)p.Gamma2Derivative.a.DynamicInvoke(t);
            xDeriv.b = (double)p.Gamma2Derivative.b.DynamicInvoke(t);

            double rZero = Math.Sqrt((Math.Pow(yVect.a, 2) + Math.Pow(yVect.b, 2)));
            double rAP = Math.Sqrt(Math.Pow(x.a - yVect.a, 2) + Math.Pow(x.b - yVect.b, 2));
            double rAStarP = Math.Sqrt(Math.Pow(x.a - (Math.Pow(p.R / rZero, 2)) * yVect.a, 2)
                             + Math.Pow(x.b - (Math.Pow(p.R / rZero, 2)) * yVect.b, 2));

            double rZero2 = rZero * rZero;
            double rAP2 = rAP * rAP;
            double rAStarP2 = rAStarP * rAStarP;

            if (t != tau)
            {
                double a;
                double b;
                a = (p.R * p.R / rZero2) *
                    (x.a - (p.R * p.R / rZero2) * yVect.a) / Math.Pow(EuclidNorm(x.a - (p.R * p.R / rZero2) * yVect.a, x.b - (p.R * p.R / rZero2) * yVect.b), 2) -
                    (x.a - yVect.a) / Math.Pow(EuclidNorm(x.a - yVect.a, x.b - yVect.b), 2);
                b = (p.R * p.R / rZero2) *
                    (x.b - (p.R * p.R / rZero2) * yVect.b) / Math.Pow(EuclidNorm(x.a - (p.R * p.R / rZero2) * yVect.a, x.b - (p.R * p.R / rZero2) * yVect.b), 2) -
                    (x.b - yVect.b) / Math.Pow(EuclidNorm(x.a - yVect.a, x.b - yVect.b), 2);

                Vector<double> mju = p.NormalDerivative(x);
                return (1 / (2 * Math.PI)) * (a * mju.a + b * mju.b);
            }
            else
            {
                Vector<double> mju = p.NormalDerivative(x);
                double a;
                double b;

                double xDeriv2A = ((double)p.Gamma2Derivative.a.DynamicInvoke(t + 0.001) - (double)p.Gamma2Derivative.a.DynamicInvoke(t - 0.001)) * 2000;
                double xDeriv2B = ((double)p.Gamma2Derivative.b.DynamicInvoke(t + 0.001) - (double)p.Gamma2Derivative.b.DynamicInvoke(t - 0.001)) * 2000;


                a = (p.R * p.R / rZero2) *
                    (x.a - (p.R * p.R / rZero2) * yVect.a) / Math.Pow(EuclidNorm(x.a - (p.R * p.R / rZero2) * yVect.a, x.b - (p.R * p.R / rZero2) * yVect.b), 2) * mju.a -
                    (1 / 2d) * (xDeriv.b * xDeriv2A) / (Math.Pow(EuclidNorm(xDeriv.a, xDeriv.b), 3));
                b = (p.R * p.R / rZero2) *
                    (x.b - (p.R * p.R / rZero2) * yVect.b) / Math.Pow(EuclidNorm(x.a - (p.R * p.R / rZero2) * yVect.a, x.b - (p.R * p.R / rZero2) * yVect.b), 2) * mju.b -
                    (1 / 2d) * (-xDeriv.a * xDeriv2B) / (Math.Pow(EuclidNorm(xDeriv.a, xDeriv.b), 3));

                return (1 / (2 * Math.PI)) * (a + b);
            }
        }

        public static double H1(Problem p, Vector<double> x, double t, double tau, double n)
        {
            Vector<double> yVect = new Vector<double>();
            yVect.a = (double)p.Gamma1.a.DynamicInvoke(tau);
            yVect.b = (double)p.Gamma1.b.DynamicInvoke(tau);
            if (t != tau)
            {
                double rAP = Math.Sqrt(Math.Pow(x.a - yVect.a, 2) + Math.Pow(x.b - yVect.b, 2));
                return Math.Log(1 / rAP, Math.Exp(1));
            }
            else
            {
                //double norm = EuclidNorm((double)p.Gamma1Derivative.a.DynamicInvoke(tau), (double)p.Gamma1Derivative.b.DynamicInvoke(tau));
                //return (-1 / 2d) * Math.Log((4 / Math.Exp(1)) * Math.Pow(Math.Sin((t - tau) / 2), 2), Math.Exp(1)) - 1 / 2 * Math.Log(2 * Math.Pow(norm, 2)) - 1 / 2d;
                return -1 / 2d * Math.Pow((R(t, tau, n) - 1 / (2 * n) * Math.Log(1, Math.Exp(1))), 2);
            }
        }

        public static double R(double t, double tau, double n)
        {
            double R = 0;
            for (int i = 0; i < n; i++)
            {
                R += 1 / i * Math.Cos(i * (t - tau));
            }
            R *= 1 / n;
            R = -1 / (2 * n * n) * Math.Cos(n * (t - tau));
            return R;
        }

        public static double EuclidNorm(double x, double y)
        {
            return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
        }

    }
}
