using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NumericalMethods;
using MathNet.Symbolics;

namespace Approximation
{

    public class ApproximationFunction
    {
        private static string varName = "x";
        private Dictionary<string, FloatingPoint> varDictionary;
        private SymbolicExpression expression;

        public ApproximationFunction(string function) 
        {
            expression = SymbolicExpression.Parse(function);
            varDictionary = new Dictionary<string, FloatingPoint>();
            varDictionary.Add(varName, 0);
        }

        private double GetFunctionValue(double x) 
        {
            varDictionary[varName] = x;
            return expression.Evaluate(varDictionary).RealValue;
        }

        private double GetXk(int k, int n) 
        {
            return Math.Cos((k + 0.5) / n * Math.PI);
        }

        private double GetTk(int k) 
        {
            //Откуда берутся A и B в формуле
        }

        public List<Point> ChebyshevPolynomial(string function) 
        {
            List<Point> resultPoints = new List<Point>();

            return resultPoints;
        }

        public List<Point> TaylorsRow(string function, int degree)
        {
            List<Point> resultPoints = new List<Point>();

            return resultPoints;
        }
    }
    
}
