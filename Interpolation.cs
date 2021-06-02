using NumericalMethods;
using System;
using System.Collections.Generic;
using System.Windows;


namespace Approximation
{
    public class Interpolation
    {
        private List<Point> points;

        public Interpolation(List<Point> points)
        {
            if (points.Count < 2) throw new Exception("Количетсво узлов интерполяции должно быть больше 1");
            this.points = points;
        }

        private double CalcFinalDiff(int n, int i)
        {
            double diff;

            if (n == 1) diff = points[i + 1].Y - points[i].Y;
            else if (n == 2) diff = points[i + 2].Y - 2 * points[i + 1].Y + points[i].Y;
            else diff = CalcFinalDiff(n - 1, i + 1) - CalcFinalDiff(n - 1, i);

            return diff;
        }

        private double Factorial(int n)
        {
            double factorial;

            if (n == 0 || n == 1) factorial = 1;
            else factorial = Factorial(n - 1) * n;

            return factorial;
        }

        private void CheckStep(double h) 
        {
            if (h <= 0) throw new Exception("Шаг приращения аргумента X не может быть меньшу нуля");
        }

        public List<Point> LinearMethod(double h)
        {
            CheckStep(h);

            List<Point> resultPoints = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                if (i == points.Count - 1) break;
                else
                {
                    for (double x = points[i].X; x <= points[i + 1].X; x += h)
                    {
                        double y = points[i].Y + (((points[i + 1].Y - points[i].Y) / (points[i + 1].X - points[i].X)) * (x - points[i].X));
                        resultPoints.Add(new Point(x, y));
                    }
                }
            }
            return resultPoints;
        }

        public List<Point> QuadraticMethod(double h)
        {
            CheckStep(h);
            if (points.Count < 3) throw new Exception("Количество точек в заданном интервале недостаточно.");
            
            List<Point> resultPoints = new List<Point>();
            for (int k = 0; k < points.Count - 2; k++)
            {
                double[,] valuesA = new double[3, 3];
                double[,] valuesB = new double[3, 1];

                for (int i = 0; i < valuesA.GetLength(0); i++)
                {
                    for (int j = 0; j < valuesA.GetLength(1); j++)
                    {
                        valuesA[i, j] = Math.Pow(points[k + i].X, 2 - j);
                    }
                    valuesB[i, 0] = points[k + i].Y;
                }
                Matrix A = new Matrix(valuesA);
                Matrix B = new Matrix(valuesB);

                SystemEquations systemEquations = new SystemEquations(A, B);
                double[,] solution = systemEquations.MatrixMethod();

                double a, b, c;
                a = solution[0, 0];
                b = solution[1, 0];
                c = solution[2, 0];

                if (k == 0)
                {
                    for (double x = points[0].X; x < points[1].X; x += h)
                    {
                        double newX = x;
                        double newY = a * Math.Pow(newX, 2) + b * newX + c;
                        resultPoints.Add(new Point(newX, newY));
                    }
                }
                for (double x = points[k + 1].X; x <= points[k + 2].X; x += h)
                {
                    double newX = x;
                    double newY = a * Math.Pow(newX, 2) + b * newX + c;
                    resultPoints.Add(new Point(newX, newY));
                }
            }
            return resultPoints;
        }

        public List<Point> LagrangePolynomial(double h)
        {
            CheckStep(h);

            List<Point> resultPoints = new List<Point>();
            for (double x = points[0].X; x <= points[points.Count - 1].X; x += h)
            {
                double Lx = 0;
                for (int i = 0; i < points.Count; i++)
                {
                    double lx = 1;
                    for (int j = 0; j < points.Count; j++)
                    {
                        if (i == j) continue;

                        lx *= (x - points[j].X) / (points[i].X - points[j].X);
                    }
                    Lx += points[i].Y * lx;
                }
                resultPoints.Add(new Point(x, Lx));
            }

            return resultPoints;
        }

        public List<Point> NewtonPolynomial(double h)
        {
            CheckStep(h);

            double step = points[1].X - points[0].X;
            for (int i = 0; i < points.Count - 1; i++)
                if (points[i + 1].X - points[i].X != step) throw new Exception("Узлы не являются равноотстоящими");

            List<Point> resPoints = new List<Point>();
            for (double x = points[0].X; x <= points[points.Count - 1].X; x += h)
            {
                int k = points.Count - 1;
                double q = (x - points[0].X) / step;
                double p = points[0].Y + q * CalcFinalDiff(1, 0);
                
                for (int n = 2; n <= k; n++)
                {
                    double qMult = q;
                    for (int i = 2; i <= n; i++)
                    {
                        qMult *= q - i + 1;
                    }
                    p += CalcFinalDiff(n, 0) * (qMult / Factorial(n));
                }
                resPoints.Add(new Point(x, p));
            }

            return resPoints;
        }

        public List<Point> SplineMethod(double step)
        {
            CheckStep(step);

            double[,] A = new double[points.Count, points.Count];
            double[,] B = new double[points.Count, 1];
            double h = points[1].X - points[0].X;
            for (int i = 0; i < A.GetLength(0); i++)
            {
                for (int j = 0; j < A.GetLength(1); j++)
                {
                    if (i == j) A[i, j] = 4 * h / 3;
                    else if (i == j + 1 || i == j - 1) A[i, j] = h / 3;
                }
            }

            for (int i = 1; i < B.GetLength(0) - 1; i++)
                B[i, 0] = (points[i + 1].Y - 2 * points[i].Y + points[i - 1].Y) / h;

            SystemEquations equations = new SystemEquations(new Matrix(A), new Matrix(B));
            double[,] C = equations.GausGordanMethod();

            List<Point> resultPoints = new List<Point>();
            for (int k = 1; k < points.Count - 1; k++)
            {
                for (double x = points[k].X; x <= points[k + 1].X; x += step)
                {
                    double a = points[k - 1].Y;
                    double b = ((points[k].Y - points[k - 1].Y) / h) - (h / 3) * (2 * C[k, 0] + C[k + 1, 0]);
                    double d = (C[k + 1, 0] - C[k, 0]) / (3 * h);
                    double c = C[k, 0];
                    double y = a + b * (x - points[k].X) + c * Math.Pow(x - points[k].X, 2) + d * Math.Pow(x - points[k].X, 3);
                    resultPoints.Add(new Point(x - h, y));
                }
            }
            return resultPoints;
        }
    }
}