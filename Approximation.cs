using SolutionSystemEquations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Approximation
{

    public class Approximattion
    {
        private List<Point> points;

        public Approximattion(List<Point> points)
        {
            this.points = points;
        }

        public List<Point> MethodOfMinimumRoots(int degree)
        {
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

            Matrix<double> argsM = CreateMatrix.DenseOfArray(arguments);
            Matrix<double> valuesM = CreateMatrix.DenseOfArray(values);
            Matrix<double> argsT = argsM.Transpose();
            Matrix<double> coefs = (argsT * argsM).Inverse() * argsT * valuesM;



            //Matrix argumentsMatrix = new Matrix(arguments);
            //Matrix valuesMatrix = new Matrix(values);
            //SystemEquations systemEquations = new SystemEquations(argumentsMatrix, valuesMatrix);
            //coefficients = systemEquations.GaussMethod();
            //Matrix temp = argumentsMatrix.GetTransporse();
            //Matrix temp1 = temp * argumentsMatrix;
            //Matrix temp2 = temp1.GetInverse();
            //Matrix temp3 = temp2 * argumentsMatrix.GetTransporse();
            //Matrix temp4 = temp3 * valuesMatrix;


            //double step = 0.1; 
            //List<Point> result;


            return points;
        }
    }
    
}
