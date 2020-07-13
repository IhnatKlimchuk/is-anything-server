using Microsoft.AspNetCore.Mvc;

namespace IsAnythingServer.Controllers.Landing
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LandingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Write([FromQuery] string subject, [FromQuery] string predicate)
        {
            ViewData["subject"] = subject;
            ViewData["predicate"] = predicate;
            return View();
        }
    }
}