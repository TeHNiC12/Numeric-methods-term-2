using Lab_8.Core;
using Lab_8.DefaultValues;

namespace Lab_8.MVVM.Model
{
    public class FiniteDifferenceModel
    {
        public double[,,] FiniteDifferenceSolve (int nX, int nY, int K, double T, int solver)
        {
            double hX = lX / nX;
            double hY = lY / nY;
            double thao = T / K;

            double[,,] U = new double[nX + 1, nY + 1, K + 1];
            U = FillStaringValues(U, nX, hX, nY, hY);

            if (solver == 1)
            {
                U = ChangindDirectionsScheme(U, nX, hX, nY, hY, K, thao);
            }
            else if (solver == 2)
            {
                U = PartialStepScheme(U, nX, hX, nY, hY, K, thao);
            }
            else
            {
                throw new Exception("Метод решения не выбран");
            }
            return U;
        }

        private double[,,] ChangindDirectionsScheme (double[,,] U, int nX, double hX, int nY, double hY, int K, double thao)
        {
            for (int k = 0; k <= K - 1; k++)
            {
                double[,] mU = CreateMidwayLayer(nX, hX, nY, hY, thao, k);

                for (int j = 1; j <= nY - 1; j++)
                {
                    MatExt layer = new()
                    {
                        A = new double[nX - 1, 3],
                        B = new double[nX - 1, 1]
                    };

                    layer.A[0, 1] = 1f / (0.5f * thao) + 2 * a / Math.Pow(hX, 2);
                    layer.A[0, 2] = -a / Math.Pow(hX, 2);
                    layer.B[0, 0] = (b / Math.Pow(hY, 2)) * (U[1, j - 1, k] + U[1, j + 1, k]) + (1f / (0.5f * thao) - 2 * b / Math.Pow(hY, 2)) * U[1, j, k] + F(1 * hX, j * hY, (k + 0.5f) * thao) + (a / Math.Pow(hX, 2)) * mU[0, j];

                    for (int i = 2; i <= nX - 2; i++)
                    {
                        layer.A[i - 1, 0] = -a / Math.Pow(hX, 2);
                        layer.A[i - 1, 1] = 1f / (0.5f * thao) + 2 * a / Math.Pow(hX, 2);
                        layer.A[i - 1, 2] = -a / Math.Pow(hX, 2);
                        layer.B[i - 1, 0] = (b / Math.Pow(hY, 2)) * (U[i, j - 1, k] + U[i, j + 1, k]) + (1f / (0.5f * thao) - 2 * b / Math.Pow(hY, 2)) * U[i, j, k] + F(i * hX, j * hY, (k + 0.5f) * thao);
                    }

                    layer.A[nX - 2, 0] = -a / Math.Pow(hX, 2);
                    layer.A[nX - 2, 1] = 1f / (0.5f * thao) + 2 * a / Math.Pow(hX, 2);
                    layer.B[nX - 2, 0] = (b / Math.Pow(hY, 2)) * (U[nX - 1, j - 1, k] + U[nX - 1, j + 1, k]) + (1f / (0.5f * thao) - 2 * b / Math.Pow(hY, 2)) * U[nX - 1, j, k] + F((nX - 1) * hX, j * hY, (k + 0.5f) * thao) + (a / Math.Pow(hX, 2)) * mU[nX, j];

                    double[] mUs = rundown.Execute(layer);

                    for (int i = 1; i <= nX - 1; i++)
                    {
                        mU[i, j] = mUs[i - 1];
                    }
                }

                for (int i = 0; i <= nX; i++)
                {
                    U[i, 0, k + 1] = phi3(i * hX, (k + 1) * thao);
                    U[i, nY, k + 1] = phi4(i * hX, (k + 1) * thao);
                }

                for (int j = 0; j <= nY; j++)
                {
                    U[0, j, k + 1] = phi1(j * hY, (k + 1) * thao);
                    U[nX, j, k + 1] = phi2(j * hY, (k + 1) * thao);
                }

                for (int i = 1; i <= nX - 1; i++)
                {
                    MatExt layer = new()
                    {
                        A = new double[nY - 1, 3],
                        B = new double[nY - 1, 1]
                    };

                    layer.A[0, 1] = 1f / (0.5f * thao) + 2 * b / Math.Pow(hY, 2);
                    layer.A[0, 2] = -b / Math.Pow(hY, 2);
                    layer.B[0, 0] = (a / Math.Pow(hX, 2)) * (mU[i - 1, 1] + mU[i + 1, 1]) + (1f / (0.5f * thao) - 2 * a / Math.Pow(hX, 2)) * mU[i, 1] + F(i * hX, 1 * hY, (k + 0.5f) * thao) + (b / Math.Pow(hY, 2)) * U[i, 0, k + 1];

                    for (int j = 2; j <= nY - 2; j++)
                    {
                        layer.A[j - 1, 0] = -b / Math.Pow(hY, 2);
                        layer.A[j - 1, 1] = 1f / (0.5f * thao) + 2 * b / Math.Pow(hY, 2);
                        layer.A[j - 1, 2] = -b / Math.Pow(hY, 2);
                        layer.B[j - 1, 0] = (a / Math.Pow(hX, 2)) * (mU[i - 1, j] + mU[i + 1, j]) + (1f / (0.5f * thao) - 2 * a / Math.Pow(hX, 2)) * mU[i, j] + F(i * hX, j * hY, (k + 0.5f) * thao);
                    }

                    layer.A[nY - 2, 0] = -b / Math.Pow(hY, 2);
                    layer.A[nY - 2, 1] = 1f / (0.5f * thao) + 2 * b / Math.Pow(hY, 2);
                    layer.B[nY - 2, 0] = (a / Math.Pow(hX, 2)) * (mU[i - 1, nY - 1] + mU[i + 1, nY - 1]) + (1f / (0.5f * thao) - 2 * a / Math.Pow(hX, 2)) * mU[i, nY - 1] + F(i * hX, (nY - 1) * hY, (k + 0.5f) * thao) + (b / Math.Pow(hY, 2)) * U[i, nY, k + 1];

                    double[] Us = rundown.Execute(layer);

                    for (int j = 1; j <= nY - 1; j++)
                    {
                        U[i, j, k + 1] = Us[j - 1];
                    }
                }
            }
            return U;
        }

        private double[,,] PartialStepScheme (double[,,] U, int nX, double hX, int nY, double hY, int K, double thao)
        {
            for (int k = 0; k <= K - 1; k++)
            {
                double[,] mU = CreateMidwayLayer(nX, hX, nY, hY, thao, k);

                for (int j = 1; j <= nY - 1; j++)
                {
                    MatExt layer = new()
                    {
                        A = new double[nX - 1, 3],
                        B = new double[nX - 1, 1]
                    };

                    layer.A[0, 1] = 1f / thao + 2 * a / Math.Pow(hX, 2);
                    layer.A[0, 2] = -a / Math.Pow(hX, 2);
                    layer.B[0, 0] = (1f / thao) * U[1, j, k] + F(1 * hX, j * hY, k * thao) / 2f + (a / Math.Pow(hX, 2)) * mU[0, j];

                    for (int i = 2; i <= nX - 2; i++)
                    {
                        layer.A[i - 1, 0] = -a / Math.Pow(hX, 2);
                        layer.A[i - 1, 1] = 1f / thao + 2 * a / Math.Pow(hX, 2);
                        layer.A[i - 1, 2] = -a / Math.Pow(hX, 2);
                        layer.B[i - 1, 0] = (1f / thao) * U[i, j, k] + F(i * hX, j * hY, k * thao) / 2f;
                    }

                    layer.A[nX - 2, 0] = -a / Math.Pow(hX, 2);
                    layer.A[nX - 2, 1] = 1f / thao + 2 * a / Math.Pow(hX, 2);
                    layer.B[nX - 2, 0] = (1f / thao) * U[nX - 1, j, k] + F((nX - 1) * hX, j * hY, k * thao) / 2f + (a / Math.Pow(hX, 2)) * mU[nX, j];

                    double[] mUs = rundown.Execute(layer);

                    for (int i = 1; i <= nX - 1; i++)
                    {
                        mU[i, j] = mUs[i - 1];
                    }
                }

                for (int i = 0; i <= nX; i++)
                {
                    U[i, 0, k + 1] = phi3(i * hX, (k + 1) * thao);
                    U[i, nY, k + 1] = phi4(i * hX, (k + 1) * thao);
                }

                for (int j = 0; j <= nY; j++)
                {
                    U[0, j, k + 1] = phi1(j * hY, (k + 1) * thao);
                    U[nX, j, k + 1] = phi2(j * hY, (k + 1) * thao);
                }

                for (int i = 1; i <= nX - 1; i++)
                {
                    MatExt layer = new()
                    {
                        A = new double[nY - 1, 3],
                        B = new double[nY - 1, 1]
                    };

                    layer.A[0, 1] = 1f / thao + 2 * b / Math.Pow(hY, 2);
                    layer.A[0, 2] = -b / Math.Pow(hY, 2);
                    layer.B[0, 0] = (1f / thao) * mU[i, 1] + F(i * hX, 1 * hY, (k + 1f) * thao) / 2f + (b / Math.Pow(hY, 2)) * U[i, 0, k + 1];

                    for (int j = 2; j <= nY - 2; j++)
                    {
                        layer.A[j - 1, 0] = -b / Math.Pow(hY, 2);
                        layer.A[j - 1, 1] = 1f / thao + 2 * b / Math.Pow(hY, 2);
                        layer.A[j - 1, 2] = -b / Math.Pow(hY, 2);
                        layer.B[j - 1, 0] = (1f / thao) * mU[i, j] + F(i * hX, j * hY, (k + 1f) * thao) / 2f;
                    }

                    layer.A[nY - 2, 0] = -b / Math.Pow(hY, 2);
                    layer.A[nY - 2, 1] = 1f / thao + 2 * b / Math.Pow(hY, 2);
                    layer.B[nY - 2, 0] = (1f / thao) * mU[i, nY - 1] + F(i * hX, (nY - 1) * hY, (k + 1f) * thao) / 2f + (b / Math.Pow(hY, 2)) * U[i, nY, k + 1];

                    double[] Us = rundown.Execute(layer);

                    for (int j = 1; j <= nY - 1; j++)
                    {
                        U[i, j, k + 1] = Us[j - 1];
                    }
                }
            }
            return U;
        }

        private double[,] CreateMidwayLayer (int nX, double hX, int nY, double hY, double thao, int k)
        {
            double[,] U = new double[nX + 1, nY + 1];

            for (int i = 0; i <= nX; i++)
            {
                U[i, 0] = phi3(i * hX, (k + 0.5f) * thao);
                U[i, nY] = phi4(i * hX, (k + 0.5f) * thao);
            }

            for (int j = 0; j <= nY; j++)
            {
                U[0, j] = phi1(j * hY, (k + 0.5f) * thao);
                U[nX, j] = phi2(j * hY, (k + 0.5f) * thao);
            }
            return U;
        }

        private double[,,] FillStaringValues (double[,,] U, int nX, double hX, int nY, double hY)
        {
            for (int i = 0; i <= nX; i++)
            {
                for (int j = 0; j <= nY; j++)
                {
                    U[i, j, 0] = psi(i * hX, j * hY);
                }
            }
            return U;
        }

        public static double a = Values.a;
        public static double b = Values.b;

        public static Func<double, double, double, double> F = (x, y, t) => Values.F(x, y, t);

        public static double lX = Values.lX;
        public static double lY = Values.lY;

        public static Func<double, double, double> phi1 = (y, t) => Values.phi1(y, t);
        public static Func<double, double, double> phi2 = (y, t) => Values.phi2(y, t);
        public static Func<double, double, double> phi3 = (x, t) => Values.phi3(x, t);
        public static Func<double, double, double> phi4 = (x, t) => Values.phi4(x, t);

        public static Func<double, double, double> psi = (x, y) => Values.psi(x, y);

        private Rundown rundown = new();
    }
}
