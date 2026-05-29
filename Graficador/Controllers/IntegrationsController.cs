using Microsoft.AspNetCore.Mvc;

namespace Graficador.Controllers
{
    public class IntegrationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
