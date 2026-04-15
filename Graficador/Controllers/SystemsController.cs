using Microsoft.AspNetCore.Mvc;

namespace Graficador.Controllers
{
    public class SystemsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
