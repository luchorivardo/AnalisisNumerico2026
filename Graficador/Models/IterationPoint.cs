namespace Graficador.Models
{
    public class IterationPoint
    {
        public int Iteration { get; set; }
        public List<double> Values { get; set; } = new();
        public double X { get; set; }
        public double Y { get; set; }
        public double Error { get; set; }
    }
}
