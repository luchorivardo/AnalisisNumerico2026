using Graficador.Models.Integration;

namespace Graficador.Service
{
    public interface IIntegrationService
    {
        IntegrationResponse ExecuteIntegration(IntegrationRequest request);
    }
}