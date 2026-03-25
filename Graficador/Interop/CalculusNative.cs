using System.Runtime.InteropServices;

namespace Graficador.Interop
{
    public static class CalculusNative
    {
        // Ejemplo hipotético de cómo llamar a la función de la DLL
        [DllImport("Calculus-C.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern double EvaluateFunction(string expression, double x);

        // Aquí mapeas las demás funciones de la librería: Derivar, Integrar, etc.
    }
}
