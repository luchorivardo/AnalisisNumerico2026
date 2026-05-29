namespace Graficador.Models.Integration
{
    public class IntegrationRequest
    {
        public string Function { get; set; } = string.Empty;
        public double Xi { get; set; }
        public double Xd { get; set; }
        public int N { get; set; } // Cantidad de subintervalos
        public string Method { get; set; } = string.Empty;
    }
}