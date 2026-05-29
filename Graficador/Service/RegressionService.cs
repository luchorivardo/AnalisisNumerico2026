using Graficador.Models.Regression;
using System.Globalization; // <-- Necesitas agregar esto

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

            // ==========================================
            // FORMATEO CORRECTO PARA GEOGEBRA (SIN COMAS)
            // ==========================================
            string a1Str = Math.Round(a1, 4).ToString(CultureInfo.InvariantCulture);
            string a0Str = Math.Abs(Math.Round(a0, 4)).ToString(CultureInfo.InvariantCulture);

            // Evaluamos si a0 es negativo para poner el signo correcto y evitar el "+ -"
            string signoA0 = a0 >= 0 ? "+" : "-";

            // Paso 10: Retorno
            return new RegressionResponse
            {
                Funcion = $"y = {a1Str}x {signoA0} {a0Str}",
                EfectividadPorcentaje = $"{Math.Round(r, 2)}%",
                EfectividadMensaje = r >= (req.Tolerancia * 100) ? "Ajuste aceptable" : "Ajuste no aceptable"
            };
        }
        public RegressionResponse ExecutePolynomialRegression(RegressionRequest req)
        {
            int n = req.PuntosCargados.Count;
            int grado = req.Degree!.Value;

            // Se requieren mínimo grado+1 puntos para un sistema con solución única
            if (n < grado + 1)
                throw new Exception($"Para un polinomio de grado {grado}, se requieren al menos {grado + 1} puntos.");

            // 1. Generar la matriz polinomial
            double[,] matriz = GenerarMatrizPolinomial(grado, req.PuntosCargados);

            // 2. Resolver el sistema con Gauss-Jordan para obtener los coeficientes (a0, a1, a2...)
            double[] vectorResultado = ResolverGaussJordan(matriz);

            // 3. Calcular el Coeficiente de Correlación (r)
            double sumY = 0;
            foreach (var p in req.PuntosCargados) sumY += p[1];
            double mediaY = sumY / n;

            double st = 0, sr = 0;
            foreach (var punto in req.PuntosCargados)
            {
                double x = punto[0];
                double y = punto[1];
                double suma = 0;

                // Equivalente a: a0 + a1*x + a2*x^2 ...
                for (int i = 0; i < vectorResultado.Length; i++)
                {
                    suma += vectorResultado[i] * Math.Pow(x, i);
                }

                sr += Math.Pow(suma - y, 2);
                st += Math.Pow(mediaY - y, 2);
            }

            double r = Math.Sqrt((st - sr) / st) * 100;

            // 4. Armar el string de la función optimizado para GeoGebra
            // Lo armamos al revés (de grado mayor a a0) para que se lea natural: ax^2 + bx + c
            List<string> terminos = new List<string>();

            for (int i = vectorResultado.Length - 1; i >= 0; i--)
            {
                double coef = Math.Round(vectorResultado[i], 4);
                if (coef == 0 && i != 0) continue; // Omitimos términos en cero

                string coefStr = Math.Abs(coef).ToString(CultureInfo.InvariantCulture);
                string termino = "";

                if (i == 0) termino = coefStr;
                else if (i == 1) termino = $"{coefStr}x";
                else termino = $"{coefStr}x^{i}";

                if (terminos.Count == 0)
                {
                    // Es el primer término que agregamos (el de mayor grado)
                    if (coef < 0) termino = "-" + termino;
                    terminos.Add(termino);
                }
                else
                {
                    // Términos subsecuentes (les ponemos el signo separado para que quede bonito)
                    string signo = coef >= 0 ? "+" : "-";
                    terminos.Add($"{signo} {termino}");
                }
            }

            string funcionFinal = "y = " + string.Join(" ", terminos);

            // 5. Retornar los datos
            return new RegressionResponse
            {
                Funcion = funcionFinal,
                EfectividadPorcentaje = $"{Math.Round(r, 4)}%", // A 4 decimales como muestra tu ejemplo
                EfectividadMensaje = r >= (req.Tolerancia * 100) ? "Ajuste aceptable" : "Ajuste no aceptable"
            };
        }

        // ==========================================
        // MÉTODOS PRIVADOS DE AYUDA (HELPERS)
        // ==========================================
        private double[,] GenerarMatrizPolinomial(int grado, List<double[]> puntosCargados)
        {
            int dimension = grado + 1;
            double[,] matriz = new double[dimension, dimension + 1];

            foreach (var punto in puntosCargados)
            {
                double x = punto[0];
                double y = punto[1];

                for (int fila = 0; fila < dimension; fila++)
                {
                    for (int col = 0; col < dimension; col++)
                    {
                        matriz[fila, col] += Math.Pow(x, fila + col);
                    }
                    matriz[fila, dimension] += y * Math.Pow(x, fila);
                }
            }
            return matriz;
        }

        private double[] ResolverGaussJordan(double[,] matriz)
        {
            int filas = matriz.GetLength(0);
            int cols = matriz.GetLength(1);

            for (int i = 0; i < filas; i++)
            {
                // Pivoteo parcial para evitar división por cero o inestabilidad
                int pivot = i;
                for (int j = i + 1; j < filas; j++)
                {
                    if (Math.Abs(matriz[j, i]) > Math.Abs(matriz[pivot, i])) pivot = j;
                }

                if (pivot != i)
                {
                    for (int k = 0; k < cols; k++)
                    {
                        double temp = matriz[i, k];
                        matriz[i, k] = matriz[pivot, k];
                        matriz[pivot, k] = temp;
                    }
                }

                double pivotVal = matriz[i, i];
                if (Math.Abs(pivotVal) < 1e-12)
                    throw new Exception("El sistema no tiene solución única (matriz singular). Los puntos podrían estar alineados de forma redundante.");

                for (int j = 0; j < cols; j++) matriz[i, j] /= pivotVal;

                for (int j = 0; j < filas; j++)
                {
                    if (i != j)
                    {
                        double factor = matriz[j, i];
                        for (int k = 0; k < cols; k++)
                        {
                            matriz[j, k] -= factor * matriz[i, k];
                        }
                    }
                }
            }

            // Extraer el vector resultado
            double[] resultado = new double[filas];
            for (int i = 0; i < filas; i++)
            {
                resultado[i] = matriz[i, cols - 1];
            }
            return resultado;
        }
    }
}