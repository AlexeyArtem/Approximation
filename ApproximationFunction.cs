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

        private double GetTi(double xK, double i)
        {
            double res = Math.Cos(i * Math.Acos(xK));
            return res;
        }

        private double GetXk(int k, int n)
        {
            return Math.Cos((k + 0.5) / n * Math.PI);
        }

        private double GetTk(double xK, double a, double b)
        {
            return (b + a) / 2 + (b - a) / 2 * xK;
        }

        private double GetCi(double x, int i, int n, double a, double b)
        {
            double c = 0;

            for (int k = 0; k < n-1; k++)
            {
                c += GetFunctionValue(GetTk(GetXk(k, n), a, b)) * GetTi(GetXk(k, n), i);
            }
            c *= 2 / n;

            return c;
        }

        private double GetTn(double x, int n)
        {
            return 1 / 2 * (Math.Pow(x + Math.Sqrt(x * x - 1), n) + Math.Pow(x - Math.Sqrt(x * x - 1), n));
        }

        public List<Point> ChebyshevPolynomial(int degree, double a, double b, double step) 
        {
            if (b < a) throw new Exception("Конечная граница должна быть больше начальной");
            if (degree <= 0) throw new Exception("Степень должна быть больше нуля");

            List<double> valuesX = new List<double>();
            for (double i = a; i <= b; i += step)
                valuesX.Add(i);

            List<Point> resultPoints = new List<Point>();
            for (int i = 0; i < valuesX.Count; i++)
            {
                double fx = 0;
                for (int j = 0; j < degree - 1; j++)
                {
                    fx += GetCi(valuesX[i], j, degree, a, b) * GetTn(valuesX[i], j);
                }
                fx -= GetCi(valuesX[i], 0, degree, a, b) / 2;
                resultPoints.Add(new Point(valuesX[i], fx));
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
