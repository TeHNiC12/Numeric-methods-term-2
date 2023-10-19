using Lab_5.Core;
using Lab_5.MVVM.Model;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;

namespace Lab_5.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
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


        public PlotModel Plot { get; private set; }

        public RelayCommand ToggleExplicitMethodCommand { get; set; }
        public RelayCommand ToggleImplicitMethodCommand { get; set; }
        public RelayCommand ToggleCrancNicolsonMethodCommand { get; set; }
        public RelayCommand ToggleTwoPointFirstOrderCommand { get; set; }
        public RelayCommand ToggleTwoPointSecondOrderCommand { get; set; }
        public RelayCommand ToggleThreePointSecondOrderCommand { get; set; }
        public RelayCommand SolveCommand { get; set; }


        public MainViewModel ()
        {
            model = new();
            Plot = InitializePlot();
            UpdateErrorMessage();

            xMin = model.XMin;
            xMax = model.XMax;


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

        private FunctionSeries GetAnaliticalUPlot ()
        {
            FunctionSeries series = new(analiticalU, xMin, xMax, 0.01f);
            return series;
        }

        private void UpdatePlot ()
        {
            Plot.Series.Clear();

            Plot.Series.Add(GetAnaliticalUPlot());

            Plot.InvalidatePlot(true);
        }

        private double analiticalU (double x)
        {
            return model.AnaliticalU(x, SelectedErrorTime);
        }

        private void UpdateErrorMessage ()
        {
            ErrorMessage = String.Format("Погрешность метода в момент времени t={0} составляет:", Math.Round(SelectedErrorTime, 3).ToString());
        }

        private FiniteDifferenceModel model;

        private double leftTimeLimit = -1;
        private double rightTimeLimit = 1;
        private double selectedErrorTime = 0;

        private string errorMessage;

        private int finiteDifferenceMethod = 1;
        private int approximationMethod = 1;

        private double xMin;
        private double xMax;
    }
}
