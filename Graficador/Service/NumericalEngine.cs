using Calculus;
using Graficador.Interop;
using Graficador.Models;
using Calculus;

namespace Graficador.Service
{
    public class NumericalEngine
    {

        private readonly Calculo _analizador = new Calculo();

        public CalculationResponse ExecuteMethod(CalculationRequest req)
        {
            // 1. Validar sintaxis antes de empezar
            if (!_analizador.Sintaxis(req.Function, 'x'))
                throw new Exception("Error de sintaxis en la función.");

            return req.Method.ToLower() switch
            {
                "biseccion" => Bisection(req.Function, req.XStart, req.XEnd, req.Tolerance),
                "reglafalsa" => ReglaFalsa(req.Function, req.XStart, req.XEnd, req.Tolerance),
                "newton" => NewtonRaphson(req.Function, req.XStart, req.Tolerance),
                "secante" => Secante(req.Function, req.XStart, req.XEnd, req.Tolerance),
                _ => throw new Exception("Método no implementado")
            };
        }

        // --- METODOS CERRADOS ---

        private CalculationResponse Bisection(string fx, double a, double b, double tol)
        {
            var res = new CalculationResponse();
            double xr = a, xrOld, error = 100;

            while (error > tol)
            {
                xrOld = xr;
                xr = (a + b) / 2;
                double fA = _analizador.EvaluaFx(a); 
                double fRr = _analizador.EvaluaFx(xr);

                error = Math.Abs((xr - xrOld) / xr);
                res.Iterations.Add(new IterationPoint { X = xr, Y = fRr, Error = error });

                if (fA * fRr < 0) b = xr; else a = xr;
            }
            return res;
        }

        private CalculationResponse ReglaFalsa(string fx, double a, double b, double tol)
        {
            var res = new CalculationResponse();
            double xr = a, xrOld, error = 100;

            while (error > tol)
            {
                xrOld = xr;
                double fA = _analizador.EvaluaFx(a);
                double fB = _analizador.EvaluaFx(b);

                // FOrmula: xr = b - (f(b)*(a-b))/(f(a)-f(b))
                xr = b - (fB * (a - b)) / (fA - fB);
                double fRr = _analizador.EvaluaFx(xr);

                error = Math.Abs((xr - xrOld) / xr);
                res.Iterations.Add(new IterationPoint { X = xr, Y = fRr, Error = error });

                if (fA * fRr < 0) b = xr; else a = xr;
            }
            return res;
        }

        // --- METODOS ABIERTOS ---

        private CalculationResponse NewtonRaphson(string fx, double x0, double tol)
        {
            var res = new CalculationResponse();
            double xn = x0, error = 100;

            for (int i = 0; i < 50 && error > tol; i++)
            {
                double fxn = _analizador.EvaluaFx(xn);
                double dfxn = _analizador.Dx(xn);

                double xNext = xn - (fxn / dfxn);
                error = Math.Abs((xNext - xn) / xNext);

                xn = xNext;
                res.Iterations.Add(new IterationPoint { Iteration = i, X = xn, Y = fxn, Error = error });
            }
            return res;
        }

        private CalculationResponse Secante(string fx, double x0, double x1, double tol)
        {
            var res = new CalculationResponse();
            double error = 100;

            while (error > tol)
            {
                double f0 = _analizador.EvaluaFx(x0);
                double f1 = _analizador.EvaluaFx(x1);

                double xNext = x1 - (f1 * (x1 - x0)) / (f1 - f0);
                error = Math.Abs((xNext - x1) / xNext);

                x0 = x1;
                x1 = xNext;
                res.Iterations.Add(new IterationPoint { X = x1, Y = f1, Error = error });
            }
            return res;
        }
    }
}
