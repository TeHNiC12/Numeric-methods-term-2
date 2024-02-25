using Lab_6.Core;
using Lab_6.DefaultValues;
using Lab_6.MVVM.Model;
using Lab_6.MVVM.View;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System.Windows;

namespace Lab_6.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public RelayCommand ToggleExplicitMethodCommand { get; set; }
        public RelayCommand ToggleImplicitMethodCommand { get; set; }

        public RelayCommand ToggleTwoPointFirstOrderCommand { get; set; }
        public RelayCommand ToggleThreePointSecondOrderCommand { get; set; }
        public RelayCommand ToggleTwoPointSecondOrderCommand { get; set; }

        public RelayCommand ToggleTwoPointFirstOrderStartCommand { get; set; }
        public RelayCommand ToggleTwoPointSecondOrderStartCommand { get; set; }


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
            twoFirstApproximationStart = 1;
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

            ToggleTwoPointFirstOrderStartCommand = new RelayCommand(o =>
            {
                if (twoFirstApproximationStart == 1)
                {
                    twoFirstApproximationStart = 0;
                }
                else
                {
                    twoFirstApproximationStart = 1;
                }
            });
            ToggleTwoPointSecondOrderStartCommand = new RelayCommand(o =>
            {
                if (twoSecondApproximationStart == 1)
                {
                    twoSecondApproximationStart = 0;
                }
                else
                {
                    twoSecondApproximationStart = 1;
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
                if ((explicitScheme + implicitScheme > 1 & twoFirstApproximation + threeSecondApproximation + twoSecondApproximation > 1) | (twoFirstApproximationStart + twoSecondApproximationStart > 1 & twoFirstApproximation + threeSecondApproximation + twoSecondApproximation > 1) | (twoFirstApproximationStart + twoSecondApproximationStart > 1 & explicitScheme + implicitScheme > 1))
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
                    if (twoFirstApproximationStart == 1)
                    {
                        plotsData.Add(ConstructPlotData("Явная 2т.1п. нач:2т.1п.", 1, 1, 1));
                    }
                    if (twoSecondApproximationStart == 1)
                    {
                        plotsData.Add(ConstructPlotData("Явная 2т.1п. нач:2т.2п.", 1, 1, 2));
                    }
                }
                if (threeSecondApproximation == 1)
                {
                    if (twoFirstApproximationStart == 1)
                    {
                        plotsData.Add(ConstructPlotData("Явная 3т.2п. нач:2т.1п.", 1, 2, 1));
                    }
                    if (twoSecondApproximationStart == 1)
                    {
                        plotsData.Add(ConstructPlotData("Явная 3т.2п. нач:2т.2п.", 1, 2, 2));
                    }
                }
                if (twoSecondApproximation == 1)
                {
                    if (twoFirstApproximationStart == 1)
                    {
                        plotsData.Add(ConstructPlotData("Явная 2т.2п. нач:2т.1п.", 1, 3, 1));
                    }
                    if (twoSecondApproximationStart == 1)
                    {
                        plotsData.Add(ConstructPlotData("Явная 2т.2п. нач:2т.2п.", 1, 3, 2));
                    }
                }
            }
            if (implicitScheme == 1)
            {
                if (twoFirstApproximation == 1)
                {
                    if (twoFirstApproximationStart == 1)
                    {
                        plotsData.Add(ConstructPlotData("Неявная 2т.1п. нач:2т.1п.", 2, 1, 1));
                    }
                    if (twoSecondApproximationStart == 1)
                    {
                        plotsData.Add(ConstructPlotData("Неявная 2т.1п. нач:2т.2п.", 2, 1, 2));
                    }
                }
                if (threeSecondApproximation == 1)
                {
                    if (twoFirstApproximationStart == 1)
                    {
                        plotsData.Add(ConstructPlotData("Неявная 3т.2п. нач:2т.1п.", 2, 2, 1));
                    }
                    if (twoSecondApproximationStart == 1)
                    {
                        plotsData.Add(ConstructPlotData("Неявная 3т.2п. нач:2т.2п.", 2, 2, 2));
                    }
                }
                if (twoSecondApproximation == 1)
                {
                    if (twoFirstApproximationStart == 1)
                    {
                        plotsData.Add(ConstructPlotData("Неявная 2т.2п. нач:2т.1п.", 2, 3, 1));
                    }
                    if (twoSecondApproximationStart == 1)
                    {
                        plotsData.Add(ConstructPlotData("Неявная 2т.2п. нач:2т.2п.", 2, 3, 2));
                    }
                }
            }
            return plotsData;
        }
        private List<PlotData> CalculateHErrorPoints ()
        {
            List<PlotData> plotsData = new();
            double T = 0.5;
            int power = 8;
            int maxN = 5 * (int) Math.Pow(2, power);
            int K = (int) (Values.a * T * maxN / Values.l) + 2;
            int tMoment = 30;

            if (explicitScheme == 1)
            {
                if (twoFirstApproximation == 1)
                {
                    if (twoFirstApproximationStart == 1)
                    {
                        plotsData.Add(ConstructErrorData("Явная 2т.1п. нач:2т.1п.", power, tMoment, T, K, 1, 1, 1));
                    }
                    if (twoSecondApproximationStart == 1)
                    {
                        plotsData.Add(ConstructErrorData("Явная 2т.1п. нач:2т.2п.", power, tMoment, T, K, 1, 1, 2));
                    }
                }
                if (threeSecondApproximation == 1)
                {
                    if (twoFirstApproximationStart == 1)
                    {
                        plotsData.Add(ConstructErrorData("Явная 3т.2п. нач:2т.1п.", power, tMoment, T, K, 1, 2, 1));
                    }
                    if (twoSecondApproximationStart == 1)
                    {
                        plotsData.Add(ConstructErrorData("Явная 3т.2п. нач:2т.2п.", power, tMoment, T, K, 1, 2, 2));
                    }
                }
                if (twoSecondApproximation == 1)
                {
                    if (twoFirstApproximationStart == 1)
                    {
                        plotsData.Add(ConstructErrorData("Явная 2т.2п. нач:2т.1п.", power, tMoment, T, K, 1, 3, 1));
                    }
                    if (twoSecondApproximationStart == 1)
                    {
                        plotsData.Add(ConstructErrorData("Явная 2т.2п. нач:2т.2п.", power, tMoment, T, K, 1, 3, 2));
                    }
                }
            }
            if (implicitScheme == 1)
            {
                if (twoFirstApproximation == 1)
                {
                    if (twoFirstApproximationStart == 1)
                    {
                        plotsData.Add(ConstructErrorData("Неявная 2т.1п. нач:2т.1п.", power, tMoment, T, K, 2, 1, 1));
                    }
                    if (twoSecondApproximationStart == 1)
                    {
                        plotsData.Add(ConstructErrorData("Неявная 2т.1п. нач:2т.2п.", power, tMoment, T, K, 2, 1, 2));
                    }
                }
                if (threeSecondApproximation == 1)
                {
                    if (twoFirstApproximationStart == 1)
                    {
                        plotsData.Add(ConstructErrorData("Неявная 3т.2п. нач:2т.1п.", power, tMoment, T, K, 2, 2, 1));
                    }
                    if (twoSecondApproximationStart == 1)
                    {
                        plotsData.Add(ConstructErrorData("Неявная 3т.2п. нач:2т.2п.", power, tMoment, T, K, 2, 2, 2));
                    }
                }
                if (twoSecondApproximation == 1)
                {
                    if (twoFirstApproximationStart == 1)
                    {
                        plotsData.Add(ConstructErrorData("Неявная 2т.2п. нач:2т.1п.", power, tMoment, T, K, 2, 3, 1));
                    }
                    if (twoSecondApproximationStart == 1)
                    {
                        plotsData.Add(ConstructErrorData("Неявная 2т.2п. нач:2т.2п.", power, tMoment, T, K, 2, 3, 2));
                    }
                }
            }
            return plotsData;
        }

        private PlotData ConstructPlotData (string title, int scheme, int approximation, int startApproximation)
        {
            PlotData plotData = new();
            plotData.title = title;
            plotData.U = model.FiniteDifferenceSolve(xMax, n, rightTimeLimit, k, scheme, approximation, startApproximation);
            return plotData;
        }
        private PlotData ConstructErrorData (string title, int power, int tMoment, double T, int K, int scheme, int approximation, int startApproximation)
        {
            PlotData plotData = new();
            plotData.title = "title";
            double[,] errorPoints = new double[2, power + 1];
            for (int i = power; i >= 0; i--)
            {
                double[,] U = model.FiniteDifferenceSolve(xMax, 5 * (int) Math.Pow(2, i), T, K, scheme, approximation, startApproximation);
                double h = xMax / (5 * Math.Pow(2, i));
                errorPoints[0, power - i] = h;
                errorPoints[1, power - i] = Math.Abs(U[tMoment, (int) Math.Pow(2, i)] - Values.AnaliticalU(Math.Pow(2, i) * h, tMoment * (T / K)));
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

        private int twoFirstApproximation = 0;
        private int threeSecondApproximation = 0;
        private int twoSecondApproximation = 0;

        private int twoFirstApproximationStart = 0;
        private int twoSecondApproximationStart = 0;

        private int hError = 0;
        private int thaoError = 0;

        private string settingsVisible;
    }
}
