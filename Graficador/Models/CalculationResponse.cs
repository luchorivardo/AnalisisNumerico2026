namespace Graficador.Models
{
    public class CalculationResponse
    {
        public List<IterationPoint> Iterations { get; set; } = new();
        public string Root { get; set; }
        public string GgbCommand { get; set; } // Comando para GeoGebra
    }
}
