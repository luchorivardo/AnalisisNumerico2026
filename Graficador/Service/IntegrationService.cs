using Calculus;
using Graficador.Models.Integration;
using System.Globalization;
// Asegúrate de usar el namespace donde esté tu clase Calculo

namespace Graficador.Service
{
    public class IntegrationService : IIntegrationService
    {
        // Instancia de tu evaluador de funciones existente
        private readonly Calculo _analizador = new Calculo();

        public IntegrationResponse ExecuteIntegration(IntegrationRequest req)
        {
            if (!_analizador.Sintaxis(req.Function, 'x'))
                throw new Exception("Función mal ingresada.");

            double resultado = req.Method.ToLower() switch
            {
                "trapecios_simple" => CalcularTrapeciosSimple(req.Function, req.Xi, req.Xd),
                "trapecios_multiple" => CalcularTrapeciosMultiple(req.Function, req.Xi, req.Xd, req.N),
                "simpson_13_simple" => CalcularSimpson13Simple(req.Function, req.Xi, req.Xd),
                "simpson_13_multiple" => CalcularSimpson13Multiple(req.Function, req.Xi, req.Xd, req.N),
                "simpson_38" => CalcularSimpson38(req.Function, req.Xi, req.Xd),
                "simpson_combinado" => CalcularSimpsonCombinado(req.Function, req.Xi, req.Xd, req.N),
                _ => throw new Exception("Método de integración no válido.")
            };

            return new IntegrationResponse
            {
                Area = resultado,
                AreaFormateada = Math.Round(resultado, 6).ToString(CultureInfo.InvariantCulture)
            };
        }

        // ==========================================
        // MÉTODOS DEL PDF
        // ==========================================
        private double CalcularTrapeciosSimple(string funcion, double xi, double xd)
        {
            double fXi = _analizador.EvaluaFx(xi);
            double fXd = _analizador.EvaluaFx(xd);
            return ((fXi + fXd) * (xd - xi)) / 2.0;
        }

        private double CalcularTrapeciosMultiple(string funcion, double xi, double xd, int n)
        {
            if (n <= 0) throw new Exception("La cantidad de subintervalos (n) debe ser mayor a 0.");

            double h = (xd - xi) / n;
            double sum = 0;

            for (int i = 1; i < n; i++)
            {
                sum += _analizador.EvaluaFx(xi + h * i);
            }

            return (h / 2.0) * (_analizador.EvaluaFx(xi) + 2 * sum + _analizador.EvaluaFx(xd));
        }

        private double CalcularSimpson13Simple(string funcion, double xi, double xd)
        {
            double h = (xd - xi) / 2.0;
            return (h / 3.0) * (_analizador.EvaluaFx(xi) + 4 * _analizador.EvaluaFx(xi + h) + _analizador.EvaluaFx(xd));
        }

        private double CalcularSimpson13Multiple(string funcion, double xi, double xd, int n)
        {
            if (n <= 0 || n % 2 != 0) throw new Exception("Para Simpson 1/3 Múltiple puro, 'n' debe ser par.");

            double h = (xd - xi) / n;
            double sumPares = 0, sumImpares = 0;

            for (int i = 1; i < n; i++)
            {
                if (i % 2 == 0) sumPares += _analizador.EvaluaFx(xi + h * i);
                else sumImpares += _analizador.EvaluaFx(xi + h * i);
            }

            return (h / 3.0) * (_analizador.EvaluaFx(xi) + 4 * sumImpares + 2 * sumPares + _analizador.EvaluaFx(xd));
        }

        private double CalcularSimpson38(string funcion, double xi, double xd)
        {
            double h = (xd - xi) / 3.0;
            return (3 * h / 8.0) * (_analizador.EvaluaFx(xi) + 3 * _analizador.EvaluaFx(xi + h) + 3 * _analizador.EvaluaFx(xi + 2 * h) + _analizador.EvaluaFx(xd));
        }

        // Esta es la lógica final de la página 3 de tu PDF
        private double CalcularSimpsonCombinado(string funcion, double xi, double xd, int n)
        {
            if (n < 3) throw new Exception("Se requieren al menos 3 subintervalos para usar este método.");

            double h = (xd - xi) / n;
            double resultado = 0;

            double workingXi = xi;
            double workingXd = xd;
            int workingN = n;

            // Si es impar, usamos Simpson 3/8 para los últimos 3 intervalos
            if (n % 2 != 0)
            {
                double nuevoXi = workingXi + h * (workingN - 3);
                resultado += CalcularSimpson38(funcion, nuevoXi, workingXd);

                workingN = workingN - 3;
                workingXd = nuevoXi; // Ajustamos el límite superior para lo que resta calcular
            }

            // Si quedan intervalos pares (o si ya era par desde el principio)
            if (workingN > 0)
            {
                resultado += CalcularSimpson13Multiple(funcion, workingXi, workingXd, workingN);
            }

            return resultado;
        }
    }
}