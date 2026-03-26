using Microsoft.AspNetCore.Mvc;

namespace Graficador.Controllers
{
    public class MethodsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Closed()
        {
            return PartialView("_ClosedMethods");
        }

        public IActionResult Open()
        {
            return PartialView("_OpenMethods");
        }

        public IActionResult Bisection()
        {
            return PartialView("_BisectionForm");
        }

        public IActionResult FalseRule()
        {
            return PartialView("_FalseRuleForm");
        }

        public IActionResult Newton()
        {
            return PartialView("_NewtonForm");
        }

        public IActionResult Secant()
        {
            return PartialView("_SecantForm");
        }
    }
}
