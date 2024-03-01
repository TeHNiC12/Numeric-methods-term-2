namespace Course_project
{
    public class ConjugateGradient
    {
        public Tuple<double[,], int> Solve (double[,] A, double[,] b, double[,]? x0, double epsilon)
        {
            if (!CheckSymmetry(A))
            {
                throw new Exception("Matrix isn't symmentrical");
            }
            int k;
            double[,] X0;
            if (x0 == null)
            {
                X0 = new double[b.GetLength(0), 1];
            }
            else
            {
                X0 = x0;
            }

            double[,] xPrev;
            double[,] rPrev;
            double[,] pPrev;

            double[,] xCur;
            double[,] rCur;
            double[,] pCur;

            xCur = X0;
            rCur = Subtract(b, Multiply(A, X0));
            pCur = Subtract(b, Multiply(A, X0));

            for (k = 1; k > 0; k++)
            {
                xPrev = xCur;
                rPrev = rCur;
                pPrev = pCur;

                double alpha = Multiply(Transpose(rPrev), rPrev)[0, 0] / Multiply(Transpose(Multiply(A, pPrev)), pPrev)[0, 0];
                xCur = Add(xPrev, Multiply(alpha, pPrev));
                rCur = Subtract(rPrev, Multiply(Multiply(alpha, A), pPrev));
                double beta = Multiply(Transpose(rCur), rCur)[0, 0] / Multiply(Transpose(rPrev), rPrev)[0, 0];
                pCur = Add(rCur, Multiply(beta, pPrev));

                double condition = NormAc(rCur) / NormAc(b);
                if (condition < epsilon)
                {
                    break;
                }
                else if (k > 1000000)
                {
                    throw new Exception("Уравнение не сходится");
                }
            }
            return new Tuple<double[,], int>(xCur, k);
        }

        public double[,] Add (double[,] A, double[,] B)
        {
            if ((A.GetLength(0) == B.GetLength(0)) & (A.GetLength(1) == B.GetLength(1)))
            {
                int rows = A.GetLength(0);
                int columns = A.GetLength(1);
                double[,] C = new double[rows, columns];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        C[i, j] = A[i, j] + B[i, j];
                    }
                }
                return C;
            }
            else
            {
                throw new Exception("Can't add matrices: size does not match");
            }
        }
        public double[,] Subtract (double[,] A, double[,] B)
        {
            if ((A.GetLength(0) == B.GetLength(0)) & (A.GetLength(1) == B.GetLength(1)))
            {
                int rows = A.GetLength(0);
                int columns = A.GetLength(1);
                double[,] C = new double[rows, columns];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        C[i, j] = A[i, j] - B[i, j];
                    }
                }
                return C;
            }
            else
            {
                throw new Exception("Can't subtract matrices: size does not match");
            }
        }
        private double[,] Multiply (double[,] A, double[,] B)
        {
            if (A.GetLength(1) == B.GetLength(0))
            {
                int rows = A.GetLength(0);
                int columns = B.GetLength(1);
                int n = A.GetLength(1);
                double[,] C = new double[rows, columns];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        for (int k = 0; k < n; k++)
                        {
                            C[i, j] += A[i, k] * B[k, j];
                        }
                    }
                }
                return C;
            }
            else
            {
                throw new Exception("Can't multiply matrices: A columns amount doesn't match B rows count");
            }
        }
        private double[,] Multiply (double c, double[,] A)
        {
            int rows = A.GetLength(0);
            int columns = A.GetLength(1);
            double[,] B = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    B[i, j] = A[i, j] * c;
                }
            }
            return B;
        }
        public double[,] Transpose (double[,] A)
        {
            int rows = A.GetLength(0);
            int columns = A.GetLength(1);
            double[,] A_T = new double[columns, rows];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    A_T[j, i] = A[i, j];
                }
            }
            return A_T;
        }
        public bool CheckSymmetry (double[,] A)
        {
            if (Compare(A, Transpose(A)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Compare (double[,] A, double[,] B)
        {
            if ((A.GetLength(0) == B.GetLength(0)) & (A.GetLength(1) == B.GetLength(1)))
            {
                int rows = A.GetLength(0);
                int columns = A.GetLength(1);
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        if (A[i, j] != B[i, j])
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public double NormAc (double[,] A)
        {
            double norm = 0;
            for (int i = 0; i < A.GetLength(0); i++)
            {
                double sum = 0;
                for (int j = 0; j < A.GetLength(1); j++)
                {
                    sum += Math.Abs(A[i, j]);
                }
                if (sum > norm)
                {
                    norm = sum;
                }
            }
            return norm;
        }
    }
}
