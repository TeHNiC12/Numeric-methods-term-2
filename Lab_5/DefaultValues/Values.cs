using System;

namespace Lab_5.DefaultValues
{
    public static class Values
    {
        public static double a = 1f;
        public static double b = 0f;
        public static double c = -3f;

        public static int alpha = 1;
        public static int beta = 0;
        public static int gamma = 0;
        public static int eta = 1;

        public static double tMin = 0;
        public static double tMax = 1;

        public static double xMin = 0;
        public static double xMax = Math.PI;

        public static Func<double, double, double> AnaliticalU = (x, t) => Math.Exp(-4f * t) * Math.Sin(x);

        public static Func<double, double, double> fX = (x, t) => 0;
        public static Func<double, double> phi1 = t => Math.Exp(-4f * t);
        public static Func<double, double> phi2 = t => Math.Exp(-4f * t);
        public static Func<double, double> psi = x => Math.Sin(x);
    }
}
