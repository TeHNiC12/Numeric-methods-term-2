using Lab_5.Core;
using Lab_5.DefaultValues;
using System;

namespace Lab_5.MVVM.Model
{
    public class FiniteDifferenceModel
    {
        public double[,] FiniteDifferenceSolve (double l, int N, double T, int K, int scheme, int approximation)
        {
            double h = l / N;
            double thao = T / K;
            double[,] U = new double[K + 1, N + 1];

            if (scheme == 1)
            {
                if (Values.a * thao / Math.Pow(h, 2) <= 0.5f)
                {
                    U = FillInitialValues(U, h, N);
                    U = Explicict(U, h, N, thao, K, approximation);
                }
                else
                {
                    throw new Exception("Условие устойчивости явной схемы не выполнено");
                }
            }
            else if (scheme == 2)
            {
                U = FillInitialValues(U, h, N);
                U = Implicit(U, h, N, thao, K, approximation);
            }
            else if (scheme == 3)
            {
                double[,] UE = new double[K + 1, N + 1];
                UE = FillInitialValues(UE, h, N);
                UE = Explicict(UE, h, N, thao, K, approximation);

                double[,] UI = new double[K + 1, N + 1];
                UI = FillInitialValues(UI, h, N);
                UI = Implicit(UI, h, N, thao, K, approximation);

                for (int k = 0; k <= K; k++)
                {
                    for (int j = 0; j <= N; j++)
                    {
                        U[k, j] = 0.5 * UE[k, j] + 0.5 * UI[k, j];
                    }
                }
            }
            else
            {
                throw new Exception("Схема не выбрана");
            }
            return U;
        }

        private double[,] FillInitialValues (double[,] U, double h, int N)
        {
            for (int j = 0; j <= N; j++)
            {
                U[0, j] = Values.psi(j * h);
            }
            return U;
        }

        private double[,] Explicict (double[,] U, double h, int N, double thao, int K, int approximation)
        {
            for (int k = 0; k <= K - 1; k++)
            {
                for (int j = 1; j <= N - 1; j++)
                {
                    U[k + 1, j] = thao * (Values.a * (U[k, j - 1] - 2 * U[k, j] + U[k, j + 1]) / Math.Pow(h, 2) + Values.b * (U[k, j + 1] - U[k, j - 1]) / (2 * h) + Values.c * U[k, j] + Values.fX(j * h, k * thao)) + U[k, j];
                }
                if (approximation == 1)
                {
                    U[k + 1, 0] = (Values.phi1((k + 1) * thao) - (Values.alpha / h) * U[k + 1, 1]) / (-Values.alpha / h + Values.beta);
                    U[k + 1, N] = (Values.phi2((k + 1) * thao) + (Values.gamma / h) * U[k + 1, N - 1]) / (Values.gamma / h + Values.eta);
                }
                else if (approximation == 2)
                {
                    U[k + 1, 0] = (Values.phi1((k + 1) * thao) - 2 * Values.alpha * U[k + 1, 1] / h + Values.alpha * U[k + 1, 2] / (2 * h)) / (-3 * Values.alpha / (2 * h) + Values.beta);
                    U[k + 1, N] = (Values.phi2((k + 1) * thao) - Values.gamma * U[k + 1, N - 2] / (2 * h) + 2 * Values.gamma * U[k + 1, N - 1] / h) / (3 * Values.gamma / (2 * h) + Values.eta);
                }
                else if (approximation == 3)
                {
                    U[k + 1, 0] =
                        (
                        Values.phi1((k + 1) * thao)
                        - Values.alpha * h / (2 * Values.a) * Values.fX(0, (k + 1) * thao)
                        - Values.alpha * h / (2 * Values.a * thao) * U[k, 0]
                        - Values.alpha * (2 * Values.a + Values.b * h) / (2 * Values.a * h) * U[k + 1, 1]
                        ) / (
                        Values.alpha * (-2 * Values.a * thao - Math.Pow(h, 2) - Values.b * h * thao + Values.c * Math.Pow(h, 2) * thao) / (2 * Values.a * h * thao)
                        + Values.beta
                        );
                    U[k + 1, N] =
                        (
                        Values.phi2((k + 1) * thao)
                        + Values.gamma * h / (2 * Values.a) * Values.fX(N * h, (k + 1) * thao)
                        + Values.gamma * h / (2 * Values.a * thao) * U[k, N]
                        - Values.gamma * (-2 * Values.a + Values.b * h) / (2 * Values.a * h) * U[k + 1, N - 1]
                        ) / (
                        Values.gamma * (2 * Values.a * thao + Math.Pow(h, 2) - Values.b * h * thao - Values.c * Math.Pow(h, 2) * thao) / (2 * Values.a * h * thao)
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
            for (int k = 0; k <= K - 1; k++)
            {
                MatExt layer = new()
                {
                    A = new double[N + 1, 3],
                    B = new double[N + 1, 1]
                };
                for (int j = 1; j <= N - 1; j++)
                {
                    layer.A[j, 0] = Values.a * thao / Math.Pow(h, 2) - Values.b * thao / (2 * h);
                    layer.A[j, 1] = -2 * Values.a * thao / Math.Pow(h, 2) - 1 + Values.c * thao;
                    layer.A[j, 2] = Values.a * thao / Math.Pow(h, 2) + Values.b * thao / (2 * h);
                    layer.B[j, 0] = -thao * Values.fX(j * h, (k + 1) * thao) - U[k, j];
                }
                if (approximation == 1)
                {
                    layer.A[0, 1] = Values.beta - Values.alpha / h;
                    layer.A[0, 2] = Values.alpha / h;
                    layer.B[0, 0] = Values.phi1((k + 1) * thao);

                    layer.A[N, 0] = -Values.gamma / h;
                    layer.A[N, 1] = Values.gamma / h + Values.eta;
                    layer.B[N, 0] = Values.phi2((k + 1) * thao);
                }
                else if (approximation == 2)
                {
                    layer.A[0, 1] = -3 * Values.alpha / (2 * h) + Values.beta - layer.A[1, 0] * (-Values.alpha / (2 * h)) / layer.A[1, 2];
                    layer.A[0, 2] = 2 * Values.alpha / h - layer.A[1, 1] * (-Values.alpha / (2 * h)) / layer.A[1, 2];
                    layer.B[0, 0] = Values.phi1((k + 1) * thao) - layer.B[1, 0] * (-Values.alpha / (2 * h)) / layer.A[1, 2];

                    layer.A[N, 0] = -2 * Values.gamma / h - layer.A[N - 1, 1] * (Values.gamma / (2 * h)) / layer.A[N - 1, 0];
                    layer.A[N, 1] = Values.eta + 3 * Values.gamma / (2 * h) - layer.A[N - 1, 2] * (Values.gamma / (2 * h)) / layer.A[N - 1, 0];
                    layer.B[N, 0] = Values.phi2((k + 1) * thao) - layer.B[N - 1, 0] * (Values.gamma / (2 * h)) / layer.A[N - 1, 0];
                }
                else if (approximation == 3)
                {
                    layer.A[0, 1] = Values.alpha * (-2 * Values.a * thao - Math.Pow(h, 2) - Values.b * h * thao + Values.c * Math.Pow(h, 2) * thao) / (2 * Values.a * h * thao) + Values.beta;
                    layer.A[0, 2] = Values.alpha * (2 * Values.a + Values.b * h) / (2 * Values.a * h);
                    layer.B[0, 0] = Values.phi1((k + 1) * thao) - Values.alpha * h / (2 * Values.a) * Values.fX(0, (k + 1) * thao) - Values.alpha * h / (2 * Values.a * thao) * U[k, 0];

                    layer.A[N, 0] = Values.gamma * (-2 * Values.a + Values.b * h) / (2 * Values.a * h);
                    layer.A[N, 1] = Values.gamma * (2 * Values.a * thao + Math.Pow(h, 2) - Values.b * h * thao - Values.c * Math.Pow(h, 2) * thao) / (2 * Values.a * h * thao) + Values.eta;
                    layer.B[N, 0] = Values.phi2((k + 1) * thao) + Values.gamma * h / (2 * Values.a) * Values.fX(N * h, (k + 1) * thao) + Values.gamma * h / (2 * Values.a * thao) * U[k, N];
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
