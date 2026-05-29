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
                // Validaciones básicas de entrada antes de procesar
                if (request.PuntosCargados == null || request.PuntosCargados.Count < 2)
                {
                    return BadRequest("Se requieren al menos 2 puntos para realizar una regresión.");
                }

                if (request.Tolerancia <= 0 || request.Tolerancia > 1)
                {
                    return BadRequest("La tolerancia debe estar expresada en formato decimal entre 0 y 1 (ej: 0.8 para 80%).");
                }

                var result = _regressionService.ExecuteLinearRegression(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Registra el error si manejas logs y devuelve el mensaje
                return BadRequest(ex.Message);
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}