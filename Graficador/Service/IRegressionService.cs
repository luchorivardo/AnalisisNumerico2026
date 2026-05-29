using Graficador.Models.Regression;

namespace Graficador.Service
{
    public interface IRegressionService
    {
        RegressionResponse ExecuteLinearRegression(RegressionRequest req);
    }
}
