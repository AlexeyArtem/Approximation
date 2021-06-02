using MathNet.Symbolics;
//using NumericalIntegration;
using NumericalMethods;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Approximation
{
    public class ApproximationFunction
    {
        private static string varName = "x";
        private Dictionary<string, FloatingPoint> varDictionary;
        private SymbolicExpression expression;
        private string function;


        public ApproximationFunction(string function) 
        {
            expression = SymbolicExpression.Parse(function);
            varDictionary = new Dictionary<string, FloatingPoint>();
            varDictionary.Add(varName, 0);
            this.function = function;
        }

        private double GetFunctionValue(double x) 
        {
            varDictionary[varName] = x;
            return expression.Evaluate(varDictionary).RealValue;
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
            double xk, tk, ti, funcValue;
            for (int k = 0; k < n-1; k++)
            {
                xk = GetXk(k, n);
                tk = GetTk(xk, a, b);
                ti = GetTi(xk, i);
                funcValue = GetFunctionValue(tk);
                c += funcValue * ti;
            }

            c *= 2 / (double)n;

            return c;
        }

        private double GetTn(double x, int n)
        {

            double sum1 = Math.Pow(x + Math.Sqrt(x * x - 1), n);
            double sum2 = Math.Pow(x - Math.Sqrt(x * x - 1), n);
            double sum = sum1 + sum2;

            double res = 0.5 * sum;
            return res;
        }

        public List<Point> ChebyshevPolynomial(int degree, double leftBorder, double rightBorder, double step) 
        {
            if (rightBorder < leftBorder) throw new Exception("Конечная граница должна быть больше начальной");
            if (degree <= 0) throw new Exception("Степень должна быть больше нуля");

            List<double> valuesX = new List<double>();
            for (double i = leftBorder; i <= rightBorder; i += step)
                valuesX.Add(i);

            List<Point> resultPoints = new List<Point>();
            for (int i = 0; i < valuesX.Count; i++)
            {
                double fx = 0;
                double ci, tn;

                for (int j = 0; j < degree - 1; j++)
                {
                    ci = GetCi(valuesX[i], j, degree, leftBorder, rightBorder);
                    tn = GetTn(valuesX[i], j);
                    fx += ci * tn;
                }
                fx -= GetCi(valuesX[i], 0, degree, leftBorder, rightBorder) / 2;
                resultPoints.Add(new Point(valuesX[i], fx));
            }

            return resultPoints;
        }

        public List<Point> TaylorsRow(int degree, double area, double areaLimit, double step)
        {
            if (degree > 5) throw new Exception("Степень не может быть больше 5");

            List<Point> resultPoints = new List<Point>();
            List<Point> values = new List<Point>();
            for (double i = area - areaLimit; i <= area + areaLimit; i += step)
                values.Add(new Point(i, GetFunctionValue(i)));

            Derivative der = new Derivative(values);
            List<Point> res1 = der.QuadraticInterpolation(1).DerivativePoints;
            List<Point> res2 = der.QuadraticInterpolation(2).DerivativePoints;
            List<Point> res3 = der.QuadraticInterpolation(3).DerivativePoints;
            List<Point> res4 = der.QuadraticInterpolation(4).DerivativePoints;
            List<Point> res5 = der.QuadraticInterpolation(5).DerivativePoints;

            for (int i = 0; i < values.Count - 15; i++)
            {
                double sum = 0;

                for (int j = 0; j <= degree; j++)
                {
                    if (j == 0) sum = values[i].Y;
                    if (j == 1) sum += res1[i].Y;
                    if (j == 2) sum += res2[i].Y;
                    if (j == 3) sum += res3[i].Y;
                    if (j == 4) sum += res4[i].Y;
                    if (j == 5) sum += res5[i].Y;
                }
                resultPoints.Add(new Point(values[i].X, sum));
            }

            return resultPoints;
        }

        public List<Point> FourierRows(int degree, double leftBorder, double rightBorder, double step)
        {
            List<Point> resultPoints = new List<Point>();
            
            List<double> valuesX = new List<double>();
            for (double i = leftBorder; i <= rightBorder; i += step)
                valuesX.Add(i);

            for (int i = 0; i < valuesX.Count; i++)
            {
                double sum = 0;
                for (int j = 1; j <= degree; j++)
                {
                    DefineIntergral intergralA = new DefineIntergral(function + "*cos(" + j + "*x)", -3.14, 3.14);
                    DefineIntergral intergralB = new DefineIntergral(function + "*sin(" + j + "*x)", -3.14, 3.14);
                    sum += 1 / Math.PI * intergralA.MethodRectangle(BorderMethodRectangle.Left, step) * Math.Cos(j * valuesX[i]);
                    sum += 1 / Math.PI * intergralB.MethodRectangle(BorderMethodRectangle.Left, step) * Math.Sin(j * valuesX[i]);
                }
                DefineIntergral intergralA0 = new DefineIntergral(function, -3.14, 3.14);
                sum += 1 / Math.PI * intergralA0.MethodRectangle(BorderMethodRectangle.Left, step);
                resultPoints.Add(new Point(valuesX[i], sum));
            }
            
            return resultPoints;
        }
    } 
}
