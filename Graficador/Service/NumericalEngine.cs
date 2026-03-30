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
            if (!_analizador.Sintaxis(req.Function, 'x'))
                throw new Exception("Error de sintaxis en la función.");

            return req.Method.ToLower() switch
            {
                "bisection" => Bisection(req.Function, req.XStart, req.XEnd, req.Tolerance, req.MaxIterations),
                "reglafalsa" => ReglaFalsa(req.Function, req.XStart, req.XEnd, req.Tolerance, req.MaxIterations),
                "newton" => NewtonRaphson(req.Function, req.XStart, req.Tolerance, req.MaxIterations),
                "secante" => Secante(req.Function, req.XStart, req.XEnd, req.Tolerance, req.MaxIterations),
                _ => throw new Exception("Método no implementado")
            };
        }

        // =========================
        // MÉTODOS CERRADOS
        // =========================

        public CalculationResponse Bisection(string fx, double xi, double xd, double tol, int maxIter)
        {
            var res = new CalculationResponse();

            if (!_analizador.Sintaxis(fx, 'x'))
                throw new Exception("Error de sintaxis en la función.");

            double fXi = _analizador.EvaluaFx(xi);
            double fXd = _analizador.EvaluaFx(xd);

            if (fXi * fXd > 0)
                throw new Exception("El intervalo no contiene raíz.");

            string ggb = "";
            ggb += $"f(x)={fx};";


            double xr = 0;
            double xrAnterior = 0;
            double error = double.MaxValue;

            for (int i = 1; i <= maxIter; i++)
            {
                xr = (xi + xd) / 2;

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
                    xi = xr;
                    fXi = fXr;
                }
                else
                {
                    xd = xr;
                    fXd = fXr;
                }

                xrAnterior = xr;
            }

            // -----------------------
            // GEO GEBRA (solo última iteración)
            // -----------------------

            string xiStr = xi.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string xdStr = xd.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string xrStr = xr.ToString(System.Globalization.CultureInfo.InvariantCulture);

            // puntos
            ggb += $"PXi=({xiStr},f({xiStr}));";
            ggb += $"PXr=({xrStr},f({xrStr}));";
            ggb += $"PXd=({xdStr},f({xdStr}));";

            // líneas verticales
            //ggb += $"Xi: x={xiStr};";
            ggb += $"Xr: x={xrStr};";
            //ggb += $"Xd: x={xdStr};";

        

            res.Root = xr.ToString(System.Globalization.CultureInfo.InvariantCulture);
            res.GgbCommand = ggb;

            return res;
        }
        public CalculationResponse ReglaFalsa(string fx, double a, double b, double tol, int maxIter)
        {
            var res = new CalculationResponse();

            double xr = a, xrOld = a, error = double.MaxValue;
            int iter = 0;

            while (error > tol && iter < maxIter)
            {
                xrOld = xr;

                double fA = _analizador.EvaluaFx(a);
                double fB = _analizador.EvaluaFx(b);

                xr = b - (fB * (a - b)) / (fA - fB);
                double fXr = _analizador.EvaluaFx(xr);

                if (xr != 0)
                    error = Math.Abs((xr - xrOld) / xr);

                res.Iterations.Add(new IterationPoint
                {
                    Iteration = iter,
                    X = xr,
                    Y = fXr,
                    Error = error
                });

                if (fA * fXr < 0)
                    b = xr;
                else
                    a = xr;

                iter++;
            }

            res.Root = xr.ToString();
            return res;
        }

        // =========================
        // MÉTODOS ABIERTOS
        // =========================

        public CalculationResponse NewtonRaphson(string fx, double x0, double tol, int maxIter)
        {
            var res = new CalculationResponse();

            double xn = x0, error = double.MaxValue;
            int iter = 0;

            while (error > tol && iter < maxIter)
            {
                double fxn = _analizador.EvaluaFx(xn);
                double dfxn = _analizador.Dx(xn);

                if (dfxn == 0)
                    throw new Exception("Derivada cero, no se puede continuar.");

                double xNext = xn - (fxn / dfxn);

                if (xNext != 0)
                    error = Math.Abs((xNext - xn) / xNext);

                xn = xNext;

                res.Iterations.Add(new IterationPoint
                {
                    Iteration = iter,
                    X = xn,
                    Y = fxn,
                    Error = error
                });

                iter++;
            }

            res.Root = xn.ToString();
            return res;
        }

        public CalculationResponse Secante(string fx, double x0, double x1, double tol, int maxIter)
        {
            var res = new CalculationResponse();

            double error = double.MaxValue;
            int iter = 0;

            while (error > tol && iter < maxIter)
            {
                double f0 = _analizador.EvaluaFx(x0);
                double f1 = _analizador.EvaluaFx(x1);

                if (f1 - f0 == 0)
                    throw new Exception("División por cero en secante.");

                double xNext = x1 - (f1 * (x1 - x0)) / (f1 - f0);

                if (xNext != 0)
                    error = Math.Abs((xNext - x1) / xNext);

                x0 = x1;
                x1 = xNext;

                res.Iterations.Add(new IterationPoint
                {
                    Iteration = iter,
                    X = x1,
                    Y = f1,
                    Error = error
                });

                iter++;
            }

            res.Root = x1.ToString();
            return res;
        }
    }
}