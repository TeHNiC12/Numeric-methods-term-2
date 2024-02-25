using Lab_6.Core;
using Lab_6.DefaultValues;

namespace Lab_6.MVVM.Model
{
    public class FiniteDifferenceModel
    {
        public double[,] FiniteDifferenceSolve (double l, int N, double T, int K, int scheme, int approximation, int startApproximation)
        {
            double h = l / N;
            double thao = T / K;
            double[,] U = new double[K + 1, N + 1];

            if (scheme == 1)
            {
                if (Math.Pow(Values.a, 2) * Math.Pow(thao, 2) / Math.Pow(h, 2) < 1)
                {
                    U = FillInitialValues(U, h, N, thao, startApproximation);
                    U = Explicict(U, h, N, thao, K, approximation);
                }
                else
                {
                    throw new Exception("Условие устойчивости явной схемы не выполнено");
                }
            }
            else if (scheme == 2)
            {
                U = FillInitialValues(U, h, N, thao, startApproximation);
                U = Implicit(U, h, N, thao, K, approximation);
            }
            else
            {
                throw new Exception("Схема не выбрана");
            }
            return U;
        }

        private double[,] FillInitialValues (double[,] U, double h, int N, double thao, int startApproximation)
        {
            if (startApproximation == 1)
            {
                for (int j = 0; j <= N; j++)
                {
                    U[0, j] = Values.psi1(j * h);
                    U[1, j] = U[0, j] + thao * Values.psi2(j * h);
                }
            }
            else if (startApproximation == 2)
            {
                for (int j = 0; j <= N; j++)
                {
                    U[0, j] = Values.psi1(j * h);
                    U[1, j] = U[0, j] + thao * Values.psi2(j * h) + (Math.Pow(thao, 2) / 2f) * (Math.Pow(Values.a, 2) * Values.ddpsi1(j * h) + Values.b * Values.dpsi1(j * h) + Values.c * U[0, j] - Values.d * Values.psi2(j * h) + Values.fX(j * h, 0));
                }
            }
            else
            {
                throw new Exception("Аппроксимация начального условия не выбрана");
            }
            return U;
        }

        private double[,] Explicict (double[,] U, double h, int N, double thao, int K, int approximation)
        {
            for (int k = 1; k <= K - 1; k++)
            {
                for (int j = 1; j <= N - 1; j++)
                {
                    U[k + 1, j] =
                        (
                        Math.Pow(thao, 2)
                        * (
                        Math.Pow(Values.a, 2) * (U[k, j - 1] - 2 * U[k, j] + U[k, j + 1]) / Math.Pow(h, 2) + Values.b * (U[k, j + 1] - U[k, j - 1]) / (2f * h) + Values.c * U[k, j] + Values.d * U[k - 1, j] / (2 * thao) + Values.fX(j * h, k * thao)
                        ) + 2 * U[k, j] - U[k - 1, j]
                        ) / (
                        1 + thao * Values.d / 2f);
                }
                if (approximation == 1)
                {
                    U[k + 1, 0] = (Values.phi0((k + 1) * thao) - (Values.alpha / h) * U[k + 1, 1]) / (-Values.alpha / h + Values.beta);
                    U[k + 1, N] = (Values.phil((k + 1) * thao) + (Values.gamma / h) * U[k + 1, N - 1]) / (Values.gamma / h + Values.eta);
                }
                else if (approximation == 2)
                {
                    U[k + 1, 0] = (Values.phi0((k + 1) * thao) - 2 * Values.alpha * U[k + 1, 1] / h + Values.alpha * U[k + 1, 2] / (2 * h)) / (-3 * Values.alpha / (2 * h) + Values.beta);
                    U[k + 1, N] = (Values.phil((k + 1) * thao) - Values.gamma * U[k + 1, N - 2] / (2 * h) + 2 * Values.gamma * U[k + 1, N - 1] / h) / (3 * Values.gamma / (2 * h) + Values.eta);
                }
                else if (approximation == 3)
                {
                    U[k + 1, 0] =
                        (
                        Values.phi0((k + 1) * thao)
                        - 2 * Math.Pow(Values.a, 2) * Values.alpha / (2 * Math.Pow(Values.a, 2) - Values.b * h)
                        * (
                        (2 * h + Values.d * h * thao) * U[k, 0] / (2 * Math.Pow(Values.a, 2) * Math.Pow(thao, 2)) - h * U[k - 1, 0] / (2 * Math.Pow(Values.a, 2) * Math.Pow(thao, 2)) + h * Values.fX(0, (k + 1) * thao) / 2f
                        )
                        - 2 * Math.Pow(Values.a, 2) * Values.alpha * U[k + 1, 1] / ((2 * Math.Pow(Values.a, 2) - Values.b * h) * h)
                        ) / (
                        2 * Math.Pow(Values.a, 2) * Values.alpha / (2 * Math.Pow(Values.a, 2) - Values.b * h)
                        * (
                        -1f / h - h / (2 * Math.Pow(Values.a, 2) * Math.Pow(thao, 2)) + Values.c * h / (2 * Math.Pow(Values.a, 2)) - Values.d * h / (2 * Math.Pow(Values.a, 2) * thao)
                        )
                        + Values.beta
                        );

                    U[k + 1, N] =
                        (
                        Values.phil((k + 1) * thao)
                        - 2 * Math.Pow(Values.a, 2) * Values.gamma / (2 * Math.Pow(Values.a, 2) - Values.b * h)
                        * (
                        (-2 * h - Values.d * h * thao) * U[k, N] / (2 * Math.Pow(Values.a, 2) * Math.Pow(thao, 2)) + h * U[k - 1, N] / (2 * Math.Pow(Values.a, 2) * Math.Pow(thao, 2)) - h * Values.fX(N * h, (k + 1) * thao) / 2f
                        )
                        - 2 * Math.Pow(Values.a, 2) * Values.gamma * U[k + 1, N - 1] / ((Values.b * h - 2 * Math.Pow(Values.a, 2)) * h)
                        ) / (
                        2 * Math.Pow(Values.a, 2) * Values.gamma / (2 * Math.Pow(Values.a, 2) - Values.b * h)
                        * (
                        1f / h + h / (2 * Math.Pow(Values.a, 2) * Math.Pow(thao, 2)) - Values.c * h / (2 * Math.Pow(Values.a, 2)) + Values.d * h / (2 * Math.Pow(Values.a, 2) * thao)
                        )
                        + Values.eta
                        );
                }
                else
                {
                    throw new Exception("Аппроксимация не выбрана");
                }
            }
            return U;
        }

        private double[,] Implicit (double[,] U, double h, int N, double thao, int K, int approximation)
        {
            for (int k = 1; k <= K - 1; k++)
            {
                MatExt layer = new()
                {
                    A = new double[N + 1, 3],
                    B = new double[N + 1, 1]
                };
                for (int j = 1; j <= N - 1; j++)
                {
                    layer.A[j, 0] = Math.Pow(Values.a, 2) / Math.Pow(h, 2) - Values.b / (2 * h);
                    layer.A[j, 1] = -1f / Math.Pow(thao, 2) - 2 * Math.Pow(Values.a, 2) / Math.Pow(h, 2) + Values.c - Values.d / (2 * thao);
                    layer.A[j, 2] = Math.Pow(Values.a, 2) / Math.Pow(h, 2) + Values.b / (2 * h);
                    layer.B[j, 0] = (U[k - 1, j] - 2 * U[k, j]) / Math.Pow(thao, 2) - Values.d * U[k - 1, j] / (2 * thao) - Values.fX(j * h, (k + 1) * thao);
                }
                if (approximation == 1)
                {
                    layer.A[0, 1] = Values.beta - Values.alpha / h;
                    layer.A[0, 2] = Values.alpha / h;
                    layer.B[0, 0] = Values.phi0((k + 1) * thao);

                    layer.A[N, 0] = -Values.gamma / h;
                    layer.A[N, 1] = Values.gamma / h + Values.eta;
                    layer.B[N, 0] = Values.phil((k + 1) * thao);
                }
                else if (approximation == 2)
                {
                    layer.A[0, 1] = -3 * Values.alpha / (2 * h) + Values.beta - layer.A[1, 0] * (-Values.alpha / (2 * h)) / layer.A[1, 2];
                    layer.A[0, 2] = 2 * Values.alpha / h - layer.A[1, 1] * (-Values.alpha / (2 * h)) / layer.A[1, 2];
                    layer.B[0, 0] = Values.phi0((k + 1) * thao) - layer.B[1, 0] * (-Values.alpha / (2 * h)) / layer.A[1, 2];

                    layer.A[N, 0] = -2 * Values.gamma / h - layer.A[N - 1, 1] * (Values.gamma / (2 * h)) / layer.A[N - 1, 0];
                    layer.A[N, 1] = Values.eta + 3 * Values.gamma / (2 * h) - layer.A[N - 1, 2] * (Values.gamma / (2 * h)) / layer.A[N - 1, 0];
                    layer.B[N, 0] = Values.phil((k + 1) * thao) - layer.B[N - 1, 0] * (Values.gamma / (2 * h)) / layer.A[N - 1, 0];
                }
                else if (approximation == 3)
                {
                    layer.A[0, 1] =
                        2 * Math.Pow(Values.a, 2) * Values.alpha / (2 * Math.Pow(Values.a, 2) - Values.b * h)
                        * (
                        -1f / h - h / (2 * Math.Pow(Values.a, 2) * Math.Pow(thao, 2)) + Values.c * h / (2 * Math.Pow(Values.a, 2)) - Values.d * h / (2 * Math.Pow(Values.a, 2) * thao)
                        )
                        + Values.beta;
                    layer.A[0, 2] = 2 * Math.Pow(Values.a, 2) / ((2 * Math.Pow(Values.a, 2) - Values.b * h) * h);
                    layer.B[0, 0] =
                        Values.phi0((k + 1) * thao)
                        - 2 * Math.Pow(Values.a, 2) * Values.alpha / (2 * Math.Pow(Values.a, 2) - Values.b * h)
                        * (
                        (2 * h + Values.d * h * thao) * U[k, 0] / (2 * Math.Pow(Values.a, 2) * Math.Pow(thao, 2)) - h * U[k - 1, 0] / (2 * Math.Pow(Values.a, 2) * Math.Pow(thao, 2)) + h * Values.fX(0, (k + 1) * thao) / 2f
                        );

                    layer.A[N, 0] = 2 * Math.Pow(Values.a, 2) * Values.gamma / ((Values.b * h - 2 * Math.Pow(Values.a, 2)) * h);
                    layer.A[N, 1] =
                    2 * Math.Pow(Values.a, 2) * Values.gamma / (2 * Math.Pow(Values.a, 2) - Values.b * h)
                        * (
                        1f / h + h / (2 * Math.Pow(Values.a, 2) * Math.Pow(thao, 2)) - Values.c * h / (2 * Math.Pow(Values.a, 2)) + Values.d * h / (2 * Math.Pow(Values.a, 2) * thao)
                        )
                        + Values.eta;
                    layer.B[N, 0] =
                        Values.phil((k + 1) * thao)
                        - 2 * Math.Pow(Values.a, 2) * Values.gamma / (2 * Math.Pow(Values.a, 2) - Values.b * h)
                        * (
                        (-2 * h - Values.d * h * thao) * U[k, N] / (2 * Math.Pow(Values.a, 2) * Math.Pow(thao, 2)) + h * U[k - 1, N] / (2 * Math.Pow(Values.a, 2) * Math.Pow(thao, 2)) - h * Values.fX(N * h, (k + 1) * thao) / 2f
                        );
                }
                else
                {
                    throw new Exception("Аппроксимация не выбрана");
                }
                double[] layerResult = rundown.Execute(layer);
                for (int i = 0; i <= N; i++)
                {
                    U[k + 1, i] = layerResult[i];
                }
            }
            return U;
        }

        Rundown rundown = new();
    }
}
