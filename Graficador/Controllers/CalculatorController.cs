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
                var result = _service.ExecuteMethod(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
