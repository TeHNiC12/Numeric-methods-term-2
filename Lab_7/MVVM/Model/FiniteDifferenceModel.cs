using Lab_7.Core;
using Lab_7.DefaultValues;
using System.Diagnostics;
using System.Windows;

namespace Lab_7.MVVM.Model
{
    public class FiniteDifferenceModel
    {
        public Tuple<double[,], int> FiniteDifferenceSolve (int nX, int nY, double epsilon, int solver, double omega)
        {
            double hX = lX / nX;
            double hY = lY / nY;
            double[,] U = new double[nX + 1, nY + 1];
            U = FillBorderValues(U, nX, hX, nY, hY);
            int step = -1;

            if (solver == 1)
            {
                Tuple<double[,], int> results = Libman(U, nX, hX, nY, hY, epsilon);
                //Tuple<double[,], int> results = ConjugateGradientMethod(U, nX, hX, nY, hY, epsilon);
                U = results.Item1;
                step = results.Item2;
            }
            else if (solver == 2)
            {
                Tuple<double[,], int> results = Zeidel(U, nX, hX, nY, hY, epsilon);
                U = results.Item1;
                step = results.Item2;
            }
            else if (solver == 3)
            {
                Tuple<double[,], int> results = LibmanRelaxed(U, nX, hX, nY, hY, epsilon, omega);
                U = results.Item1;
                step = results.Item2;
            }
            else
            {
                throw new Exception("Метод решения не выбран");
            }
            return new Tuple<double[,], int>(U, step);
        }

        private Tuple<double[,], int> Libman (double[,] U, int nX, double hX, int nY, double hY, double Epsilon)
        {
            double[,] prevU;
            double[,] curU = U;
            int stepSolved = -1;

            var watch = Stopwatch.StartNew();
            for (int k = 1; k > 0; k++)
            {
                prevU = curU;
                curU = CopyBorderValues(prevU, nX, nY);

                for (int i = 1; i < nX; i++)
                {
                    for (int j = 1; j < nY; j++)
                    {
                        curU[i, j] =
                            (
                            (bX / (2 * hX) - 1 / Math.Pow(hX, 2)) * prevU[i + 1, j]
                            + (-bX / (2 * hX) - 1 / Math.Pow(hX, 2)) * prevU[i - 1, j]
                            + (bY / (2 * hY) - 1 / Math.Pow(hY, 2)) * prevU[i, j + 1]
                            + (-bY / (2 * hY) - 1 / Math.Pow(hY, 2)) * prevU[i, j - 1]
                            + F(i * hX, j * hY)
                            ) / (-2 / Math.Pow(hX, 2) - 2 / Math.Pow(hY, 2) - c);
                    }
                }

                UpdateBorderValues(curU, nX, hX, nY, hY);

                if (Norm(curU, prevU, nX, nY) <= Epsilon)
                {
                    stepSolved = k;
                    break;
                }
            }
            watch.Stop();
            if (displayExecutionTime)
            {
                MessageBox.Show($"Время выполнения: {watch.Elapsed.TotalMilliseconds} мс.");
            }

            return new Tuple<double[,], int>(curU, stepSolved);
        }

        private Tuple<double[,], int> ConjugateGradientMethod (double[,] U, int nX, double hX, int nY, double hY, double Epsilon)
        {
            int n = (nX - 1) * (nY - 1);
            double[,] A = new double[n, n];
            double[,] b = new double[n, 1];
            double[,] x0 = new double[n, 1];

            for (int i = 1; i <= nX - 1; i++)
            {
                for (int j = 1; j <= nY - 1; j++)
                {
                    x0[(j - 1) * (nX - 1) + i - 1, 0] = U[i, j];

                    if (i == 1)
                    {
                        if (j == 1)
                        {
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + i - 1] = -2f / Math.Pow(hX, 2) - 2F / Math.Pow(hY, 2) - c;
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + (i + 1) - 1] = -bX / (2 * hX) + 1f / Math.Pow(hX, 2);
                            A[(j - 1) * (nX - 1) + i - 1, ((j + 1) - 1) * (nX - 1) + i - 1] = -bY / (2 * hY) + 1f / Math.Pow(hY, 2);
                            b[(j - 1) * (nX - 1) + i - 1, 0] = F(i * hX, j * hY) - (bY / (2 * hY) + 1f / Math.Pow(hY, 2)) * U[1, 0] - (bX / (2 * hX) + 1f / Math.Pow(hX, 2)) * U[0, 1];
                        }
                        else if (j == nY - 1)
                        {
                            A[(j - 1) * (nX - 1) + i - 1, ((j - 1) - 1) * (nX - 1) + i - 1] = bY / (2 * hY) + 1f / Math.Pow(hY, 2);
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + i - 1] = -2f / Math.Pow(hX, 2) - 2F / Math.Pow(hY, 2) - c;
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + (i + 1) - 1] = -bX / (2 * hX) + 1f / Math.Pow(hX, 2);
                            b[(j - 1) * (nX - 1) + i - 1, 0] = F(i * hX, j * hY) - (bX / (2 * hX) + 1f / Math.Pow(hX, 2)) * U[0, nY - 1] - (-bY / (2 * hY) + 1f / Math.Pow(hY, 2)) * U[1, nY];
                        }
                        else
                        {
                            A[(j - 1) * (nX - 1) + i - 1, ((j - 1) - 1) * (nX - 1) + i - 1] = bY / (2 * hY) + 1f / Math.Pow(hY, 2);
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + i - 1] = -2f / Math.Pow(hX, 2) - 2F / Math.Pow(hY, 2) - c;
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + (i + 1) - 1] = -bX / (2 * hX) + 1f / Math.Pow(hX, 2);
                            A[(j - 1) * (nX - 1) + i - 1, ((j + 1) - 1) * (nX - 1) + i - 1] = -bY / (2 * hY) + 1f / Math.Pow(hY, 2);
                            b[(j - 1) * (nX - 1) + i - 1, 0] = F(i * hX, j * hY) - (bX / (2 * hX) + 1f / Math.Pow(hX, 2)) * U[0, j];
                        }
                    }
                    else if (i == nX - 1)
                    {
                        if (j == 1)
                        {
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + (i - 1) - 1] = bX / (2 * hX) + 1f / Math.Pow(hX, 2);
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + i - 1] = -2f / Math.Pow(hX, 2) - 2F / Math.Pow(hY, 2) - c;
                            A[(j - 1) * (nX - 1) + i - 1, ((j + 1) - 1) * (nX - 1) + i - 1] = -bY / (2 * hY) + 1f / Math.Pow(hY, 2);
                            b[(j - 1) * (nX - 1) + i - 1, 0] = F(i * hX, j * hY) - (bY / (2 * hY) + 1f / Math.Pow(hY, 2)) * U[nX - 1, 0] - (-bX / (2 * hX) + 1f / Math.Pow(hX, 2)) * U[nX, 1];
                        }
                        else if (j == nY - 1)
                        {
                            A[(j - 1) * (nX - 1) + i - 1, ((j - 1) - 1) * (nX - 1) + i - 1] = bY / (2 * hY) + 1f / Math.Pow(hY, 2);
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + (i - 1) - 1] = bX / (2 * hX) + 1f / Math.Pow(hX, 2);
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + i - 1] = -2f / Math.Pow(hX, 2) - 2F / Math.Pow(hY, 2) - c;
                            b[(j - 1) * (nX - 1) + i - 1, 0] = F(i * hX, j * hY) - (-bX / (2 * hX) + 1f / Math.Pow(hX, 2)) * U[nX, nY - 1] - (-bY / (2 * hY) + 1f / Math.Pow(hY, 2)) * U[nX - 1, nY];
                        }
                        else
                        {
                            A[(j - 1) * (nX - 1) + i - 1, ((j - 1) - 1) * (nX - 1) + i - 1] = bY / (2 * hY) + 1f / Math.Pow(hY, 2);
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + (i - 1) - 1] = bX / (2 * hX) + 1f / Math.Pow(hX, 2);
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + i - 1] = -2f / Math.Pow(hX, 2) - 2F / Math.Pow(hY, 2) - c;
                            A[(j - 1) * (nX - 1) + i - 1, ((j + 1) - 1) * (nX - 1) + i - 1] = -bY / (2 * hY) + 1f / Math.Pow(hY, 2);
                            b[(j - 1) * (nX - 1) + i - 1, 0] = F(i * hX, j * hY) - (-bX / (2 * hX) + 1f / Math.Pow(hX, 2)) * U[nX, j];
                        }
                    }
                    else
                    {
                        if (j == 1)
                        {
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + (i - 1) - 1] = bX / (2 * hX) + 1f / Math.Pow(hX, 2);
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + i - 1] = -2f / Math.Pow(hX, 2) - 2F / Math.Pow(hY, 2) - c;
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + (i + 1) - 1] = -bX / (2 * hX) + 1f / Math.Pow(hX, 2);
                            A[(j - 1) * (nX - 1) + i - 1, ((j + 1) - 1) * (nX - 1) + i - 1] = -bY / (2 * hY) + 1f / Math.Pow(hY, 2);
                            b[(j - 1) * (nX - 1) + i - 1, 0] = F(i * hX, j * hY) - (bY / (2 * hY) + 1f / Math.Pow(hY, 2)) * U[i, 0];
                        }
                        else if (j == nY - 1)
                        {
                            A[(j - 1) * (nX - 1) + i - 1, ((j - 1) - 1) * (nX - 1) + i - 1] = bY / (2 * hY) + 1f / Math.Pow(hY, 2);
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + (i - 1) - 1] = bX / (2 * hX) + 1f / Math.Pow(hX, 2);
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + i - 1] = -2f / Math.Pow(hX, 2) - 2F / Math.Pow(hY, 2) - c;
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + (i + 1) - 1] = -bX / (2 * hX) + 1f / Math.Pow(hX, 2);
                            b[(j - 1) * (nX - 1) + i - 1, 0] = F(i * hX, j * hY) - (-bY / (2 * hY) + 1f / Math.Pow(hY, 2)) * U[i, nY];
                        }
                        else
                        {
                            A[(j - 1) * (nX - 1) + i - 1, ((j - 1) - 1) * (nX - 1) + i - 1] = bY / (2 * hY) + 1f / Math.Pow(hY, 2);
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + (i - 1) - 1] = bX / (2 * hX) + 1f / Math.Pow(hX, 2);
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + i - 1] = -2f / Math.Pow(hX, 2) - 2F / Math.Pow(hY, 2) - c;
                            A[(j - 1) * (nX - 1) + i - 1, (j - 1) * (nX - 1) + (i + 1) - 1] = -bX / (2 * hX) + 1f / Math.Pow(hX, 2);
                            A[(j - 1) * (nX - 1) + i - 1, ((j + 1) - 1) * (nX - 1) + i - 1] = -bY / (2 * hY) + 1f / Math.Pow(hY, 2);
                            b[(j - 1) * (nX - 1) + i - 1, 0] = F(i * hX, j * hY);
                        }
                    }
                }
            }

            var watch = Stopwatch.StartNew();
            Tuple<double[,], int> result = solver.Solve(A, b, x0, Epsilon);
            watch.Stop();
            if (displayExecutionTime)
            {
                MessageBox.Show($"Время выполнения: {watch.Elapsed.TotalMilliseconds} мс.");
            }

            for (int i = 0; i < n; i++)
            {
                U[n % (nX - 1), n / (nX - 1)] = result.Item1[i, 0];
            }

            UpdateBorderValues(U, nX, hX, nY, hY);
            return new Tuple<double[,], int>(U, result.Item2);
        }

        private Tuple<double[,], int> Zeidel (double[,] U, int nX, double hX, int nY, double hY, double Epsilon)
        {
            double[,] prevU;
            double[,] curU = U;
            int stepSolved = -1;

            var watch = Stopwatch.StartNew();
            for (int k = 1; k > 0; k++)
            {
                prevU = curU;
                curU = CopyBorderValues(prevU, nX, nY);

                for (int i = 1; i < nX; i++)
                {
                    for (int j = 1; j < nY; j++)
                    {
                        curU[i, j] =
                            (
                            (bX / (2 * hX) - 1 / Math.Pow(hX, 2)) * prevU[i + 1, j]
                            + (-bX / (2 * hX) - 1 / Math.Pow(hX, 2)) * curU[i - 1, j]
                            + (bY / (2 * hY) - 1 / Math.Pow(hY, 2)) * prevU[i, j + 1]
                            + (-bY / (2 * hY) - 1 / Math.Pow(hY, 2)) * curU[i, j - 1]
                            + F(i * hX, j * hY)
                            ) / (-2 / Math.Pow(hX, 2) - 2 / Math.Pow(hY, 2) - c);
                    }
                }

                UpdateBorderValues(curU, nX, hX, nY, hY);

                if (Norm(curU, prevU, nX, nY) <= Epsilon)
                {
                    stepSolved = k;
                    break;
                }
            }
            watch.Stop();
            if (displayExecutionTime)
            {
                MessageBox.Show($"Время выполнения: {watch.Elapsed.TotalMilliseconds} мс.");
            }

            return new Tuple<double[,], int>(curU, stepSolved);
        }

        private Tuple<double[,], int> LibmanRelaxed (double[,] U, int nX, double hX, int nY, double hY, double Epsilon, double omega)
        {
            double[,] prevU;
            double[,] curU = U;
            int stepSolved = -1;

            var watch = Stopwatch.StartNew();
            for (int k = 1; k > 0; k++)
            {
                prevU = curU;
                curU = CopyBorderValues(prevU, nX, nY);

                for (int i = 1; i < nX; i++)
                {
                    for (int j = 1; j < nY; j++)
                    {
                        /*curU[i, j] =
                            (
                            (bX / (2 * hX) - 1 / Math.Pow(hX, 2)) * prevU[i + 1, j]
                            + (-bX / (2 * hX) - 1 / Math.Pow(hX, 2)) * prevU[i - 1, j]
                            + (bY / (2 * hY) - 1 / Math.Pow(hY, 2)) * prevU[i, j + 1]
                            + (-bY / (2 * hY) - 1 / Math.Pow(hY, 2)) * prevU[i, j - 1]
                            + F(i * hX, j * hY)
                            ) / (-2 / Math.Pow(hX, 2) - 2 / Math.Pow(hY, 2) - c);*/
                        curU[i, j] =
                            (
                            (bX / (2 * hX) - 1 / Math.Pow(hX, 2)) * prevU[i + 1, j]
                            + (-bX / (2 * hX) - 1 / Math.Pow(hX, 2)) * curU[i - 1, j]
                            + (bY / (2 * hY) - 1 / Math.Pow(hY, 2)) * prevU[i, j + 1]
                            + (-bY / (2 * hY) - 1 / Math.Pow(hY, 2)) * curU[i, j - 1]
                            + F(i * hX, j * hY)
                            ) / (-2 / Math.Pow(hX, 2) - 2 / Math.Pow(hY, 2) - c);
                        curU[i, j] = omega * curU[i, j] + (1 - omega) * prevU[i, j];
                    }
                }

                UpdateBorderValues(curU, nX, hX, nY, hY);

                if (Norm(curU, prevU, nX, nY) <= Epsilon)
                {
                    stepSolved = k;
                    break;
                }
            }
            watch.Stop();
            if (displayExecutionTime)
            {
                MessageBox.Show($"Время выполнения: {watch.Elapsed.TotalMilliseconds} мс.");
            }

            return new Tuple<double[,], int>(curU, stepSolved);
        }

        private double[,] FillBorderValues (double[,] U, int nX, double hX, int nY, double hY)
        {
            if (alpha1 == 0)
            {
                for (int j = 0; j <= nY; j++)
                {
                    U[0, j] = phi1(j * hY);
                }
            }
            if (alpha2 == 0)
            {
                for (int j = 0; j <= nY; j++)
                {
                    U[nX, j] = phi2(j * hY);
                }
            }
            if (alpha3 == 0)
            {
                for (int i = 0; i <= nX; i++)
                {
                    U[i, 0] = phi3(i * hX);
                }
            }
            if (alpha4 == 0)
            {
                for (int i = 0; i <= nX; i++)
                {
                    U[i, nY] = phi4(i * hX);
                }
            }
            if (alpha1 + alpha2 + alpha3 + alpha4 != 0)
            {
                U = InterpolateInnerValues(U, nX, nY);
            }

            U = UpdateBorderValues(U, nX, hX, nY, hY);

            return U;
        }

        private double[,] InterpolateInnerValues (double[,] U, int nX, int nY)
        {
            for (int i = 1; i < nX; i++)
            {
                for (int j = 1; j < nY; j++)
                {
                    /*//U[i, j] = (1 - alpha1) * (i / (double) nX) * U[0, j] + (1 - alpha2) * ((nX - i) / (double) nX) * U[nX, j] + (1 - alpha3) * (j / (double) nY) * U[i, 0] + (1 - alpha4) * ((nY - j) / (double) nY) * U[i, nY];
                    U[i, j] = (i / (double) (i + j)) * U[i, 0] + (i / (double) (i + j)) * U[0, j];*/
                    U[i, j] = ((nY - j) / (double) (nX + nY - i - j)) * U[nX, j] + ((nX - i) / (double) (nX + nY - i - j)) * U[i, nY];
                    //U[i, j] = ((nY - j) / (double) (nX + nY - i - j)) * U[i, nY] + ((nX - i) / (double) (nX + nY - i - j)) * U[nX, j];
                }
            }
            return U;
        }

        private double[,] CopyBorderValues (double[,] U, int nX, int nY)
        {
            double[,] newU = new double[nX + 1, nY + 1];

            for (int i = 0; i <= nX; i++)
            {
                newU[i, 0] = U[i, 0];
                newU[i, nY] = U[i, nY];
            }
            for (int j = 0; j <= nY; j++)
            {
                newU[0, j] = U[0, j];
                newU[nX, j] = U[nX, j];
            }

            return newU;
        }

        private double[,] UpdateBorderValues (double[,] U, int nX, double hX, int nY, double hY)
        {
            if (alpha1 == 1)
            {
                for (int j = 1; j <= nY - 1; j++)
                {
                    //U[0, j] = (phi1(j * hY) - (alpha1 / (2 * hX)) * (4 * U[1, j] - U[2, j])) / (-3 * (alpha1 / (2 * hX)) + beta1);
                    U[0, j] = (phi1(j * hY) - (alpha1 / hX) * U[1, j]) / (-alpha1 / hX + beta1);
                }
            }
            if (alpha2 == 1)
            {
                for (int j = 1; j <= nY - 1; j++)
                {
                    //U[nX, j] = (phi1(j * hY) - (alpha1 / (2 * hX)) * (4 * U[1, j] - U[2, j])) / (-3 * (alpha1 / (2 * hX)) + beta1);
                    U[nX, j] = (phi2(j * hY) + (alpha2 / hX) * U[nX - 1, j]) / (alpha2 / hX + beta2);
                }
            }
            if (alpha3 == 1)
            {
                for (int i = 1; i <= nX - 1; i++)
                {
                    //U[i, 0] = (phi3(i * hX) - (alpha3 / (2 * hY)) * (4 * U[i, 1] - U[i, 2])) / (-3 * (alpha3 / (2 * hY)) + beta3);
                    U[i, 0] = (phi3(i * hX) - (alpha3 / hY) * U[i, 1]) / (-alpha3 / hY + beta3);
                }
            }
            if (alpha4 == 1)
            {
                for (int i = 1; i <= nX - 1; i++)
                {
                    //U[i, nY] = (phi4(i * hX) - (alpha4 / (2 * hY)) * (-4 * U[i, nY - 1] + U[i, nY - 2])) / (3 * (alpha4 / (2 * hY)) + beta4);
                    U[i, nY] = (phi4(i * hX) + (alpha4 / hY) * U[i, nY - 1]) / (alpha4 / hY + beta4);
                }
            }

            return U;
        }

        private double Norm (double[,] curU, double[,] prevU, int nX, int nY)
        {
            double max = -1f;
            for (int i = 0; i <= nX; i++)
            {
                for (int j = 0; j <= nY; j++)
                {
                    double x = Math.Abs(curU[i, j] - prevU[i, j]);
                    if (x >= max)
                    {
                        max = x;
                    }
                }
            }
            return max;
        }

        private double bX = Values.bX;
        private double bY = Values.bY;
        private double c = Values.c;

        private Func<double, double, double> F = (x, y) => Values.F(x, y);

        private double lX = Values.lX;
        private double lY = Values.lY;

        private int alpha1 = Values.alpha1;
        private int beta1 = Values.beta1;
        private Func<double, double> phi1 = y => Values.phi1(y);

        private int alpha2 = Values.alpha2;
        private int beta2 = Values.beta2;
        private Func<double, double> phi2 = y => Values.phi2(y);

        private int alpha3 = Values.alpha3;
        private int beta3 = Values.beta3;
        private Func<double, double> phi3 = x => Values.phi3(x);

        private int alpha4 = Values.alpha4;
        private int beta4 = Values.beta4;
        private Func<double, double> phi4 = x => Values.phi4(x);

        //Выводит время исполнения алгоритма
        private bool displayExecutionTime = false;

        ConjugateGradient solver = new();
    }
}
