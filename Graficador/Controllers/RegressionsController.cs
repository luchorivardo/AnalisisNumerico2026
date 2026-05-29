using Graficador.Models.Regression;
using Graficador.Service;
using Microsoft.AspNetCore.Mvc;

namespace Graficador.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegressionsController : Controller
    {
        private readonly IRegressionService _regressionService;

        // Inyección de dependencias
        public RegressionsController(IRegressionService regressionService)
        {
            _regressionService = regressionService;
        }

        [HttpPost("calculate")]
        public IActionResult Calculate([FromBody] RegressionRequest request)
        {
            try
            {
                if (request.PuntosCargados == null || request.PuntosCargados.Count < 2)
                    return BadRequest("Se requieren al menos 2 puntos para realizar una regresión.");

                if (request.Tolerancia <= 0 || request.Tolerancia > 1)
                    return BadRequest("La tolerancia debe estar entre 0 y 1.");

                RegressionResponse result;

                // Bifurcación basada en el Switch del Frontend
                if (request.Method.ToLower() == "polynomial")
                {
                    if (!request.Degree.HasValue || request.Degree.Value < 2)
                        return BadRequest("Para regresión polinomial debe especificar un grado válido (>= 2).");

                    result = _regressionService.ExecutePolynomialRegression(request);
                }
                else
                {
                    result = _regressionService.ExecuteLinearRegression(request);
                }

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