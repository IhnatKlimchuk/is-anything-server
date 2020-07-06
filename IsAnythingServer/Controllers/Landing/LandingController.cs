using Microsoft.AspNetCore.Mvc;

namespace IsAnythingServer.Controllers.Landing
{
    [Route("/")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LandingController : ControllerBase
    {
        [HttpGet]
        public ContentResult Index()
        {
            return base.Content(
                content:
                "<!DOCTYPE html>" +
                "<html lang = \"en\">" +
                "<head>" +
                "<meta charset = \"utf-8\"/>" +
                "<title>Index</title>" +
                "</head>" +
                "<body>Index</body>" +
                "</html>",
                contentType: "text/html");
        }
    }
}