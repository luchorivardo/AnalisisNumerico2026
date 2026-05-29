using Graficador.Models.Regression;

namespace Graficador.Service
{
    public class RegressionService : IRegressionService
    {
        public RegressionResponse ExecuteLinearRegression(RegressionRequest req)
        {
            int n = req.PuntosCargados.Count;
            if (n == 0) throw new Exception("No hay puntos cargados.");

            // Pasos 2 al 5 de tu algoritmo
            double sumX = 0, sumY = 0, sumXY = 0, sumX2 = 0;

            foreach (var punto in req.PuntosCargados)
            {
                double x = punto[0];
                double y = punto[1];

                sumX += x;
                sumY += y;
                sumXY += (x * y);
                sumX2 += (x * x);
            }

            // Paso 6 y 7
            double denominador = (n * sumX2) - Math.Pow(sumX, 2);
            if (denominador == 0) throw new Exception("No se puede calcular la pendiente (división por cero).");

            double a1 = ((n * sumXY) - (sumX * sumY)) / denominador;
            double a0 = (sumY / n) - a1 * (sumX / n);

            // Pasos 8 y 9: Cálculo de error y r
            double st = 0, sr = 0;
            double yMedia = sumY / n;

            foreach (var punto in req.PuntosCargados)
            {
                double x = punto[0];
                double y = punto[1];

                st += Math.Pow(y - yMedia, 2);
                sr += Math.Pow(y - a0 - a1 * x, 2);
            }

            double r = Math.Sqrt((st - sr) / st) * 100;

            // Paso 10: Retorno
            return new RegressionResponse
            {
                Funcion = $"y = {Math.Round(a1, 4)}x + {Math.Round(a0, 4)}",
                EfectividadPorcentaje = $"{Math.Round(r, 2)}%",
                EfectividadMensaje = r >= (req.Tolerancia * 100) ? "Ajuste aceptable" : "Ajuste no aceptable"
            };
        }
    }
}
