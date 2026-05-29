namespace Graficador.Models.Regression
{
    public class RegressionRequest
    {
        public List<double[]> PuntosCargados { get; set; } = new();
        public double Tolerancia { get; set; }
        public string Method { get; set; } = string.Empty;
        
        // Propiedad agregada para Regresión Polinomial
        public int? Degree { get; set; } 
    }
}