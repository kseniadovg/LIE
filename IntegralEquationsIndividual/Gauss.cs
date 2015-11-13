﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegralEquationsIndividual
{
    class GaussMethodSLAEsolver
    {
        const double eps = 1e-8;
        List<double> X;
        int n;

        double sum(double[,] A, int k)
        {
            double temp = 0;
            for (int j = k + 1; j < n; j++)
                temp += A[k, j] * X[j];
            return temp;
        }
        bool change(double[,] A, double[] B, int k)
        {
            bool flag = true;
            double max;
            int m = 0;
            max = Math.Abs(A[k, k]);
            for (int i = k + 1; i < n; i++)
                if (Math.Abs(A[i, k]) > max)
                {
                    max = Math.Abs(A[i, k]);
                    m = i;
                }
            if (max <= eps) flag = false;
            else if (max != Math.Abs(A[k, k]))
            {
                for (int j = k; j < n; j++)
                {
                    max = A[k, j];
                    A[k, j] = A[m, j];
                    A[m, j] = max;
                }
                max = B[k];
                B[k] = B[m];
                B[m] = max;
            }
            return flag;
        }
        public List<double> GaussSolve(double[,] A, double[] B)
        {
            X = new List<double>(B.Length);
            for (int i = 0; i < B.Length; i++)
                X.Add(0);
                n = B.Length;

            // прямий хід
            double[,] M = new double[n, n];

            for (int k = 0; k < n - 1; k++)
            {
                change(A, B, k);

                for (int i = k + 1; i < n; i++)
                {
                    M[i, k] = -(A[i, k] / A[k, k]);
                    B[i] += M[i, k] * B[k];
                    for (int j = k + 1; j < n; j++) 
                        A[i, j] += M[i, k] * A[k, j];
                }

                for (int z = k + 1; z < n; z++) A[z, k] = 0;
            }

            if (A[n - 1, n - 1] != 0)
            // зворотній хід
            {
                X[n - 1] = B[n - 1] / A[n - 1, n - 1];
                for (int k = n - 2; k >= 0; k--) 
                    X[k] = (B[k] - sum(A, k)) / A[k, k];

                return X;
            }

            return null;

        }
    }
}
