namespace Lab_6.DefaultValues
{
    public static class Values
    {
        public static double a = 1f;
        public static double b = 2f;
        public static double c = -3f;
        public static double d = 2f;

        public static int alpha = 1;
        public static int beta = 0;
        public static int gamma = 0;
        public static int eta = 1;

        public static double l = Math.PI;

        public static Func<double, double, double> AnaliticalU = (x, t) => Math.Exp(-t - x) * Math.Cos(x) * Math.Cos(2 * t);

        public static Func<double, double, double> fX = (x, t) => 0;

        public static Func<double, double> phi0 = t => -Math.Exp(-t) * Math.Cos(2 * t);
        public static Func<double, double> phil = t => -Math.Exp(-t - Math.PI) * Math.Cos(2 * t);

        public static Func<double, double> psi1 = x => Math.Exp(-x) * Math.Cos(x);
        public static Func<double, double> dpsi1 = x => -Math.Exp(-x) * (Math.Sin(x) + Math.Cos(x));
        public static Func<double, double> ddpsi1 = x => 2 * Math.Exp(-x) * Math.Sin(x);
        public static Func<double, double> psi2 = x => -Math.Exp(-x) * Math.Cos(x);
    }
}
