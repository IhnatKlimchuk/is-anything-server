using Microsoft.AspNetCore.Mvc;

namespace IsAnythingServer.Controllers.Landing
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LandingController : Controller
    {
        public IActionResult Read()
        {
            return View();
        }

        public IActionResult Write([FromQuery] string subject, [FromQuery] string predicate)
        {
            ViewData["subject"] = subject;
            ViewData["predicate"] = predicate;
            return View();
        }

        public IActionResult ClientExample()
        {
            return View();
        }
    }
}