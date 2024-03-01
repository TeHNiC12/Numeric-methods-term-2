namespace Course_project
{
    public class Execute
    {
        public void Solve ()
        {
            try
            {
                result = solver.Solve(A, b, null, epsilon);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public void DisplayResults ()
        {
            Console.WriteLine($"Iterations = {result.Item2}");

            for (int i = 0; i < result.Item1.GetLength(0); i++)
            {
                Console.WriteLine($"X{i + 1} = {result.Item1[i, 0]}");
            }
        }

        private Tuple<double[,], int> result;

        private double[,] A = new double[10, 10]
            {
                { 0.681, 0.0, 0.0, 0.511, 0.0, 0.0, 0.055, 0.09, 0.0, 0.594 },
                { 0.0, 0.0, 0.0, 0.801, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 },
                { 0.0, 0.0, 0.0, 0.0, 0.175, 0.384, 0.405, 0.179, 0.0, 0.0 },
                { 0.511, 0.801, 0.0, 0.438, 0.0, 0.188, 0.0, 0.0, 0.645, 0.167 },
                { 0.0, 0.0, 0.175, 0.0, 0.688, 0.0, 0.0, 0.0, 0.0, 0.541 },
                { 0.0, 0.0, 0.384, 0.188, 0.0, 0.0, 0.349, 0.221, 0.0, 0.0 },
                { 0.055, 0.0, 0.405, 0.0, 0.0, 0.349, 0.0, 0.714, 0.813, 0.916 },
                { 0.09, 0.0, 0.179, 0.0, 0.0, 0.221, 0.714, 0.136, 0.0, 0.0 },
                { 0.0, 0.0, 0.0, 0.645, 0.0, 0.0, 0.813, 0.0, 0.0, 0.944 },
                { 0.594, 0.0, 0.0, 0.167, 0.541, 0.0, 0.916, 0.0, 0.944, 0.0 }
            };
        private double[,] b = new double[10, 1]
            {
                { 11 },
                { 48 },
                { 6 },
                { 39 },
                { 16 },
                { 42 },
                { 28 },
                { 41 },
                { 40 },
                { 24 }
            };
        private double epsilon = 0.0000001;

        public string filePath = @"C:\Users\ivanr\Documents\MAI 4th year\Численные методы\Numeric_methods_2\Course_project\Data\Data.txt";

        private ConjugateGradient solver = new();
    }
}
