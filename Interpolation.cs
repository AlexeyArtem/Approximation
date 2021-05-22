using SolutionSystemEquations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace Approximation
{
    public class Interpolation
    {
        private List<Point> points;

        public Interpolation(List<Point> points)
        {
            this.points = points;
        }

        private void CheckCondition(double x, int numPoint1, int numPoint2)
        {
            if (numPoint2 - numPoint1 != 1) throw new Exception("Выбранные точки не являются соседними.");
            if (x < points[numPoint1].X || x > points[numPoint2].X) throw new Exception("Значение X не входит в заданный промежуток.");
        }

        public Point LinearMethod(double x, int numPoint1, int numPoint2)
        {
            if (points.Count < 2) throw new Exception("Количество точек в заданном интервале недостаточно.");
            CheckCondition(x, numPoint1, numPoint2);

            double y = points[numPoint1].Y + ((points[numPoint2].Y - points[numPoint1].Y) / (points[numPoint2].X - points[numPoint1].X)) * (x - points[numPoint1].X);
            return new Point(x, y);
        }

        public List<Point> QuadraticMethod(double h)
        {
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

        public Point LagrangePolynomial(double x)
        {
            //?
            if (points.Count < 2) throw new Exception("Количество точек недостаточно.");

            double minX = points[0].X;
            double maxX = points[0].X;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].X < minX) minX = points[i].X;
                else if (points[i].X > maxX) maxX = points[i].X;
            }
            if (x < minX || x > maxX) throw new Exception("Значение X не входит в заданный промежуток.");

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

            return new Point(x, Lx);
        }

        public Point NewtonPolynomial(double x)
        {
            double h = points[1].X - points[0].X;

            for (int i = 0; i < points.Count - 1; i++)
            {
                if (points[i + 1].X - points[i].X != h) throw new Exception("Узлы не являются равноотстоящими");
            }

            int k = points.Count - 1;
            double q = (x - points[0].X) / h;
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

            return new Point(x, p);
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
    }
}