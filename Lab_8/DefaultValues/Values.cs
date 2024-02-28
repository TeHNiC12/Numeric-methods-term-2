namespace Lab_8.DefaultValues
{
    public static class Values
    {
        public static double a = 1f;
        public static double b = 1f;

        public static Func<double, double, double, double> F = (x, y, t) => -x * y * Math.Sin(t);

        public static double lX = 1;
        public static double lY = 1;

        public static Func<double, double, double> phi1 = (y, t) => 0;
        public static Func<double, double, double> phi2 = (y, t) => y * Math.Cos(t);
        public static Func<double, double, double> phi3 = (x, t) => 0;
        public static Func<double, double, double> phi4 = (x, t) => x * Math.Cos(t);

        public static Func<double, double, double> psi = (x, y) => x * y;

        public static Func<double, double, double, double> AnaliticalU = (x, y, t) => x * y * Math.Cos(t);
    }
}
