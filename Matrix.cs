using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SolutionSystemEquations
{
    public class MatrixNotQuadraticException : Exception 
    {
        public MatrixNotQuadraticException() : base() { }
        public MatrixNotQuadraticException(string msg) : base(msg) { }
        public MatrixNotQuadraticException(string msg, SystemException inner) : base(msg, inner) { }
        public MatrixNotQuadraticException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class DeterminantIsNullException : Exception
    {
        public DeterminantIsNullException() : base() { }
        public DeterminantIsNullException(string msg) : base(msg) { }
        public DeterminantIsNullException(string msg, SystemException inner) : base(msg, inner) { }
        public DeterminantIsNullException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class MatrixSizesMismatchException : Exception
    {
        public MatrixSizesMismatchException() : base() { }
        public MatrixSizesMismatchException(string msg) : base(msg) { }
        public MatrixSizesMismatchException(string msg, SystemException inner) : base(msg, inner) { }
        public MatrixSizesMismatchException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public class Matrix
    {
        private readonly double[,] values;

        public bool IsQuadraticity => CheckQuadraticity();
        public int Rows => values.GetLength(0);
        public int Columns => values.GetLength(1);
        public int Lenth => values.Length;

        public double this[int i, int j]
        {
            get
            {
                return values[i, j];
            }
            private set 
            {
                values[i, j] = value;
            }
        }

        public Matrix(double[,] values)
        {
            this.values = CopyMatrixValues(values);
        }

        private bool CheckQuadraticity()
        {
            if (values.GetLength(0) == values.GetLength(1)) return true;
            else return false;
        }

        private double[,] CopyMatrixValues(double[,] values)
        {
            double[,] copyValues = new double[values.GetLength(0), values.GetLength(1)];
            Array.Copy(values, copyValues, values.Length);

            return copyValues;
        }

        private double[,] CopyMatrixValues(Matrix matrix)
        {
            double[,] copyValues = new double[matrix.Rows, matrix.Columns];
            for (int i = 0; i < copyValues.GetLength(0); i++)
            {
                for (int j = 0; j < copyValues.GetLength(1); j++)
                {
                    copyValues[i, j] = matrix[i, j];
                }
            }

            return copyValues;
        }

        private double GetAlgebraicExtra(double[,] matrix, int indexRow, int indexColumn)
        {
            if (!IsQuadraticity) throw new MatrixNotQuadraticException("Матрица не является квадратичной.");

            
            if (indexRow > matrix.GetLength(0) || indexColumn > matrix.GetLength(1) || indexRow < 1 || indexColumn < 1)
                throw new IndexOutOfRangeException();

            double[,] minor = new double[matrix.GetLength(0) - 1, matrix.GetLength(1) - 1];

            int row, col;
            row = col = 0;

            for (int i = 0; i < matrix.GetLength(1); i++)
            {
                for (int j = 0; j < matrix.GetLength(0); j++)
                {
                    if (i + 1 != indexRow && j + 1 != indexColumn)
                    {
                        minor[row, col] = matrix[i, j];
                        col++;
                    }
                }
                col = 0;
                if (i + 1 != indexRow) row++;
            }
            Matrix m = new Matrix(minor);
            double algExtra = m.GetDeterminant() * Math.Pow(-1, indexRow + indexColumn);

            return algExtra;
        }

        public Matrix GetInverse()
        {
            //if (!IsQuadraticity) throw new MatrixNotQuadraticException("Матрица не является квадратичной.");
            
            double det = GetDeterminant();
            if (det == 0) throw new DeterminantIsNullException("Детерминант равен нулю");

            double[,] extraMatrix = new double[Rows, Columns];
            
            for (int i = 0; i < extraMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < extraMatrix.GetLength(1); j++)
                {
                    extraMatrix[i, j] = GetAlgebraicExtra(values, i + 1, j + 1);
                }
            }

            double koef = 1 / det;
            Matrix m = new Matrix(extraMatrix);

            return m.GetTransporse() * koef;
        }

        public Matrix GetTriangular() 
        {
            if (!IsQuadraticity) throw new MatrixNotQuadraticException("Матрица не является квадратичной.");

            double[,] values = CopyMatrixValues(this.values);

            for (int i = 0; i < values.GetLength(0) - 1; i++)
            {
                for (int j = i + 1; j < values.GetLength(1); j++)
                {
                    double coef;
                    if (values[i, i] == 0)
                    {
                        for (int k = 0; k < values.GetLength(1); k++)
                        {
                            values[i, k] += values[i + 1, k];
                        }
                        coef = values[j, i] / values[i, i];
                    }
                    else coef = values[j, i] / values[i, i];

                    for (int k = i; k < values.GetLength(0); k++)
                        values[j, k] -= values[i, k] * coef;
                }
            }

            //Проверка на NaN
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    if (double.IsNaN(values[i, j])) values[i, j] = 0;
                }
            }

            return new Matrix(values);
        }

        private void CheckNaN(double[,] values) 
        {
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    if (double.IsNaN(values[i, j])) values[i, j] = 0;
                }
            }
        }

        public Matrix GetTransporse()
        {
            //if (!IsQuadraticity) throw new MatrixNotQuadraticException("Матрица не является квадратичной.");

            //double[,] values = CopyMatrixValues(this.values);
            //double num;
            //for (int i = 0; i < values.GetLength(0); i++)
            //{
            //    for (int j = 0; j < i; j++)
            //    {
            //        num = values[i, j];
            //        values[i, j] = values[j, i];
            //        values[j, i] = num;
            //    }
            //}

            double[,] values = new double[this.values.GetLength(1), this.values.GetLength(0)];
            
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    values[i, j] = this.values[j, i];
                }
            }

            return new Matrix(values);
        }

        public double[,] GetValues() 
        {
            return values.Clone() as double[,];
        }

        public double GetDeterminant()
        {
            if (!IsQuadraticity) throw new MatrixNotQuadraticException("Матрица не является квадратичной.");

            Matrix m = GetTriangular();
            double det = 1;

            for (int i = 0; i < values.GetLength(0); i++)
            {
                det = det * m[i, i];
            }

            return det;
        }

        public double GetFirstNorm() 
        {
            double norm = 0;
            for (int i = 0; i < Rows; i++)
            {
                double sumRow = 0;
                for (int j = 0; j < Columns; j++)
                {
                    sumRow += Math.Abs(values[i, j]);
                }
                if (norm < sumRow) norm = sumRow;
            }

            return norm;
        }

        public static Matrix operator*(Matrix matrix, double num) 
        {
            double[,] values = new double[matrix.Rows, matrix.Columns];
            for (int i = 0; i < values.GetLength(0); i++)
            {
                for (int j = 0; j < values.GetLength(1); j++)
                {
                    values[i, j] = matrix[i, j] * num;
                }
            }

            return new Matrix(values);
        }

        public static Matrix operator*(Matrix matrixA, Matrix matrixB) 
        {
            if (matrixA.Columns != matrixB.Rows) throw new Exception("Количество строк и столбцов перемножаемых матриц не совпадают.");
            
            double[,] result = new double[matrixA.Rows, matrixB.Columns];

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    for (int k = 0; k < matrixA.Columns; k++)
                    {
                        result[i, j] += matrixA[i, k] * matrixB[k, j];
                    }
                }
            }

            return new Matrix(result);
        }

        public static double[,] Multiplication(double[,] a, double[,] b)
        {
            double[,] r = new double[a.GetLength(0), b.GetLength(1)];
            Parallel.For(0, a.GetLength(0), (i) =>
            {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    for (int k = 0; k < b.GetLength(0); k++)
                    {
                        r[i, j] += a[i, k] * b[k, j];
                    }
                }
            });
            return r;
        }

        public static Matrix operator -(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA.Columns != matrixB.Columns || matrixA.Rows != matrixB.Rows) throw new MatrixSizesMismatchException("Матрицы не совпадают по размеру.");

            double[,] result = new double[matrixA.Rows, matrixB.Columns];

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = matrixA[i, j] - matrixB[i, j];
                }
            }

            return new Matrix(result);
        }

        public static Matrix operator +(Matrix matrixA, Matrix matrixB)
        {
            if (matrixA.Columns != matrixB.Columns || matrixA.Rows != matrixB.Rows) throw new MatrixSizesMismatchException("Матрицы не совпадают по размеру.");

            double[,] result = new double[matrixA.Rows, matrixB.Columns];

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = matrixA[i, j] + matrixB[i, j];
                }
            }

            return new Matrix(result);
        }


    }
}
