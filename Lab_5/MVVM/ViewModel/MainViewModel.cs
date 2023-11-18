using Lab_5.Core;
using Lab_5.DefaultValues;
using Lab_5.MVVM.Model;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;

namespace Lab_5.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public RelayCommand ToggleExplicitMethodCommand { get; set; }
        public RelayCommand ToggleImplicitMethodCommand { get; set; }
        public RelayCommand ToggleCrancNicolsonMethodCommand { get; set; }
        public RelayCommand ToggleTwoPointFirstOrderCommand { get; set; }
        public RelayCommand ToggleTwoPointSecondOrderCommand { get; set; }
        public RelayCommand ToggleThreePointSecondOrderCommand { get; set; }
        public RelayCommand SolveCommand { get; set; }

        public double LeftTimeLimit
        {
            get { return leftTimeLimit; }
            set { leftTimeLimit = value; OnPropertyChanged(); }
        }

        public double RightTimeLimit
        {
            get { return rightTimeLimit; }
            set { rightTimeLimit = value; OnPropertyChanged(); }
        }

        public double TicFrequency
        {
            get { return ticFrequency; }
            set { ticFrequency = value; OnPropertyChanged(); }
        }

        public double SelectedErrorTime
        {
            get { return selectedErrorTime; }
            set { selectedErrorTime = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; OnPropertyChanged(); }
        }

        public string ErrorValue
        {
            get { return errorValue; }
            set { errorValue = value; OnPropertyChanged(); }
        }

        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; OnPropertyChanged(); }
        }

        public MainViewModel ()
        {
            model = new();
            plotModel = new();

            ticFrequency = (rightTimeLimit - leftTimeLimit) / 10;

            ToggleExplicitMethodCommand = new RelayCommand(o =>
            {
                finiteDifferenceMethod = 1;
            });
            ToggleImplicitMethodCommand = new RelayCommand(o =>
            {
                finiteDifferenceMethod = 2;
            });
            ToggleCrancNicolsonMethodCommand = new RelayCommand(o =>
            {
                finiteDifferenceMethod = 3;
            });
            ToggleTwoPointFirstOrderCommand = new RelayCommand(o =>
            {
                approximationMethod = 1;
            });
            ToggleTwoPointSecondOrderCommand = new RelayCommand(o =>
            {
                approximationMethod = 2;
            });
            ToggleThreePointSecondOrderCommand = new RelayCommand(o =>
            {
                approximationMethod = 3;
            });
            SolveCommand = new RelayCommand(o =>
            {
                UpdatePlot();
                UpdateErrorMessage();
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

        private void UpdateErrorMessage ()
        {
            ErrorMessage = String.Format("Погрешность метода в момент времени t={0} составляет:", Math.Round(SelectedErrorTime, 3).ToString());
        }

        private void UpdatePlot ()
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
                Title = "Y"
            };

            plotModel.Legends.Add(new Legend
            {
                LegendPosition = LegendPosition.TopLeft,
                LegendPlacement = LegendPlacement.Inside
            });

            plotModel.Annotations.Add(zeroLineX);
            plotModel.Annotations.Add(zeroLineY);
            plotModel.Axes.Add(xAxis);
            plotModel.Axes.Add(yAxis);

            var scatterSeries = new ScatterSeries
            {
                MarkerType = MarkerType.Circle,
                MarkerSize = 6,
                MarkerStroke = OxyColors.Blue,
                Title = "Interpolation points"
            };

            plotModel.Series.Add(new FunctionSeries(analiticalU, Values.xMin, Values.xMax, 0.001f, "Аналитическая функция"));

            plotModel.InvalidatePlot(true);
        }

        private double analiticalU (double x)
        {
            return Values.AnaliticalU(x, SelectedErrorTime);
        }

        private FiniteDifferenceModel model;

        private double leftTimeLimit = Values.tMin;
        private double rightTimeLimit = Values.tMax;
        private double ticFrequency;
        private double selectedErrorTime = 0;

        private string errorMessage;
        private string errorValue;

        private int finiteDifferenceMethod = 1;
        private int approximationMethod = 1;

        private double xMin = Values.xMin;
        private double xMax = Values.xMax;
    }
}
