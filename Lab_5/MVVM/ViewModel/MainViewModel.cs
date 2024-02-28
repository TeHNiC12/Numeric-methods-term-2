using Lab_5.Core;
using Lab_5.DefaultValues;
using Lab_5.MVVM.Model;
using Lab_5.MVVM.View;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Lab_5.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public RelayCommand ToggleExplicitMethodCommand { get; set; }
        public RelayCommand ToggleImplicitMethodCommand { get; set; }
        public RelayCommand ToggleCrancNicolsonMethodCommand { get; set; }
        public RelayCommand ToggleTwoPointFirstOrderCommand { get; set; }
        public RelayCommand ToggleThreePointSecondOrderCommand { get; set; }
        public RelayCommand ToggleTwoPointSecondOrderCommand { get; set; }

        public RelayCommand ToggleHErrorCommand { get; set; }
        public RelayCommand ToggleThaoErrorCommand { get; set; }

        public RelayCommand SolveCommand { get; set; }
        public RelayCommand PopOutCommand { get; set; }

        public double RightTimeLimit
        {
            get { return rightTimeLimit; }
            set { rightTimeLimit = value; UpdateGridPar(); OnPropertyChanged(); }
        }
        public int K
        {
            get { return k; }
            set { k = value; UpdateGridPar(); OnPropertyChanged(); }
        }
        public double Thao
        {
            get { return thao; }
            set { thao = value; OnPropertyChanged(); }
        }

        public int N
        {
            get { return n; }
            set { n = value; UpdateGridPar(); OnPropertyChanged(); }
        }
        public double H
        {
            get { return h; }
            set { h = value; OnPropertyChanged(); }
        }

        public double TicFrequency
        {
            get { return ticFrequency; }
            set { ticFrequency = value; OnPropertyChanged(); }
        }
        public double SelectedTimeMoment
        {
            get { return selectedTimeMoment; }
            set
            {
                if (value % thao < 0.5 * thao)
                {
                    selectedTimePoint = (int) (value / thao);
                    selectedTimeMoment = selectedTimePoint * thao;
                }
                else
                {
                    selectedTimePoint = (int) (value / thao) + 1;
                    if (selectedTimePoint > k)
                    {
                        selectedTimePoint = k;
                    }
                    selectedTimeMoment = selectedTimePoint * thao;
                }
                OnPropertyChanged();
                UpdatePlot(plotModel);
            }
        }

        public string SettingsVisible
        {
            get { return settingsVisible; }
            set { settingsVisible = value; OnPropertyChanged(); }
        }


        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; OnPropertyChanged(); }
        }

        public MainViewModel ()
        {
            model = new();
            plotModel = InitializePlot();

            explicitScheme = 1;
            twoFirstApproximation = 1;
            SettingsVisible = "Visible";

            K = 100;
            N = 10;

            ToggleExplicitMethodCommand = new RelayCommand(o =>
            {
                if (explicitScheme == 1)
                {
                    explicitScheme = 0;
                }
                else
                {
                    explicitScheme = 1;
                }
            });
            ToggleImplicitMethodCommand = new RelayCommand(o =>
            {
                if (implicitScheme == 1)
                {
                    implicitScheme = 0;
                }
                else
                {
                    implicitScheme = 1;
                }
            });
            ToggleCrancNicolsonMethodCommand = new RelayCommand(o =>
            {
                if (cnScheme == 1)
                {
                    cnScheme = 0;
                }
                else
                {
                    cnScheme = 1;
                }
            });
            ToggleTwoPointFirstOrderCommand = new RelayCommand(o =>
            {
                if (twoFirstApproximation == 1)
                {
                    twoFirstApproximation = 0;
                }
                else
                {
                    twoFirstApproximation = 1;
                }
            });
            ToggleThreePointSecondOrderCommand = new RelayCommand(o =>
            {
                if (threeSecondApproximation == 1)
                {
                    threeSecondApproximation = 0;
                }
                else
                {
                    threeSecondApproximation = 1;
                }
            });
            ToggleTwoPointSecondOrderCommand = new RelayCommand(o =>
            {
                if (twoSecondApproximation == 1)
                {
                    twoSecondApproximation = 0;
                }
                else
                {
                    twoSecondApproximation = 1;
                }
            });
            ToggleHErrorCommand = new RelayCommand(o =>
            {
                if (hError == 1)
                {
                    if (thaoError == 0)
                    {
                        SettingsVisible = "Visible";
                    }
                    hError = 0;
                }
                else
                {
                    SettingsVisible = "Hidden";
                    hError = 1;
                }
            });
            ToggleThaoErrorCommand = new RelayCommand(o =>
            {
                if (thaoError == 1)
                {
                    if (hError == 0)
                    {
                        SettingsVisible = "Visible";
                    }
                    thaoError = 0;
                }
                else
                {
                    SettingsVisible = "Hidden";
                    thaoError = 1;
                }
            });

            SolveCommand = new RelayCommand(o =>
            {
                if (explicitScheme + implicitScheme + cnScheme > 1 & twoFirstApproximation + threeSecondApproximation + twoSecondApproximation > 1)
                {
                    MessageBox.Show("Сравнение более одной схемы и более одного метода аппроксимации одновременно невозможно");
                }
                else if (thao == 0 | h == 0)
                {
                    MessageBox.Show("Сеточные параметры не могут быть равны 0");
                }
                else if (hError + thaoError == 2)
                {
                    MessageBox.Show("Невозможно изучаение отклонеия от h и 𝜏 одновременно");
                }
                else
                {
                    try
                    {
                        if (thaoError + hError == 0)
                        {
                            plots = CalculatePoints();
                        }
                        else if (hError == 1)
                        {
                            plots = CalculateHErrorPoints();
                        }
                        UpdatePlot(plotModel);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            });
            PopOutCommand = new RelayCommand(o =>
            {
                string title = "";
                if (hError + thaoError == 0)
                {
                    title = string.Format("графики фунцкий U в момент времени t = {0}", selectedTimeMoment);
                }
                else if (hError == 1)
                {
                    title = "График звисимости погрешности от сеточного параметра h";
                }
                else if (thaoError == 1)
                {
                    title = "График звисимости погрешности от сеточного параметра 𝜏";
                }
                PlotModel pm = new PlotModel();
                UpdatePlot(pm);
                PlotInspectorViewModel plotInspectorViewModel = new(pm, title);
                PlotInspector plotInspector = new PlotInspector
                {
                    DataContext = plotInspectorViewModel
                };
                plotInspector.Show();
            });
        }

        private PlotModel InitializePlot ()
        {
            PlotModel plot = new();

            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                TitleFontSize = 18,
                Title = "X"
            };
            var uAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                TitleFontSize = 18,
                Title = "U"
            };

            plot.Axes.Add(xAxis);
            plot.Axes.Add(uAxis);

            return plot;
        }
        private void UpdatePlot (PlotModel plotModel)
        {
            plotModel.Annotations.Clear();
            plotModel.Axes.Clear();
            plotModel.Legends.Clear();
            plotModel.Series.Clear();

            var zeroLineY = new LineAnnotation
            {
                Type = LineAnnotationType.Horizontal,
                Y = 0,
                Color = OxyColors.Black,
                LineStyle = LineStyle.Solid,
                StrokeThickness = 1
            };
            var zeroLineX = new LineAnnotation
            {
                Type = LineAnnotationType.Vertical,
                X = 0,
                Color = OxyColors.Black,
                LineStyle = LineStyle.Solid,
                StrokeThickness = 1
            };

            plotModel.Legends.Add(new Legend
            {
                LegendPosition = LegendPosition.TopLeft,
                LegendPlacement = LegendPlacement.Inside
            });

            plotModel.Annotations.Add(zeroLineX);
            plotModel.Annotations.Add(zeroLineY);

            if (hError + thaoError == 0)
            {
                var xAxis = new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    TitleFontSize = 18,
                    Title = "X"
                };
                var yAxis = new LinearAxis
                {
                    Position = AxisPosition.Left,
                    TitleFontSize = 18,
                    Title = "U"
                };
                plotModel.Axes.Add(xAxis);
                plotModel.Axes.Add(yAxis);

                plotModel.Series.Add(new FunctionSeries(AnaliticalU, 0f, xMax, 0.001f, "Аналитическое решение"));

                if (plots != null)
                {
                    foreach (PlotData plot in plots)
                    {
                        LineSeries lineSeries = new();
                        lineSeries.Title = plot.title;
                        for (int i = 0; i <= N; i++)
                        {
                            lineSeries.Points.Add(new DataPoint(i * h, plot.U[selectedTimePoint, i]));
                        }
                        plotModel.Series.Add(lineSeries);
                    }
                }
            }
            else if (hError == 1)
            {
                var xAxis = new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    TitleFontSize = 18,
                    Title = "h"
                };
                var yAxis = new LinearAxis
                {
                    Position = AxisPosition.Left,
                    TitleFontSize = 18,
                    Title = "Error"
                };
                plotModel.Axes.Add(xAxis);
                plotModel.Axes.Add(yAxis);

                if (plots != null)
                {
                    foreach (PlotData plot in plots)
                    {
                        LineSeries lineSeries = new();
                        lineSeries.Title = plot.title;
                        for (int i = 0; i < plot.U.GetLength(1); i++)
                        {
                            lineSeries.Points.Add(new DataPoint(plot.U[0, i], plot.U[1, i]));
                        }
                        plotModel.Series.Add(lineSeries);
                    }
                }
            }
            else if (thaoError == 1)
            {
                var xAxis = new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    TitleFontSize = 18,
                    Title = "𝜏"
                };
                var yAxis = new LinearAxis
                {
                    Position = AxisPosition.Left,
                    TitleFontSize = 18,
                    Title = "Error"
                };
                plotModel.Axes.Add(xAxis);
                plotModel.Axes.Add(yAxis);
            }

            plotModel.InvalidatePlot(true);
        }

        private List<PlotData> CalculatePoints ()
        {
            List<PlotData> plotsData = new();
            if (explicitScheme == 1)
            {
                if (twoFirstApproximation == 1)
                {
                    PlotData plotData = new();
                    plotData.title = "Явная 2Т.1П.";
                    plotData.U = model.FiniteDifferenceSolve(xMax, n, rightTimeLimit, k, 1, 1);
                    plotsData.Add(plotData);
                }
                if (threeSecondApproximation == 1)
                {
                    PlotData plotData = new();
                    plotData.title = "Явная 3Т.2П.";
                    plotData.U = model.FiniteDifferenceSolve(xMax, n, rightTimeLimit, k, 1, 2);
                    plotsData.Add(plotData);
                }
                if (twoSecondApproximation == 1)
                {
                    PlotData plotData = new();
                    plotData.title = "Явная 2Т.2П.";
                    plotData.U = model.FiniteDifferenceSolve(xMax, n, rightTimeLimit, k, 1, 3);
                    plotsData.Add(plotData);
                }
            }
            if (implicitScheme == 1)
            {
                if (twoFirstApproximation == 1)
                {
                    PlotData plotData = new();
                    plotData.title = "Невная 2Т.1П.";
                    plotData.U = model.FiniteDifferenceSolve(xMax, n, rightTimeLimit, k, 2, 1);
                    plotsData.Add(plotData);
                }
                if (threeSecondApproximation == 1)
                {
                    PlotData plotData = new();
                    plotData.title = "Невная 3Т.2П.";
                    plotData.U = model.FiniteDifferenceSolve(xMax, n, rightTimeLimit, k, 2, 2);
                    plotsData.Add(plotData);
                }
                if (twoSecondApproximation == 1)
                {
                    PlotData plotData = new();
                    plotData.title = "Невная 2Т.2П.";
                    plotData.U = model.FiniteDifferenceSolve(xMax, n, rightTimeLimit, k, 2, 3);
                    plotsData.Add(plotData);
                }
            }
            if (cnScheme == 1)
            {
                if (twoFirstApproximation == 1)
                {
                    PlotData plotData = new();
                    plotData.title = "Кранка-Никлсона 2Т.1П.";
                    plotData.U = model.FiniteDifferenceSolve(xMax, n, rightTimeLimit, k, 3, 1);
                    plotsData.Add(plotData);
                }
                if (threeSecondApproximation == 1)
                {
                    PlotData plotData = new();
                    plotData.title = "Кранка-Никлсона 3Т.2П.";
                    plotData.U = model.FiniteDifferenceSolve(xMax, n, rightTimeLimit, k, 3, 2);
                    plotsData.Add(plotData);
                }
                if (twoSecondApproximation == 1)
                {
                    PlotData plotData = new();
                    plotData.title = "Кранка-Никлсона 2Т.2П.";
                    plotData.U = model.FiniteDifferenceSolve(xMax, n, rightTimeLimit, k, 3, 3);
                    plotsData.Add(plotData);
                }
            }
            return plotsData;
        }
        private List<PlotData> CalculateHErrorPoints ()
        {
            List<PlotData> plotsData = new();
            int nMin = 5;
            int stepSize = 5;
            int steps = 5;
            double t = 1f;
            int k = 190;
            int timeMoment = 20;

            if (explicitScheme == 1)
            {
                if (twoFirstApproximation == 1)
                {
                    plotsData.Add(ConstructErrorData("Явная 2Т.1П.", nMin, stepSize, steps, t, k, timeMoment, 1, 1));
                }
                if (threeSecondApproximation == 1)
                {
                    plotsData.Add(ConstructErrorData("Явная 3Т.2П.", nMin, stepSize, steps, t, k, timeMoment, 1, 2));
                }
                if (twoSecondApproximation == 1)
                {
                    plotsData.Add(ConstructErrorData("Явная 2Т.2П.", nMin, stepSize, steps, t, k, timeMoment, 1, 3));
                }
            }
            if (implicitScheme == 1)
            {
                if (twoFirstApproximation == 1)
                {
                    plotsData.Add(ConstructErrorData("Неявная 2Т.1П.", nMin, stepSize, steps, t, k, timeMoment, 2, 1));
                }
                if (threeSecondApproximation == 1)
                {
                    plotsData.Add(ConstructErrorData("Неявная 3Т.2П.", nMin, stepSize, steps, t, k, timeMoment, 2, 2));
                }
                if (twoSecondApproximation == 1)
                {
                    plotsData.Add(ConstructErrorData("Неявная 2Т.2П.", nMin, stepSize, steps, t, k, timeMoment, 2, 3));
                }
            }
            if (cnScheme == 1)
            {
                if (twoFirstApproximation == 1)
                {
                    plotsData.Add(ConstructErrorData("Кранка-Никлсона 2Т.1П.", nMin, stepSize, steps, t, k, timeMoment, 3, 1));
                }
                if (threeSecondApproximation == 1)
                {
                    plotsData.Add(ConstructErrorData("Кранка-Никлсона 3Т.2П.", nMin, stepSize, steps, t, k, timeMoment, 3, 2));
                }
                if (twoSecondApproximation == 1)
                {
                    plotsData.Add(ConstructErrorData("Кранка-Никлсона 2Т.2П.", nMin, stepSize, steps, t, k, timeMoment, 3, 3));
                }
            }
            return plotsData;
        }

        private PlotData ConstructErrorData (string title, int nMin, int stepSize, int steps, double t, int k, int timePoint, int scheme, int approximation)
        {
            PlotData plotData = new();
            plotData.title = title;
            double[,] errorPoints = new double[2, steps];
            double thao = t / k;

            for (int s = 0; s < steps; s++)
            {
                int n = nMin + s * stepSize;
                double hX = Values.l / n;
                double[,] eU = model.FiniteDifferenceSolve(xMax, n, t, k, scheme, approximation);

                double errorSumm = 0;
                int errorPointsAmount = 0;
                for (int i = 0; i <= n; i++)
                {
                    errorSumm += Math.Pow(Values.AnaliticalU(i * hX, timePoint * thao) - eU[timePoint, i], 2);
                    errorPointsAmount += 1;
                }
                //MessageBox.Show($"Шаг = {hX} Ошибка = {errorSumm / errorPointsAmount} количество точек {errorPointsAmount} {steps - s - 1}");
                errorPoints[0, steps - s - 1] = hX;
                errorPoints[1, steps - s - 1] = errorSumm / errorPointsAmount;
            }

            plotData.U = errorPoints;
            return plotData;
        }

        private double AnaliticalU (double x)
        {
            return Values.AnaliticalU(x, selectedTimeMoment);
        }

        private void UpdateGridPar ()
        {
            Thao = rightTimeLimit / k;
            H = xMax / n;
            TicFrequency = rightTimeLimit / 10;
        }

        private PlotModel plotModel;
        private FiniteDifferenceModel model;
        private List<PlotData> plots;

        private double rightTimeLimit = 0.5f;
        private int k;
        private double thao;

        private double xMax = Values.l;
        private int n;
        private double h;

        private double ticFrequency;
        private double selectedTimeMoment = 0f;
        private int selectedTimePoint;

        private int explicitScheme = 0;
        private int implicitScheme = 0;
        private int cnScheme = 0;

        private int twoFirstApproximation = 0;
        private int threeSecondApproximation = 0;
        private int twoSecondApproximation = 0;

        private int hError = 0;
        private int thaoError = 0;

        private string settingsVisible;
    }
}
