using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SolutionSystemEquations
{
    public enum Method
    {
        Iteration,
        Seidel
    }

    public class MatrixSetIncorrectlyException : Exception
    {
        public MatrixSetIncorrectlyException() : base() { }
        public MatrixSetIncorrectlyException(string msg) : base(msg) { }
        public MatrixSetIncorrectlyException(string msg, SystemException inner) : base(msg, inner) { }
        public MatrixSetIncorrectlyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }


    public class SystemEquations
    {
        public readonly double Accuracy = 0.01;
        private Matrix A;
        private Matrix B;

        private Dictionary<Method, int> iterations = new Dictionary<Method, int>();
        private Dictionary<Method, double> lastValues = new Dictionary<Method, double>();

        public int CountVariables => B.Rows;

        public SystemEquations(Matrix A, Matrix B)
        {
            if (!A.IsQuadraticity || B.Rows != A.Rows || B.Columns != 1)
                throw new MatrixSetIncorrectlyException("Неверный ввод матрицы коэффициентов или вектора свободных членов.");

            lastValues[Method.Iteration] = 0;
            lastValues[Method.Seidel] = 0;
            iterations[Method.Iteration] = 0;
            iterations[Method.Seidel] = 0;

            this.A = A;
            this.B = B;
        }

        public SystemEquations(Matrix A, Matrix B, double accuracy)
        {
            if (!A.IsQuadraticity || B.Rows != A.Rows || B.Columns != 1)
                throw new MatrixSetIncorrectlyException("Неверный ввод матрицы коэффициентов или вектора свободных членов.");

            Accuracy = accuracy;

            lastValues[Method.Iteration] = 0;
            lastValues[Method.Seidel] = 0;
            iterations[Method.Iteration] = 0;
            iterations[Method.Seidel] = 0;

            this.A = A;
            this.B = B;
        }

        public double GetLastValueIteration(Method method)
        {
            switch (method)
            {
                case Method.Iteration:
                    return lastValues[Method.Iteration];
                case Method.Seidel:
                    return lastValues[Method.Seidel];
                default:
                    return 0;
            }
        }

        public int GetCountIteration(Method method)
        {
            switch (method)
            {
                case Method.Iteration:
                    return iterations[Method.Iteration];
                case Method.Seidel:
                    return iterations[Method.Seidel];
                default:
                    return 0;
            }
        }

        public double[,] MatrixMethod()
        {
            Matrix X = A.GetInverse() * B;

            return X.GetValues();
        }

        private double[,] MatrixMethod(Matrix A, Matrix B)
        {
            Matrix X = A.GetInverse() * B;

            return X.GetValues();
        }

        public double[,] MethodKramer()
        {
            double det = A.GetDeterminant();
            double[,] X = new double[CountVariables, 1];

            for (int j = 0; j < A.Columns; j++)
            {
                double[,] values = A.GetValues();
                for (int i = 0; i < B.Rows; i++)
                {
                    values[i, j] = B[i, 0];
                }

                Matrix m = new Matrix(values);
                double d = m.GetDeterminant();
                X[j, 0] = d / det;
            }

            return X;
        }

        public double[,] MethodLU()
        {
            double[,] matrixLU = A.GetValues();

            //Факторизация
            for (int k = 0; k <= CountVariables; k++)
            {
                int j, i;
                j = k;
                double sum;
                for (i = k; i < matrixLU.GetLength(0); i++)
                {
                    sum = 0;
                    for (int s = 0; s <= j - 1; s++)
                    {
                        sum += matrixLU[i, s] * matrixLU[s, j];
                    }

                    matrixLU[i, j] = matrixLU[i, j] - sum;
                }

                i = k;
                for (j = k + 1; j < matrixLU.GetLength(1); j++)
                {
                    sum = 0;
                    for (int s = 0; s <= i - 1; s++)
                    {
                        sum += matrixLU[i, s] * matrixLU[s, j];
                    }

                    matrixLU[i, j] = 1 / matrixLU[i, i] * (matrixLU[i, j] - sum);
                }
            }

            double[,] matrixL = new double[matrixLU.GetLength(0), matrixLU.GetLength(1)];
            double[,] matrixU = new double[matrixLU.GetLength(0), matrixLU.GetLength(1)];

            for (int i = 0; i < matrixLU.GetLength(0); i++)
            {
                for (int j = 0; j < matrixLU.GetLength(1); j++)
                {
                    if (i >= j)
                    {
                        if (i == j) matrixU[i, j] = 1;
                        matrixL[i, j] = matrixLU[i, j];
                    }
                    else matrixU[i, j] = matrixLU[i, j];
                }
            }

            //Нахождение решения
            double[,] y = MatrixMethod(new Matrix(matrixL), B);
            double[,] x = MatrixMethod(new Matrix(matrixU), new Matrix(y));

            return x;
        }

        public double[,] GaussMethod()
        {
            double s;
            double[,] x = new double[CountVariables, 1];
            double[,] valuesA = A.GetValues();
            double[,] valuesB = B.GetValues();

            for (int k = 0; k < CountVariables - 1; k++)
            {
                for (int i = k + 1; i < CountVariables; i++)
                {
                    for (int j = k + 1; j < CountVariables; j++)
                    {
                        valuesA[i, j] = valuesA[i, j] - valuesA[k, j] * (valuesA[i, k] / valuesA[k, k]);
                    }

                    valuesB[i, 0] = valuesB[i, 0] - valuesB[k, 0] * valuesA[i, k] / valuesA[k, k];
                }
            }

            for (int k = CountVariables - 1; k >= 0; k--)
            {
                s = 0;
                for (int j = k + 1; j < CountVariables; j++)
                    s = s + valuesA[k, j] * x[j, 0];
                x[k, 0] = (valuesB[k, 0] - s) / valuesA[k, k];
            }

            return x;
        }

        public double[,] GausGordanMethod()
        {
            double[,] matrixA = A.GetValues();
            double[,] matrixB = B.GetValues();

            for (int k = 0; k < A.Rows - 1; k++)
            {
                for (int i = k + 1; i < A.Rows; i++)
                {
                    for (int j = k + 1; j < A.Rows; j++)
                    {
                        matrixA[i, j] = matrixA[i, j] - matrixA[k, j] * (matrixA[i, k] / matrixA[k, k]);
                    }
                    matrixB[i, 0] = matrixB[i, 0] - matrixB[k, 0] * matrixA[i, k] / matrixA[k, k];
                }
            }

            for (int i = A.Rows - 1; i >= 0; i--)
            {
                for (int j = A.Rows - 1; j >= 0; j--)
                {
                    if (i > j) matrixA[i, j] = 0;
                    if (i == j) matrixB[i, 0] = matrixB[i, 0] / matrixA[i, i];
                    if (i == j || j > i) matrixA[i, j] = matrixA[i, j] / matrixA[i, i];
                }
            }

            for (int k = A.Rows - 1; k > 0; k--)
            {
                for (int i = k - 1; i >= 0; i--)
                {
                    for (int j = i; j >= 0; j--)
                    {
                        matrixA[i, j] = matrixA[i, j] - matrixA[k, j] * (matrixA[i, k] / matrixA[k, k]);
                    }
                    matrixB[i, 0] = matrixB[i, 0] - matrixB[k, 0] * matrixA[i, k] / matrixA[k, k];
                }
            }

            return matrixB;
        }

        public double[,] SquareRootsMethod()
        {
            double[,] matrixU = new double[CountVariables, CountVariables];

            for (int i = 0; i < CountVariables; i++)
            {
                for (int j = 0; j < CountVariables; j++)
                {
                    if (i == j)
                    {
                        double sum = 0;
                        for (int k = 0; k < i; k++)
                        {
                            sum += Math.Pow(matrixU[k, i], 2);
                        }
                        matrixU[i, j] = Math.Sqrt(A[i, j] - sum);
                    }
                    else if (i < j)
                    {
                        double sum = 0;
                        for (int k = 0; k < i; k++)
                        {
                            sum += matrixU[k, i] * matrixU[k, j];
                        }

                        matrixU[i, j] = (A[i, j] - sum) / matrixU[i, i];
                    }
                }
            }

            Matrix U = new Matrix(matrixU).GetTransporse();
            double[,] y = MatrixMethod(U, B);
            double[,] x = MatrixMethod(U, new Matrix(y));

            return x;
        }

        public double[,] MethodSimpleIteration()
        {
            if (A.GetDeterminant() == 0) throw new DeterminantIsNullException("Детерминант равен нулю.");

            Matrix inverseA = A.GetInverse();

            double[,] valuesE = new double[inverseA.Rows, inverseA.Columns];
            for (int i = 0; i < valuesE.GetLength(0); i++)
            {
                for (int j = 0; j < valuesE.GetLength(1); j++)
                {
                    valuesE[i, j] = Accuracy;
                }
            }

            Matrix e = new Matrix(valuesE);
            Matrix D = inverseA - e;

            Matrix alpha = e * A;
            Matrix betta = D * B;

            Dictionary<int, double[,]> results = new Dictionary<int, double[,]>();
            results.Add(0, betta.GetValues()); //Задаем значения X на нулевой итерации

            double norm = 0;
            int k = 0;
            do
            {
                Matrix x = new Matrix(results[k]);
                Matrix currentX = alpha * x + betta;
                k++;
                results.Add(k, currentX.GetValues()); //добавление новых X в таблицу результатов

                norm = Math.Abs((currentX - x).GetFirstNorm());
            }
            while (norm > Accuracy);

            iterations[Method.Iteration] = k;
            lastValues[Method.Iteration] = norm;
            return results[results.Count - 1];
        }

        public double[,] MethodSeidel() 
        {
            if (A.GetDeterminant() == 0) throw new DeterminantIsNullException("Детерминант равен нулю.");

            Matrix inverseA = A.GetInverse();

            double[,] valuesE = new double[inverseA.Rows, inverseA.Columns];
            for (int i = 0; i < valuesE.GetLength(0); i++)
            {
                for (int j = 0; j < valuesE.GetLength(1); j++)
                {
                    valuesE[i, j] = Accuracy;
                }
            }

            Matrix e = new Matrix(valuesE);
            Matrix D = inverseA - e;

            Matrix alpha = e * A;
            Matrix betta = D * B;

            Dictionary<int, double[,]> results = new Dictionary<int, double[,]>();
            results.Add(0, betta.GetValues()); //Задаем значение X на нулевой итерации

            int k = 0;
            double norm = 0;
            do
            {
                Matrix previousX = new Matrix(results[k]);//Значения X на предыдущей итерации
                double[,] currentX = previousX.GetValues();//Переменная для хранения X на текущей итерации

                for (int i = 0; i < CountVariables; i++)
                {
                    Matrix current = new Matrix(currentX);
                    Matrix result = alpha * current + betta;//Нахождение значения X

                    currentX[i, 0] = result[i, 0];//Добавление нового найденного значения
                }
                k++;
                results.Add(k, currentX);//Добавление нового X в таблицу результатов
                norm = Math.Abs((new Matrix(currentX) - previousX).GetFirstNorm());
            }
            while (norm > Accuracy);

            iterations[Method.Seidel] = k;
            lastValues[Method.Seidel] = norm;
            return results[results.Count - 1];
        }
    }
}
