namespace Graficador.Models.Regression
{
    public class RegressionRequest
    {
        // Lista de arrays donde [0] es X y [1] es Y
        public List<double[]> PuntosCargados { get; set; } = new();
        public double Tolerancia { get; set; } // ej: 0.8 para el 80%
        public string Method { get; set; } // "RegresionLineal"
    }
}
