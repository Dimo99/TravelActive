using Api.ION;
using Microsoft.AspNetCore.Mvc;
using TravelActive.Common.Utilities;
using TravelActive.Models.ViewModels;

namespace TravelActive.Controllers
{
    [Route("/")]
    public class RootController : Controller
    {
        [HttpGet(Name = RouteNames.Root)]
        public IActionResult GetRoot()
        {
            var response = new RootResponse()
            {
                Self = LinkGenerator.To(RouteNames.Root),
                Directions = LinkGenerator.To(RouteNames.DirectionsRoot),
                Users = LinkGenerator.To(RouteNames.UsersRoot),
                Tokens = LinkGenerator.To(RouteNames.TokenRoot)
            };
            
            return Ok(response);
        }
    }
}