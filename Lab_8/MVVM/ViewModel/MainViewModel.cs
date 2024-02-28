using Lab_8.Core;
using Lab_8.DefaultValues;
using Lab_8.MVVM.Model;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using SciChart.Charting3D.Model;
using System.Windows;

namespace Lab_8.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public RelayCommand ToggleCDSchemeCommand { get; set; }
        public RelayCommand TogglePSSchemeCommand { get; set; }

        public RelayCommand ToggleHxErrorCommand { get; set; }
        public RelayCommand ToggleHyErrorCommand { get; set; }

        public RelayCommand ToggleTrueFxyCommand { get; set; }
        public RelayCommand ToggleErrorFxyUxyCommand { get; set; }

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
        public double T
        {
            get { return t; }
            set { t = value; UpdateGridPar(); OnPropertyChanged(); }
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
                try
                {
                    UpdatePlot3D();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
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

            cDScheme = 1;
            SettingsVisible = "Visible";
            PlotVisible = "Hidden";

            NX = 10;
            NY = 10;
            T = 1f;
            K = 10;

            ToggleCDSchemeCommand = new RelayCommand(o =>
            {
                if (cDScheme == 1)
                {
                    cDScheme = 0;
                }
                else
                {
                    cDScheme = 1;
                }
            });
            TogglePSSchemeCommand = new RelayCommand(o =>
            {
                if (pSScheme == 1)
                {
                    pSScheme = 0;
                }
                else
                {
                    pSScheme = 1;
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

            ToggleTrueFxyCommand = new RelayCommand(o =>
            {
                if (trueFxy == 1)
                {
                    trueFxy = 0;
                }
                else
                {
                    trueFxy = 1;
                }
            });
            ToggleErrorFxyUxyCommand = new RelayCommand(o =>
            {
                if (errorFxyUxy == 1)
                {
                    errorFxyUxy = 0;
                }
                else
                {
                    errorFxyUxy = 1;
                }
            });

            SolveCommand = new RelayCommand(o =>
            {
                if (cDScheme + pSScheme > 1)
                {
                    MessageBox.Show("Сравнение методов решения невозможно");
                }
                else if ((hXError + hYError > 0) & (trueFxy + errorFxyUxy > 0))
                {
                    MessageBox.Show("Выберите опцию только из одного раздела");
                }
                else if ((cDScheme + pSScheme > 0) & trueFxy == 1)
                {
                    MessageBox.Show("Построение графика решения и аналитического графика одновременно невозможно");
                }
                else if (trueFxy + errorFxyUxy > 1)
                {
                    MessageBox.Show("Построения аналитического решения и графика ошибки одновременно невозможно");
                }
                else if (hX == 0 | hY == 0 | thao == 0)
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
                            U1 = model.FiniteDifferenceSolve(nX, nY, k, t, 1);
                            U2 = model.FiniteDifferenceSolve(nX, nY, k, t, 2);
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
            if (cDScheme == 1)
            {
                if (errorFxyUxy == 1)
                {
                    if (U1 == null)
                    {
                        throw new Exception("Нажмите кнопку Решить");
                    }
                    var meshDataSeries = new UniformGridDataSeries3D<double>(nX + 1, nY + 1)
                    {
                        StartX = 0,
                        StepX = hX,
                        StartZ = 0,
                        StepZ = hY,
                        SeriesName = $"График ошибок для метода переменных направлений в момент времени t = {selectedTimeMoment}",
                    };
                    for (int x = 0; x <= nX; x++)
                    {
                        for (int y = 0; y <= nY; y++)
                        {
                            meshDataSeries[y, x] = Math.Abs(Values.AnaliticalU(hX * x, hY * y, selectedTimePoint * thao) - U1[x, y, selectedTimePoint]);
                        }
                    }
                    MeshDataSeries = meshDataSeries;
                }
                else
                {
                    if (U1 == null)
                    {
                        throw new Exception("Нажмите кнопку Решить");
                    }
                    var meshDataSeries = new UniformGridDataSeries3D<double>(nX + 1, nY + 1)
                    {
                        StartX = 0,
                        StepX = hX,
                        StartZ = 0,
                        StepZ = hY,
                        SeriesName = $"График решения методом переменных направлений в момент времени t = {selectedTimeMoment}",
                    };
                    for (int x = 0; x <= nX; x++)
                    {
                        for (int y = 0; y <= nY; y++)
                        {
                            meshDataSeries[y, x] = U1[x, y, selectedTimePoint];
                        }
                    }
                    MeshDataSeries = meshDataSeries;
                }
            }
            else if (pSScheme == 1)
            {
                if (errorFxyUxy == 1)
                {
                    if (U2 == null)
                    {
                        throw new Exception("Нажмите кнопку Решить");
                    }
                    var meshDataSeries = new UniformGridDataSeries3D<double>(nX + 1, nY + 1)
                    {
                        StartX = 0,
                        StepX = hX,
                        StartZ = 0,
                        StepZ = hY,
                        SeriesName = $"График ошибок для метода переменных направлений в момент времени t = {selectedTimeMoment}",
                    };
                    for (int x = 0; x <= nX; x++)
                    {
                        for (int y = 0; y <= nY; y++)
                        {
                            meshDataSeries[y, x] = Math.Abs(Values.AnaliticalU(hX * x, hY * y, selectedTimePoint * thao) - U2[x, y, selectedTimePoint]);
                        }
                    }
                    MeshDataSeries = meshDataSeries;
                }
                else
                {
                    if (U2 == null)
                    {
                        throw new Exception("Нажмите кнопку Решить");
                    }
                    var meshDataSeries = new UniformGridDataSeries3D<double>(nX + 1, nY + 1)
                    {
                        StartX = 0,
                        StepX = hX,
                        StartZ = 0,
                        StepZ = hY,
                        SeriesName = $"График решения методом переменных направлений в момент времени t = {selectedTimeMoment}",
                    };
                    for (int x = 0; x <= nX; x++)
                    {
                        for (int y = 0; y <= nY; y++)
                        {
                            meshDataSeries[y, x] = U2[x, y, selectedTimePoint];
                        }
                    }
                    MeshDataSeries = meshDataSeries;
                }
            }
            else if (trueFxy == 1)
            {
                var meshDataSeries = new UniformGridDataSeries3D<double>(nX + 1, nY + 1)
                {
                    StartX = 0,
                    StepX = hX,
                    StartZ = 0,
                    StepZ = hY,
                    SeriesName = $"Аналитическое решение в момент времени t = {selectedTimeMoment}",
                };
                for (int x = 0; x <= nX; x++)
                {
                    for (int y = 0; y <= nY; y++)
                    {
                        meshDataSeries[y, x] = Values.AnaliticalU(hX * x, hY * y, selectedTimePoint * thao);
                    }
                }
                MeshDataSeries = meshDataSeries;
            }
            else
            {
                throw new Exception("Действие не выбрано");
            }
        }

        private List<PlotData> CalculateHErrorPoints2D ()
        {
            List<PlotData> plotsData = new();
            int nMin = 10;
            int stepSize = 10;
            int steps = 10;
            int n2 = 10;
            double t = 1f;
            int k = 10;
            int timeMoment = 3;


            if (hXError == 1)
            {
                if (cDScheme == 1)
                {
                    plotsData.Add(ConstructErrorData(1, "Переменные направления", nMin, stepSize, steps, n2, t, k, timeMoment, 1));
                }
                if (pSScheme == 1)
                {
                    plotsData.Add(ConstructErrorData(1, "Дробные шаги", nMin, stepSize, steps, n2, t, k, timeMoment, 2));
                }
            }
            if (hYError == 1)
            {
                if (cDScheme == 1)
                {
                    plotsData.Add(ConstructErrorData(2, "Переменные направления", nMin, stepSize, steps, n2, t, k, timeMoment, 1));
                }
                if (pSScheme == 1)
                {
                    plotsData.Add(ConstructErrorData(2, "Дробные шаги", nMin, stepSize, steps, n2, t, k, timeMoment, 2));
                }
            }
            return plotsData;
        }

        private PlotData ConstructErrorData (int mode, string title, int nMin, int stepSize, int steps, int n2, double t, int k, int timePoint, int solver)
        {
            PlotData plotData = new();
            plotData.title = title;
            double[,] errorPoints = new double[2, steps];
            double thao = t / k;

            if (mode == 1)
            {
                double hY = Values.lY / n2;
                for (int s = 0; s < steps; s++)
                {
                    int n = nMin + s * stepSize;
                    double hX = Values.lX / n;
                    double[,,] eU = model.FiniteDifferenceSolve(n, n2, k, t, solver);

                    double errorSumm = 0;
                    int errorPointsAmount = 0;
                    for (int i = 0; i <= n; i++)
                    {
                        for (int j = 0; j <= n2; j++)
                        {
                            errorSumm += Math.Pow(Values.AnaliticalU(i * hX, j * hY, timePoint * thao) - eU[i, j, timePoint], 2);
                            errorPointsAmount += 1;
                        }
                    }
                    //MessageBox.Show($"Шаг = {hX} Ошибка = {errorSumm / errorPointsAmount} количество точек {errorPointsAmount} {steps - s - 1}");
                    errorPoints[0, steps - s - 1] = hX;
                    errorPoints[1, steps - s - 1] = errorSumm / errorPointsAmount;
                    //errorPoints[1, s] = errorSumm / errorPointsAmount;
                }
            }
            else
            {
                double hX = Values.lX / n2;
                for (int s = 0; s < steps; s++)
                {
                    int n = nMin + s * stepSize;
                    double hY = Values.lY / n;
                    double[,,] eU = model.FiniteDifferenceSolve(n2, n, k, t, solver);

                    double errorSumm = 0;
                    int errorPointsAmount = 0;
                    for (int i = 0; i <= n2; i++)
                    {
                        for (int j = 0; j <= n; j++)
                        {
                            errorSumm += Math.Pow(Values.AnaliticalU(i * hX, j * hY, timePoint * thao) - eU[i, j, timePoint], 2);
                            errorPointsAmount += 1;
                        }
                    }
                    //MessageBox.Show($"Шаг = {hY} Ошибка = {errorSumm / errorPointsAmount} количество точек {errorPointsAmount} {steps - s - 1}");
                    errorPoints[0, steps - s - 1] = hY;
                    errorPoints[1, steps - s - 1] = errorSumm / errorPointsAmount;
                    //errorPoints[1, s] = errorSumm / errorPointsAmount;
                }
            }

            plotData.U = errorPoints;
            return plotData;
        }

        private void UpdateGridPar ()
        {
            HX = xMax / nX;
            HY = yMax / nY;
            Thao = t / k;
            TicFrequency = T / 10;
        }

        private PlotModel plotModel;
        private UniformGridDataSeries3D<double> meshDataSeries;
        private FiniteDifferenceModel model;
        private List<PlotData> plots;

        private double[,,] U1;
        private double[,,] U2;

        private double xMax = Values.lX;
        private int nX;
        private double hX;

        private double yMax = Values.lY;
        private int nY;
        private double hY;

        private double t;
        private double thao;
        private int k;

        private double ticFrequency;
        private double selectedTimeMoment = 0f;
        private int selectedTimePoint;

        private int cDScheme = 0;
        private int pSScheme = 0;

        private int hXError = 0;
        private int hYError = 0;

        private int trueFxy = 0;
        private int errorFxyUxy = 0;

        private string settingsVisible;
        private string plotVisible;
    }
}
