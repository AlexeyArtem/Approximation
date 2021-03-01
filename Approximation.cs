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

        public List<Point> MethodOfMinimumRoots(int degree)
        {
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

            Matrix argumentsMatrix = new Matrix(arguments);
            Matrix valuesMatrix = new Matrix(values);
            //SystemEquations systemEquations = new SystemEquations(argumentsMatrix, valuesMatrix);
            //coefficients = systemEquations.MatrixMethod();

            Matrix temp = (argumentsMatrix.GetTransporse() * argumentsMatrix).GetInverse() * argumentsMatrix.GetTransporse() * valuesMatrix;
            //temp = temp.GetTransporse();
            
            double step = 0.1; 
            List<Point> result;


            return points;
        }
    }
    
}
