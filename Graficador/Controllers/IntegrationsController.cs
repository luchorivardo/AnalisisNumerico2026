using Graficador.Models.Integration;
using Graficador.Service;
using Microsoft.AspNetCore.Mvc;

namespace Graficador.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IntegrationsController : Controller
    {
        private readonly IIntegrationService _integrationService;

        public IntegrationsController(IIntegrationService integrationService)
        {
            _integrationService = integrationService;
        }

        [HttpPost("calculate")]
        public IActionResult Calculate([FromBody] IntegrationRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Function))
                    return BadRequest("Debe ingresar una función válida.");

                if (request.Xi >= request.Xd)
                    return BadRequest("El límite inferior (Xi) debe ser menor al límite superior (Xd).");

                var result = _integrationService.ExecuteIntegration(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}