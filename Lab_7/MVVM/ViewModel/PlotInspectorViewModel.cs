using Lab_7.Core;
using OxyPlot;

namespace Lab_7.MVVM.ViewModel
{
    public class PlotInspectorViewModel : ObservableObject
    {
        private PlotModel? plotModel;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; OnPropertyChanged(); }
        }

        private string? title;
        public string Title
        {
            get { return title; }
            set { title = value; OnPropertyChanged(); }
        }

        public PlotInspectorViewModel () { }

        public PlotInspectorViewModel (PlotModel plotModel, string title)
        {
            this.plotModel = plotModel;
            this.title = title;
        }
    }
}
