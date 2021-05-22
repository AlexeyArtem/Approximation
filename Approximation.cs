using SolutionSystemEquations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Approximation
{

    public class Approximattion
    {
        private List<Point> points;

        public Approximattion(List<Point> points)
        {
            this.points = points;
        }

        public List<Point> MethodOfMinimumRoots(int degree, double h)
        {
            List<Point> resultPoints = new List<Point>();
            // XA=Y
            double[,] coefficients; // A
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
;

            Matrix argumentsMatrix = new Matrix(arguments);
            Matrix valuesMatrix = new Matrix(values);

            Matrix first = (argumentsMatrix.GetTransporse() * argumentsMatrix).GetInverse();
            Matrix second = argumentsMatrix.GetTransporse() * valuesMatrix;
            Matrix A = first * second;

            for (double x = points[0].X; x <= points[points.Count - 1].X; x += 1)
            {
                double y = 0;
                for (int i = 0; i <= degree; i++)
                {
                    y += A[i, 0] * Math.Pow(x, i);
                }
                resultPoints.Add(new Point(x, y));
            }


            //SystemEquations systemEquations1 = new SystemEquations(argumentsMatrix, valuesMatrix);

            //double[,] result = systemEquations1.MatrixMethod();

            //for (double x = points[0].X; x <= points[points.Count - 1].X; x += 1)
            //{
            //    double y = 0;
            //    for (int i = 0; i <= degree; i++)
            //    {
            //        y += result[i, 0] * Math.Pow(x, i);
            //    }
            //    resultPoints.Add(new Point(x, y));
            //}
            return resultPoints;
            //A=(Xt*X)^-1*Xt*Y

            //Matrix temp = argumentsMatrix.GetTransporse();
            //Matrix temp1 = temp * argumentsMatrix;
            //Matrix temp2 = temp1.GetInverse();

            //Matrix temp3 = temp2 * argumentsMatrix.GetTransporse();
            //Matrix temp4 = temp3 * valuesMatrix;


            //SystemEquations systemEquations = new SystemEquations(argumentsMatrix.GetTransporse() * argumentsMatrix, argumentsMatrix.GetTransporse() * valuesMatrix);
            //double[,] result = systemEquations.MatrixMethod();

            double step = 0.1; 
            //List<Point> result;


            return points;
        }
    }
    
}
