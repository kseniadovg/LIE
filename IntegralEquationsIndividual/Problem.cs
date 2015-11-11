using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegralEquationsIndividual
{
    class Problem
    {
        public Vector<Delegate> Gamma1;
        public Vector<Delegate> Gamma2;
        public Vector<Delegate> Gamma1Derivative;
        public Vector<Delegate> Gamma2Derivative;
        Delegate f0, f1, f2;
        double R;

        public Problem(double r, Vector<Delegate> v1,Vector<Delegate> v2,Vector<Delegate> v1der,Vector<Delegate> v2der,
            Delegate func0,Delegate func1,Delegate func2)
        {
            Gamma1 = v1;
            Gamma2 = v2;
            Gamma1Derivative = v1der;
            Gamma2Derivative = v2der;
            f0 = func0;
            f1 = func1;
            f2 = func2;
            R = r;
        }

        public Vector<double> Gamma0(double t)
        {            
            return new Vector<double>(R * Math.Cos(t), R * Math.Sin(t));
        }
        public Vector<Double> Gamma0Derivative(double t)
        {
            return new Vector<double>(-R * Math.Sin(t), R * Math.Cos(t));
        }

        public Vector<double> NormalDerivative(Vector<double> curveDerivativeValues)
        {
            double derivativeNorm = Math.Sqrt(Math.Pow(curveDerivativeValues.a, 2) + Math.Pow(curveDerivativeValues.b, 2));

            return new Vector<double>(curveDerivativeValues.b / derivativeNorm, -curveDerivativeValues.a / derivativeNorm);
        }



    }
}
