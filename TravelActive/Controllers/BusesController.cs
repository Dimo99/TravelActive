using System.Threading.Tasks;
using Api.ION;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelActive.Common.Utilities;
using TravelActive.Models.BindingModels;
using TravelActive.Models.ViewModels;
using TravelActive.Services;

namespace TravelActive.Controllers
{
    [Authorize()]
    [Route("/[controller]")]
    public class BusesController : Controller
    {
        private readonly BusService busService;

        public BusesController(BusService busService)
        {
            this.busService = busService;
        }

        [HttpGet(Name = RouteNames.Buses)]
        public async Task<IActionResult> Buses()
        {
            var buses = await busService.GetBuses();
            var busesResponse = new BusesResponse()
            {
                Self = LinkGenerator.ToCollection(RouteNames.Buses),
                Value = buses.ToArray(),
                BusForm = FormMetadata.FromModel(new BusBindingModel(),
                    LinkGenerator.ToForm(RouteNames.PostBus, null, LinkGenerator.PostMethod, Form.CreateRelation))
            };
            return Ok(busesResponse);
        }
        [Authorize(Roles = "Moderator")]
        [HttpPost(Name = RouteNames.PostBus)]
        public IActionResult Bus([FromBody] BusBindingModel busBindingModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var busId = busService.CreateBus(busBindingModel);
            return Ok();
        }

        [HttpGet("{busId}", Name = RouteNames.Bus)]
        public IActionResult Bus(int busId)
        {
            ComplexBusViewModel bus = busService.GetBus(busId);
            return Ok(bus);
        }
        [HttpGet("bussstops/{busId}", Name = RouteNames.StopSequence)]
        public async Task<IActionResult> BusStopSequence(int busId)
        {
            var stops = await busService.GetBusStops(busId);
            if (stops == null)
            {
                return BadRequest(new ApiError("No bus stops info for the bus"));
            }
            Collection<BusStopViewModel> busStops = new Collection<BusStopViewModel>()
            {
                Self = LinkGenerator.ToCollection(RouteNames.StopSequence, new { parameter = busId }),
                Value = stops.ToArray()
            };
            return Ok(busStops);
        }
        [HttpGet("departuretimes/{busId}", Name = RouteNames.DepartureTimes)]
        public async Task<IActionResult> BusDepartureTimes(int busId)
        {
            var departureTimes = await busService.GetBusDepartureTimes(busId);
            if (departureTimes.Value?.Length == 0)
            {
                return BadRequest(new ApiError("No departure times info for the bus"));
            }
            departureTimes.Self = LinkGenerator.ToCollection(RouteNames.DepartureTimes, new { busId });
            return Ok(departureTimes);
        }

    }
}