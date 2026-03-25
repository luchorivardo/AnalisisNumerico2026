using Graficador.Models;
using Graficador.Service;
using Microsoft.AspNetCore.Mvc;

namespace Graficador.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalculatorController : ControllerBase
    {
        private readonly NumericalEngine _service = new();

        [HttpPost("calculate")]
        public IActionResult Calculate([FromBody] CalculationRequest request)
        {
            try
            {
                CalculationResponse result;
                switch (request.Method.ToLower())
                {
                    case "bisection":
                        result = _service.Bisection(request.Function, request.XStart, request.XEnd, request.Tolerance);
                        break;
                    case "newton":
                        result = _service.NewtonRaphson(request.Function, request.XStart, request.Tolerance);
                        break;
                    default:
                        return BadRequest("Método no soportado.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
