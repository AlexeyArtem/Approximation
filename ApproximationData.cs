using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NumericalMethods;

namespace Approximation
{
    class ApproximationData
    {
        private List<Point> points;

        public ApproximationData(List<Point> points)
        {
            this.points = points;
        }

        public List<Point> MethodOfMinimumRoots(int degree, double h)
        {
            if (degree <= 0) throw new Exception("Степень должна быть положительным числом");

            List<Point> resultPoints = new List<Point>();
            // XA=Y
            double[,] arguments = new double[points.Count, degree + 1]; // X
            double[,] values = new double[points.Count, 1]; // Y

            for (int i = 0; i < arguments.GetLength(0); i++)
            {
                for (int j = 0; j < arguments.GetLength(1); j++)
                {
                    arguments[i, j] = Math.Pow(points[i].X, j);
                }
            }

            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    values[i, j] = points[i].Y;
                }
            }

            Matrix argumentsMatrix = new Matrix(arguments);
            Matrix valuesMatrix = new Matrix(values);

            Matrix first = (argumentsMatrix.GetTransporse() * argumentsMatrix).GetInverse();
            Matrix second = argumentsMatrix.GetTransporse() * valuesMatrix;
            Matrix A = first * second;

            for (double x = points[0].X; x <= points[points.Count - 1].X; x += h)
            {
                double y = 0;
                for (int i = 0; i <= degree; i++)
                {
                    y += A[i, 0] * Math.Pow(x, i);
                }
                resultPoints.Add(new Point(x, y));
            }

            return resultPoints;
        }
    }
}
