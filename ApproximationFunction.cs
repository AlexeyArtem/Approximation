using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NumericalMethods;
using MathNet.Symbolics;
using System.Security.Cryptography;

namespace Approximation
{

    public class ApproximationFunction
    {
        private static string varName = "x";
        private Dictionary<string, FloatingPoint> varDictionary;
        private SymbolicExpression expression;
        private List<Point> points;

        public ApproximationFunction(List<Point> points)
        {
            this.points = points;
        }

        public ApproximationFunction(string function) 
        {
            expression = SymbolicExpression.Parse(function);
            varDictionary = new Dictionary<string, FloatingPoint>();
            varDictionary.Add(varName, 0);
            points = new List<Point>();

            for (double i = 0; i < 100; i += 0.1)
            {
                points.Add(new Point(i, GetFunctionValue(i)));
            }
        }

        private double GetFunctionValue(double x) 
        {
            varDictionary[varName] = x;
            return expression.Evaluate(varDictionary).RealValue;
        }

        

        //private double GetTk(int k) 
        //{
        //    //Откуда берутся A и B в формуле
        //}

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

            for (double x = points[0].X; x <= points[points.Count - 1].X; x += 0.1)
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

        private double GetTi(double x, double i)
        {
            return Math.Cos(i * Math.Acos(x));
        }

        private double GetXk(int k, int n)
        {
            return Math.Cos((k + 0.5) / n * Math.PI);
        }

        private double GetTk(double x, int k, int n)
        {
            return (0 + 100) / 2 + (0 - 100) / 2 * GetXk(k, n);
        }

        private double GetCi(double x, int i, int n)
        {
            double c = 0;

            for (int k = 0; k < n-1; k++)
            {
                c += GetFunctionValue(GetTk(x, k, n)) * GetTi(GetXk(k, n), i);
            }

            c *= 2 / n;

            return c;
        }

        private double GetTn(double x, int n)
        {
            return 1 / 2 * (Math.Pow(x + Math.Sqrt(x * x - 1), n) + Math.Pow(x - Math.Sqrt(x * x - 1), n));
        }

        public List<Point> ChebyshevPolynomial(string function, int n) 
        {
            List<Point> resultPoints = new List<Point>();

            for (double x = points[0].X; x <= points[points.Count - 1].X; x += 0.1)
            {
                double fx = 0;
                for (int i = 0; i < n - 1; i++)
                {
                    fx += GetCi(x, i, n) * GetTn(x, i);
                }
                fx -= GetCi(x, 0, n) / 2;
                resultPoints.Add(new Point(x, fx));
            }

            return resultPoints;
        }

        public List<Point> TaylorsRow(string function, int degree)
        {
            List<Point> resultPoints = new List<Point>();

            return resultPoints;
        }
    }
    
}
