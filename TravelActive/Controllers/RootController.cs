using Microsoft.AspNetCore.Mvc;

namespace TravelActive.Controllers
{
    [Route("/")]
    public class RootController : Controller
    {
        [HttpGet]
        public IActionResult GetRoot()
        {
            return Ok();
        }
    }
}