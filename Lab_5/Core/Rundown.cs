namespace Lab_5.Core
{
    public class Rundown
    {
        public double[] Execute (MatExt input)
        {
            MatExt PQ = FindPQ(input);
            return Solve(PQ);
        }
        private MatExt FindPQ (MatExt input)
        {
            int size = input.A.GetLength(0);
            MatExt PQ = new()
            {
                A = CreateEmpty(size, 1),
                B = CreateEmpty(size, 1)
            };
            for (int i = 0; i < size; i++)
            {
                if (i == 0)
                {
                    PQ.A[i, 0] = -input.A[i, 2] / input.A[i, 1];
                    PQ.B[i, 0] = input.B[i, 0] / input.A[i, 1];
                }
                else
                {
                    PQ.A[i, 0] = -input.A[i, 2] / (input.A[i, 1] + input.A[i, 0] * PQ.A[i - 1, 0]);
                    PQ.B[i, 0] = (input.B[i, 0] - input.A[i, 0] * PQ.B[i - 1, 0]) / (input.A[i, 1] + input.A[i, 0] * PQ.A[i - 1, 0]);
                }
            }
            return PQ;
        }
        private double[] Solve (MatExt PQ)
        {
            int size = PQ.A.GetLength(0);
            double[] X = new double[size];
            for (int i = size - 1; i >= 0; i--)
            {
                if (i == size - 1)
                {
                    X[i] = PQ.B[i, 0];
                }
                else
                {
                    X[i] = PQ.A[i, 0] * X[i + 1] + PQ.B[i, 0];
                }
            }
            return X;
        }
        public static double[,] CreateEmpty (int rows, int columns)
        {
            double[,] A = new double[rows, columns];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    A[i, j] = 0.0f;
                }
            }
            return A;
        }
    }
}
