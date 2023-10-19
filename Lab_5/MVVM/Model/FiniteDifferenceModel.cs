using System;

namespace Lab_5.MVVM.Model
{
    public class FiniteDifferenceModel
    {
        public double XMin
        {
            get { return xMin; }
            set { xMin = value; }
        }

        public double XMax
        {
            get { return xMax; }
            set { xMax = value; }
        }


        public Func<double, double, double> AnaliticalU = (x, t) => Math.Exp(-4f * t) * Math.Sin(x);

        private double xMin = 0;
        private double xMax = Math.PI;
    }
}
