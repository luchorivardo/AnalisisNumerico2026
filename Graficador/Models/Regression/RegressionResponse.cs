namespace Graficador.Models.Regression
{
    public class RegressionResponse
    {
        public string Funcion { get; set; } // "y = a1 x + a0"
        public string EfectividadPorcentaje { get; set; } // Coeficiente "r"
        public string EfectividadMensaje { get; set; } // "Aceptable" o "No aceptable"
    }
}
