using Microsoft.AspNetCore.Mvc;

namespace VCAuthn.Controllers
{
    [Route("/")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class IndexController : Controller
    {
        public IndexController() : base() { }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}