using Lab_7.Core;
using Lab_7.DefaultValues;
using Lab_7.MVVM.Model;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using SciChart.Charting3D.Model;
using System.Windows;

namespace Lab_7.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public RelayCommand ToggleLibmanMethodCommand { get; set; }
        public RelayCommand ToggleZeidelMethodCommand { get; set; }
        public RelayCommand ToggleLibmanRelaxedMethodCommand { get; set; }

        public RelayCommand ToggleHxErrorCommand { get; set; }
        public RelayCommand ToggleHyErrorCommand { get; set; }

        public RelayCommand TrueFxyCommand { get; set; }
        public RelayCommand ErrorFxyUxyCommand { get; set; }

        public RelayCommand SolveCommand { get; set; }
        public RelayCommand PopOutCommand { get; set; }

        public int NX
        {
            get { return nX; }
            set { nX = value; UpdateGridPar(); OnPropertyChanged(); }
        }
        public double HX
        {
            get { return hX; }
            set { hX = value; OnPropertyChanged(); }
        }

        public int NY
        {
            get { return nY; }
            set { nY = value; UpdateGridPar(); OnPropertyChanged(); }
        }
        public double HY
        {
            get { return hY; }
            set { hY = value; OnPropertyChanged(); }
        }

        public double Omega
        {
            get { return omega; }
            set { omega = value; OnPropertyChanged(); }
        }
        public double Epsilon
        {
            get { return epsilon; }
            set { epsilon = value; OnPropertyChanged(); }
        }

        public string SettingsVisible
        {
            get { return settingsVisible; }
            set { settingsVisible = value; OnPropertyChanged(); }
        }
        public string PlotVisible
        {
            get { return plotVisible; }
            set { plotVisible = value; OnPropertyChanged(); }
        }

        public int EndStep
        {
            get { return endStep; }
            set { endStep = value; OnPropertyChanged(); }
        }


        public UniformGridDataSeries3D<double> MeshDataSeries
        {
            get { return meshDataSeries; }
            set { meshDataSeries = value; OnPropertyChanged(); }
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

            libmanMethod = 1;
            SettingsVisible = "Visible";
            PlotVisible = "Hidden";

            NX = 10;
            NY = 10;
            Epsilon = 0.001f;
            Omega = 1.7f;

            ToggleLibmanMethodCommand = new RelayCommand(o =>
            {
                if (libmanMethod == 1)
                {
                    libmanMethod = 0;
                }
                else
                {
                    libmanMethod = 1;
                }
            });
            ToggleZeidelMethodCommand = new RelayCommand(o =>
            {
                if (zeidelMethod == 1)
                {
                    zeidelMethod = 0;
                }
                else
                {
                    zeidelMethod = 1;
                }
            });
            ToggleLibmanRelaxedMethodCommand = new RelayCommand(o =>
            {
                if (libmanRelaxedMethod == 1)
                {
                    libmanRelaxedMethod = 0;
                }
                else
                {
                    libmanRelaxedMethod = 1;
                }
            });

            ToggleHxErrorCommand = new RelayCommand(o =>
            {
                if (hXError == 1)
                {
                    if (hYError == 0)
                    {
                        SettingsVisible = "Visible";
                        PlotVisible = "Hidden";
                    }
                    hXError = 0;
                }
                else
                {
                    SettingsVisible = "Hidden";
                    PlotVisible = "Visible";
                    hXError = 1;
                }
            });
            ToggleHyErrorCommand = new RelayCommand(o =>
            {
                if (hYError == 1)
                {
                    if (hXError == 0)
                    {
                        SettingsVisible = "Visible";
                        PlotVisible = "Hidden";
                    }
                    hYError = 0;
                }
                else
                {
                    SettingsVisible = "Hidden";
                    PlotVisible = "Visible";
                    hYError = 1;
                }
            });

            SolveCommand = new RelayCommand(o =>
            {
                if (libmanMethod + zeidelMethod + libmanRelaxedMethod > 1)
                {
                    MessageBox.Show("Сравнение методов решения невозможно");
                }
                else if (hX == 0 | hY == 0)
                {
                    MessageBox.Show("Сеточные параметры не могут быть равны 0");
                }
                else if (hXError + hYError > 1)
                {
                    MessageBox.Show("Изучение зависимости ошибки от hX и hY одновременно невозможно");
                }
                else
                {
                    try
                    {
                        if (hXError + hYError == 0)
                        {
                            UpdatePlot3D();
                        }
                        else
                        {
                            plots = CalculateHErrorPoints2D();
                            UpdatePlot2D(plotModel);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            });
            TrueFxyCommand = new RelayCommand(o =>
            {
                UpdateAnaliticalPlot3D();
            });
            ErrorFxyUxyCommand = new RelayCommand(o =>
            {
                UpdateErrorPlot3D();
            });
            /*PopOutCommand = new RelayCommand(o =>
            {
                string title = "";
                if (hXError + hYError == 0)
                {
                    title = string.Format("графики фунцкий U(x,y)");
                }
                else if (hXError == 1)
                {
                    title = "График звисимости погрешности от сеточного параметра hX";
                }
                else if (hYError == 1)
                {
                    title = "График звисимости погрешности от сеточного параметра hY";
                }
                PlotModel pm = new PlotModel();
                UpdatePlot(pm);
                PlotInspectorViewModel plotInspectorViewModel = new(pm, title);
                PlotInspector plotInspector = new PlotInspector
                {
                    DataContext = plotInspectorViewModel
                };
                plotInspector.Show();
            });*/
        }

        private PlotModel InitializePlot ()
        {
            PlotModel plot = new();

            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                TitleFontSize = 18,
                Title = "h"
            };
            var uAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                TitleFontSize = 18,
                Title = "Error"
            };

            plot.Axes.Add(xAxis);
            plot.Axes.Add(uAxis);

            return plot;
        }

        private void UpdatePlot2D (PlotModel plotModel)
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

            if (hXError == 1)
            {
                var xAxis = new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    TitleFontSize = 18,
                    Title = "hX"
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
            else if (hYError == 1)
            {
                var xAxis = new LinearAxis
                {
                    Position = AxisPosition.Bottom,
                    TitleFontSize = 18,
                    Title = "hY"
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

            plotModel.InvalidatePlot(true);
        }

        private void UpdatePlot3D ()
        {
            if (libmanMethod == 1)
            {
                Tuple<double[,], int> result = model.FiniteDifferenceSolve(nX, nY, epsilon, 1, omega);
                double[,] U = result.Item1;
                EndStep = result.Item2;
                var meshDataSeries = new UniformGridDataSeries3D<double>(nX + 1, nY + 1)
                {
                    StartX = 0,
                    StepX = hX,
                    StartZ = 0,
                    StepZ = hY,
                    SeriesName = "График ошибок для метода Либмана",
                };
                for (int x = 0; x <= nX; x++)
                {
                    for (int y = 0; y <= nY; y++)
                    {
                        meshDataSeries[y, x] = U[x, y];
                    }
                }
                MeshDataSeries = meshDataSeries;
            }
            if (zeidelMethod == 1)
            {
                Tuple<double[,], int> result = model.FiniteDifferenceSolve(nX, nY, epsilon, 2, omega);
                double[,] U = result.Item1;
                EndStep = result.Item2;
                var meshDataSeries = new UniformGridDataSeries3D<double>(nX + 1, nY + 1)
                {
                    StartX = 0,
                    StepX = hX,
                    StartZ = 0,
                    StepZ = hY,
                    SeriesName = "График ошибок для метода Зейделя",
                };
                for (int x = 0; x <= nX; x++)
                {
                    for (int y = 0; y <= nY; y++)
                    {
                        meshDataSeries[y, x] = U[x, y];
                    }
                }
                MeshDataSeries = meshDataSeries;
            }
            if (libmanRelaxedMethod == 1)
            {
                Tuple<double[,], int> result = model.FiniteDifferenceSolve(nX, nY, epsilon, 3, omega);
                double[,] U = result.Item1;
                EndStep = result.Item2;
                var meshDataSeries = new UniformGridDataSeries3D<double>(nX + 1, nY + 1)
                {
                    StartX = 0,
                    StepX = hX,
                    StartZ = 0,
                    StepZ = hY,
                    SeriesName = "График ошибок для метода Либмана рел.",
                };
                for (int x = 0; x <= nX; x++)
                {
                    for (int y = 0; y <= nY; y++)
                    {
                        meshDataSeries[y, x] = U[x, y];
                    }
                }
                MeshDataSeries = meshDataSeries;
            }
        }

        private void UpdateAnaliticalPlot3D ()
        {
            var meshDataSeries = new UniformGridDataSeries3D<double>(nX + 1, nY + 1)
            {
                StartX = 0,
                StepX = hX,
                StartZ = 0,
                StepZ = hY,
                SeriesName = "Аналитическое решение",
            };
            for (int x = 0; x <= nX; x++)
            {
                for (int y = 0; y <= nY; y++)
                {
                    meshDataSeries[y, x] = Values.AnaliticalU(hX * x, hY * y);
                }
            }
            MeshDataSeries = meshDataSeries;
        }

        private void UpdateErrorPlot3D ()
        {
            if (libmanMethod == 1)
            {
                double[,] U = model.FiniteDifferenceSolve(nX, nY, epsilon, 1, omega).Item1;
                var meshDataSeries = new UniformGridDataSeries3D<double>(nX + 1, nY + 1)
                {
                    StartX = 0,
                    StepX = hX,
                    StartZ = 0,
                    StepZ = hY,
                    SeriesName = "График ошибок для метода Либмана",
                };
                for (int x = 0; x <= nX; x++)
                {
                    for (int y = 0; y <= nY; y++)
                    {
                        meshDataSeries[y, x] = Math.Abs(Values.AnaliticalU(hX * x, hY * y) - U[x, y]);
                    }
                }
                MeshDataSeries = meshDataSeries;
            }
            if (zeidelMethod == 1)
            {
                double[,] U = model.FiniteDifferenceSolve(nX, nY, epsilon, 2, omega).Item1;
                var meshDataSeries = new UniformGridDataSeries3D<double>(nX + 1, nY + 1)
                {
                    StartX = 0,
                    StepX = hX,
                    StartZ = 0,
                    StepZ = hY,
                    SeriesName = "График ошибок для метода Зейделя",
                };
                for (int x = 0; x <= nX; x++)
                {
                    for (int y = 0; y <= nY; y++)
                    {
                        meshDataSeries[y, x] = Math.Abs(Values.AnaliticalU(hX * x, hY * y) - U[x, y]);
                    }
                }
                MeshDataSeries = meshDataSeries;
            }
            if (libmanRelaxedMethod == 1)
            {
                double[,] U = model.FiniteDifferenceSolve(nX, nY, epsilon, 3, omega).Item1;
                var meshDataSeries = new UniformGridDataSeries3D<double>(nX + 1, nY + 1)
                {
                    StartX = 0,
                    StepX = hX,
                    StartZ = 0,
                    StepZ = hY,
                    SeriesName = "График ошибок для метода Либмана рел.",
                };
                for (int x = 0; x <= nX; x++)
                {
                    for (int y = 0; y <= nY; y++)
                    {
                        meshDataSeries[y, x] = Math.Abs(Values.AnaliticalU(hX * x, hY * y) - U[x, y]);
                    }
                }
                MeshDataSeries = meshDataSeries;
            }
        }

        private List<PlotData> CalculateHErrorPoints2D ()
        {
            List<PlotData> plotsData = new();
            int n1 = 3;
            int n2 = 40;
            int power = 4;
            int slice = 10;

            if (hXError == 1)
            {
                if (libmanMethod == 1)
                {
                    plotsData.Add(ConstructErrorData(1, "Либман", n1, n2, power, slice, 1));
                }
                if (zeidelMethod == 1)
                {
                    plotsData.Add(ConstructErrorData(1, "Зейдель", n1, n2, power, slice, 2));
                }
                if (libmanRelaxedMethod == 1)
                {
                    plotsData.Add(ConstructErrorData(1, "Либман рел.", n1, n2, power, slice, 3));
                }
            }
            if (hYError == 1)
            {
                if (libmanMethod == 1)
                {
                    plotsData.Add(ConstructErrorData(2, "Либман", n1, n2, power, slice, 1));
                }
                if (zeidelMethod == 1)
                {
                    plotsData.Add(ConstructErrorData(2, "Зейдель", n1, n2, power, slice, 2));
                }
                if (libmanRelaxedMethod == 1)
                {
                    plotsData.Add(ConstructErrorData(2, "Либман рел.", n1, n2, power, slice, 3));
                }
            }
            return plotsData;
        }

        private PlotData ConstructErrorData (int mode, string title, int n1, int n2, int power, int slice, int solver)
        {
            PlotData plotData = new();
            plotData.title = title;
            double[,] errorPoints = new double[2, power + 1];

            if (mode == 1)
            {
                for (int i = power; i >= 0; i--)
                {
                    double[,] U = model.FiniteDifferenceSolve(n1 * (int) Math.Pow(2, i), n2, epsilon, solver, omega).Item1;
                    double hx = Values.lX / (n1 * Math.Pow(2, i));
                    errorPoints[0, power - i] = hx;
                    errorPoints[1, power - i] = Math.Abs(U[(int) Math.Pow(2, i), slice] - Values.AnaliticalU(Math.Pow(2, i) * hx, slice * (Values.lY / n2)));
                }
            }
            else
            {
                for (int i = power; i >= 0; i--)
                {
                    double[,] U = model.FiniteDifferenceSolve(n2, n1 * (int) Math.Pow(2, i), epsilon, solver, omega).Item1;
                    double hy = Values.lY / (n1 * Math.Pow(2, i));
                    errorPoints[0, power - i] = hy;
                    errorPoints[1, power - i] = Math.Abs(U[slice, (int) Math.Pow(2, i)] - Values.AnaliticalU(slice * (Values.lX / n2), Math.Pow(2, i) * hy));
                }
            }

            plotData.U = errorPoints;
            return plotData;
        }

        private void UpdateGridPar ()
        {
            HX = xMax / nX;
            HY = yMax / nY;
        }

        private PlotModel plotModel;
        private UniformGridDataSeries3D<double> meshDataSeries;
        private FiniteDifferenceModel model;
        private List<PlotData> plots;

        private double xMax = Values.lX;
        private int nX;
        private double hX;

        private double yMax = Values.lY;
        private int nY;
        private double hY;

        private double omega;
        private double epsilon;

        private int libmanMethod = 0;
        private int zeidelMethod = 0;
        private int libmanRelaxedMethod = 0;

        private int hXError = 0;
        private int hYError = 0;

        private string settingsVisible;
        private string plotVisible;
        private int endStep;
    }
}
