namespace Lab_7.DefaultValues
{
    public static class Values
    {
        public static double bX = 0f;
        public static double bY = 0f;
        public static double c = 0f;

        public static Func<double, double, double> F = (x, y) => 0f;

        public static double lX = 1;
        public static double lY = 1;

        public static int alpha1 = 1;
        public static int beta1 = 0;
        public static Func<double, double> phi1 = y => 0;

        public static int alpha2 = 0;
        public static int beta2 = 1;
        public static Func<double, double> phi2 = y => 1 - Math.Pow(y, 2);

        public static int alpha3 = 1;
        public static int beta3 = 0;
        public static Func<double, double> phi3 = x => 0;

        public static int alpha4 = 0;
        public static int beta4 = 1;
        public static Func<double, double> phi4 = x => Math.Pow(x, 2) - 1;

        public static Func<double, double, double> AnaliticalU = (x, y) => Math.Pow(x, 2) - Math.Pow(y, 2);
    }
    /* public static class Values
     {
         public static double bX = 0f;
         public static double bY = 0f;
         public static double c = 1f;

         public static Func<double, double, double> F = (x, y) => -3 * Math.Sin(x) * Math.Sin(y);

         public static double lX = Math.PI / 2f;
         public static double lY = Math.PI / 2f;

         public static int alpha1 = 0;
         public static int beta1 = 1;
         public static Func<double, double> phi1 = y => 0;

         public static int alpha2 = 1;
         public static int beta2 = 0;
         public static Func<double, double> phi2 = y => 0;

         public static int alpha3 = 0;
         public static int beta3 = 1;
         public static Func<double, double> phi3 = x => Math.Sin(x);

         public static int alpha4 = 1;
         public static int beta4 = 0;
         public static Func<double, double> phi4 = x => -Math.Sin(x);

         public static Func<double, double, double> AnaliticalU = (x, y) => Math.Sin(x) * Math.Sin(y);
     }*/
}
