using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Approximation;
using SolutionSystemEquations;

namespace TestMatrix
{
    class Program
    {
        static void Main(string[] args)
        {
            Matrix matrix = new Matrix(new double[3, 4] { {3, 2, 1, 3 }, {1, 4, 7, 3}, {9, 7, 2, 2 } });
            Matrix inverse = matrix.GetTransporse();

            Matrix res = inverse * matrix;
            Matrix res1 = res.GetInverse();
            double det = res.GetDeterminant();
            Console.Read();
        }
    }
}
