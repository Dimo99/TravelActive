using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TravelActive.Common.Extensions;
using TravelActive.Common.Utilities;
using TravelActive.Services;

namespace TravelActive.Controllers
{
    [Route("/[controller]")]
    public class RidesController : Controller
    {
        private readonly RidesService ridesService;
        public RidesController(RidesService ridesService)
        {
            this.ridesService = ridesService;
        }
        [HttpGet]
        [Route("{userId}",Name = RouteNames.RidesForUser)]
        public async Task<IActionResult> Rides(string userId)
        {
            return Ok(await ridesService.GetRide(userId));
        }
        [HttpGet]
        public async Task<IActionResult> Rides()
        {
            return Ok(await ridesService.GetRides());
        }
        [HttpGet]
        [Route("/me")]
        public async Task<IActionResult> RidesMe()
        {
            return Ok(await ridesService.GetRide(User.GetId()));
        }
    }
}