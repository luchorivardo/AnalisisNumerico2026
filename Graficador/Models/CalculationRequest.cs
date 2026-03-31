namespace Graficador.Models
{
    public class CalculationRequest
    {
        public string Function { get; set; } // ej: "x^2 - 4"
        public string Method { get; set; }   // "Bisection", "Newton", etc.
        public double? XStart { get; set; }   // 'a' o 'x0'
        public double? XEnd { get; set; }     // 'b' (para métodos cerrados)
        public double Tolerance { get; set; }
        public int MaxIterations { get; set; }
    }
}
