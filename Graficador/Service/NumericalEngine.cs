using Calculus;
using Graficador.Models;
using System.Globalization;

namespace Graficador.Service
{
    public class NumericalEngine
    {
        private readonly Calculo _analizador = new Calculo();


        public CalculationResponse ExecuteMethod(CalculationRequest req)
        {
            string methodLower = req.Method.ToLower();

            if (methodLower != "gaussjordan" && methodLower != "gaussseidel")
            {
                if (string.IsNullOrEmpty(req.Function) || !_analizador.Sintaxis(req.Function, 'x'))
                    throw new Exception("Error de sintaxis en la función.");
                if (req.Tolerance <= 0) throw new Exception("La tolerancia debe ser positiva.");
                if (req.MaxIterations <= 0) throw new Exception("Las iteraciones deben ser mayores que cero.");
            }

            return methodLower switch
            {
                "bisection" or "falserule" => ClosedMethod(methodLower, req.Function!, req.XStart, req.XEnd, req.Tolerance, req.MaxIterations),
                "newton" or "secant" => OpenMethod(methodLower, req.Function!, req.XStart, req.XEnd, req.Tolerance, req.MaxIterations),
                "gaussjordan" => SolveGaussJordan(req.Matrix),
                "gaussseidel" => SolveGaussSeidel(req.Matrix, req.Tolerance, req.MaxIterations),
                _ => throw new Exception("Método no implementado")
            };
        }

        // =========================
        // MÉTODO GAUSS-JORDAN
        // =========================
        public CalculationResponse SolveGaussJordan(double[][]? matrix)
        {
            if (matrix == null || matrix.Length == 0) throw new Exception("Matriz vacía.");
            int n = matrix.Length;
            int m = matrix[0].Length;

            for (int rowDiag = 0; rowDiag < n; rowDiag++)
            {
                double coefDiag = matrix[rowDiag][rowDiag];
                if (Math.Abs(coefDiag) < 1e-12) throw new Exception("Diagonal nula. Requiere pivoteo.");

                for (int col = 0; col < m; col++)
                {
                    matrix[rowDiag][col] /= coefDiag;
                }

                for (int row = 0; row < n; row++)
                {
                    if (row != rowDiag)
                    {
                           
                        double coefCero = matrix[row][rowDiag];
                        for (int col = 0; col < m; col++)
                        {
                            matrix[row][col] -= (coefCero * matrix[rowDiag][col]);
                        }
                    }
                }
            }

            return FormatSystemResponse(matrix, n);
        }

        // =========================
        // MÉTODO GAUSS-SEIDEL
        // =========================
        public CalculationResponse SolveGaussSeidel(double[][]? matrix, double tolerance, int maxIter)
        {
            if (matrix == null || matrix.Length == 0) throw new Exception("Matriz vacía.");
            int n = matrix.Length;
            double[] x = new double[n];
            double[] xOld = new double[n];
            int iter = 0;
            bool converge = false;
            var res = new CalculationResponse();

            while (iter < maxIter && !converge)
            {
                iter++;
                Array.Copy(x, xOld, n);

                for (int i = 0; i < n; i++)
                {
                    double suma = matrix[i][n];
                    for (int j = 0; j < n; j++)
                    {
                        if (i != j) suma -= matrix[i][j] * x[j];
                    }
                    x[i] = suma / matrix[i][i];
                }

                int hits = 0;
                double maxError = 0;

                for (int i = 0; i < n; i++)
                {
                    double error = x[i] == 0 ? Math.Abs(x[i] - xOld[i]) : Math.Abs((x[i] - xOld[i]) / x[i]);
                    if (error < tolerance) hits++;
                    if (error > maxError) maxError = error;
                }
                converge = (hits == n);

                res.Iterations.Add(new IterationPoint
                {
                    Iteration = iter,
                    Values = x.ToList(),
                    Error = maxError
                });
            }

            if (!converge) throw new Exception("El método no convergió (posible matriz no dominante).");

            
            res.Root = string.Join(" | ", x.Select((val, i) => $"x{i + 1} = {Math.Round(val, 6)}"));
            return res;
        }

        private CalculationResponse FormatSystemResponse(double[][] matrix, int n)
        {
            var res = new CalculationResponse();
            List<string> sols = new List<string>();
            for (int i = 0; i < n; i++)
                sols.Add($"x{i + 1} = {Math.Round(matrix[i][n], 6).ToString(CultureInfo.InvariantCulture)}");

            res.Root = string.Join(" | ", sols);
            return res;
        }

        // =========================
        // MÉTODOS CERRADOS
        // =========================
        private CalculationResponse ClosedMethod(string method, string fx, double? xi, double? xd, double tol, int maxIter)
        {
            if (xi == null || xd == null)
                throw new Exception("Los métodos cerrados requieren Xi y Xd.");

            double xLeft = xi.Value;
            double xRight = xd.Value;

            var res = new CalculationResponse();

            double fXi = _analizador.EvaluaFx(xLeft);
            double fXd = _analizador.EvaluaFx(xRight);

            if (fXi * fXd > 0)
                throw new Exception("El intervalo no contiene raíz.");

            string ggb = $"f(x)={fx};";

            double xr = 0;
            double xrAnterior = 0;
            double error = double.MaxValue;

            for (int i = 1; i <= maxIter; i++)
            {
                xr = method switch
                {
                    "bisection" => (xLeft + xRight) / 2,
                    "falserule" => (fXd * xLeft - fXi * xRight) / (fXd - fXi),
                    _ => throw new Exception("Método cerrado no válido")
                };

                double fXr = _analizador.EvaluaFx(xr);

                if (i > 1 && xr != 0)
                    error = Math.Abs((xr - xrAnterior) / xr);

                res.Iterations.Add(new IterationPoint
                {
                    Iteration = i,
                    X = xr,
                    Y = fXr,
                    Error = error
                });

                if (Math.Abs(fXr) < tol || error < tol)
                    break;

                if (fXi * fXr > 0)
                {
                    xLeft = xr;
                    fXi = fXr;
                }
                else
                {
                    xRight = xr;
                    fXd = fXr;
                }

                xrAnterior = xr;
            }

            string xrStr = xr.ToString(CultureInfo.InvariantCulture);

            ggb += $"PXr=({xrStr},f({xrStr}));";
            ggb += $"Xr: x={xrStr};";

            res.Root = xrStr;
            res.GgbCommand = ggb;

            return res;
        }

        // =========================
        // MÉTODOS ABIERTOS
        // =========================
        private CalculationResponse OpenMethod(string method, string fx, double? xi, double? xd, double tol, int maxIter)
        {
            if (xi == null)
                throw new Exception("Debe ingresar Xi.");

            if (method == "secant" && xd == null)
                throw new Exception("El método de la secante requiere Xi y Xd.");

            double xLeft = xi.Value;
            double xRight = xd ?? 0;

            var res = new CalculationResponse();

            string ggb = $"f(x)={fx};";

            double xr = 0;
            double xrAnterior = 0;
            double error = double.MaxValue;

            double fXi = _analizador.EvaluaFx(xLeft);

            if (Math.Abs(fXi) < tol)
            {
                res.Root = xLeft.ToString(CultureInfo.InvariantCulture);
                return res;
            }

            if (method == "secant")
            {
                double fXd = _analizador.EvaluaFx(xRight);

                if (Math.Abs(fXd) < tol)
                {
                    res.Root = xRight.ToString(CultureInfo.InvariantCulture);
                    return res;
                }
            }

            for (int i = 1; i <= maxIter; i++)
            {
                xr = method switch
                {
                    "newton" => CalcularNewton(xLeft, tol),
                    "secant" => CalcularSecante(xLeft, xRight),
                    _ => throw new Exception("Método abierto no válido")
                };

                if (double.IsNaN(xr) || double.IsInfinity(xr))
                    throw new Exception("El método diverge.");

                double fXr = _analizador.EvaluaFx(xr);

                if (i > 1 && xr != 0)
                    error = Math.Abs((xr - xrAnterior) / xr);

                res.Iterations.Add(new IterationPoint
                {
                    Iteration = i,
                    X = xr,
                    Y = fXr,
                    Error = error
                });

                if (Math.Abs(fXr) < tol || error < tol)
                    break;

                if (method == "newton")
                {
                    xLeft = xr;
                }
                else
                {
                    xLeft = xRight;
                    xRight = xr;
                }

                xrAnterior = xr;
            }

            string xrStr = xr.ToString(CultureInfo.InvariantCulture);

            ggb += $"PXr=({xrStr},f({xrStr}));";
            ggb += $"Xr: x={xrStr};";

            res.Root = xrStr;
            res.GgbCommand = ggb;

            return res;
        }

        // =========================
        // HELPERS
        // =========================
        private double CalcularNewton(double xi, double tol)
        {
            double fXi = _analizador.EvaluaFx(xi);
            double derivada = _analizador.Dx(xi);

            if (Math.Abs(derivada) < tol || double.IsNaN(derivada))
                return double.NaN;

            return xi - (fXi / derivada);
        }

        private double CalcularSecante(double xi, double xd)
        {
            double fXi = _analizador.EvaluaFx(xi);
            double fXd = _analizador.EvaluaFx(xd);

            if (fXd - fXi == 0)
                return double.NaN;

            return (fXd * xi - fXi * xd) / (fXd - fXi);
        }
    }
}