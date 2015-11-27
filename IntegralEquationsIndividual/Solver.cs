using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntegralEquationsIndividual
{
    class Solver
    {
        public Problem p;
        public double meshStep;
        public int integralStep;
        int pointsOnCurve = 10;
        public Result res;

        public Solver(Problem pr, double mstep, int istep)
        {
            p = pr;
            meshStep = mstep;
            integralStep = istep;
            GenerateMesh();
        }

        public void Solve()
        {
            //double u0 = IntegralsEvaluator.IntegralU0(p, new Vector<double>(1, 0));            

            GaussMethodSLAEsolver gauss=new GaussMethodSLAEsolver();
            List<double> mju = gauss.GaussSolve(Matrix(integralStep, p), Vector(integralStep, p));
            res.mju1 = mju.GetRange(0, integralStep);
            res.mju2 = mju.GetRange(integralStep, integralStep);

            p.u = new List<double>();
            foreach(var v in p.mesh)
            {
                p.u.Add(
                    IntegralsEvaluator.IntegralU0(p, v) +
                    IntegralsEvaluator.IntegralMju1GreenOnGamma1(p, v) +
                    IntegralsEvaluator.IntegralMju2GreenOnGamma2(p, v));
            }

            double param = 2 * Math.PI / pointsOnCurve;
            Vector<double> gamma0, gamma1;
            for(int i=0;i<pointsOnCurve;i++)
            {
                gamma0 = p.Gamma0(i * param);
                gamma1 = new Vector<double>(
                    (double)p.Gamma1.a.DynamicInvoke(i * param),
                    (double)p.Gamma1.b.DynamicInvoke(i * param));

                p.mesh.Add(gamma0);
                p.mesh.Add(gamma1);

                p.u.Add((double)p.f0.DynamicInvoke(gamma0));
                p.u.Add((double)p.f1.DynamicInvoke(gamma1));

            }

            // do writing to file mesh and u as x y u

        }

        public void GenerateMesh()
        {
            p.mesh = new List<Vector<double>>();

            for (double i = 0; i <= p.R; i += meshStep)
            {
                for (double j = 0; j <= p.R; j += meshStep)
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

            double param = 2 * Math.PI / pointsOnCurve;
            for (int i = 0; i < pointsOnCurve; i++)
            {
                p.mesh.Add(new Vector<double>(
                    (double)p.Gamma2.a.DynamicInvoke(i * param), 
                    (double)p.Gamma2.b.DynamicInvoke(i * param)));
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

                    Matrix[i, j + n] = GreenFunction.Bphi2(p, i * param, j * param);

                    Matrix[i + n, j] = GreenFunction.Cphi1(p, i * param, j * param);

                    Matrix[i + n, j + n] = GreenFunction.Dphi2(p, i * param, j * param);
                    if (i == j)
                        Matrix[i, j] += 0.5;
                }
            }

            return Matrix;
        }

        public static double[] Vector(int n, Problem p)
        {
            double[] vector = new double[2 * n];

            double param = 2 * Math.PI / n;

            for (int j = 0; j < n; j++)
            {
                vector[j] = IntegralsEvaluator.IntegralU0(p,new Vector<double>(
                    (double)p.Gamma1.a.DynamicInvoke(j * param), 
                    (double)p.Gamma1.b.DynamicInvoke(j * param)));

                vector[j + n] = IntegralsEvaluator.IntegralU0Der(p,new Vector<double>(
                    (double)p.Gamma2.a.DynamicInvoke(j * param), 
                    (double)p.Gamma2.b.DynamicInvoke(j * param)));
            }

            return vector;
        }

    }
}
