using System.Threading.Tasks;
using Api.ION;
using Api.Query;
using Api.Query.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelActive.Common.Utilities;
using TravelActive.Models.BindingModels;
using TravelActive.Models.Entities;
using TravelActive.Models.ViewModels;
using TravelActive.Services;

namespace TravelActive.Controllers
{
    [Authorize]
    [Route("/[controller]")]
    public class BusStopsController : Controller
    {
        private readonly BusService busService;
        public BusStopsController(BusService busService)
        {
            this.busService = busService;
        }
        [HttpGet(Name = RouteNames.ListBusStops)]
        public async Task<IActionResult> BusStops([FromQuery]SearchOptions<BusStopViewModel, BusStop> searchOptions)
        {
            var busStops = await busService.GetAllBusStops(searchOptions);
            var busStopsResponse = new BusStopstResponse()
            {
                Self = LinkGenerator.ToCollection(RouteNames.ListBusStops),
                Value = busStops.ToArray(),
                BusStopSequence = LinkGenerator.ToCollection(RouteNames.StopSequence, new { parameter = "exampleBusId" }),
                BusStopForm = FormMetadata.FromModel(new BusStopBindingModel(), LinkGenerator.ToForm(RouteNames.PostBusStop, null, LinkGenerator.PostMethod, Form.CreateRelation)),
                BusStopByName = LinkGenerator.To(RouteNames.BusStop, new { name = "exampleBusStopName" }),
                DepartureTimes = LinkGenerator.ToCollection(RouteNames.DepartureTimes, new { busId = "exampleBusId" }),
                BusStopsQueryForm = FormHelper.FromResource<BusStopViewModel>(LinkGenerator.ToForm(RouteNames.ListBusStops,null,LinkGenerator.GetMethod,Form.QueryRelation)),

            };
            return Ok(busStopsResponse);
        }

        [HttpGet("{parameter}", Name = RouteNames.StopSequence)]
        public async Task<IActionResult> BusStopSequence(string parameter)
        {
            if (int.TryParse(parameter, out int busId))
            {
                return await BusStop(parameter);
            }
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
        [Authorize(Roles = "Moderator")]
        [HttpPost(Name = RouteNames.PostBusStop)]
        public IActionResult BusStop([FromBody] BusStopBindingModel busStopBindingModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiError(ModelState));
            }
            busService.AddBusStop(busStopBindingModel);
            return Ok();
        }

        [HttpGet("{name}", Name = RouteNames.BusStop)]
        public async Task<IActionResult> BusStop(string name)
        {
            var busStop = await busService.GetBusStop(name);
            if (busStop == null)
            {
                return BadRequest(new ApiError($"There is no {name} stop in the database"));
            }
            busStop.Self = LinkGenerator.To(RouteNames.BusStop, new { name = busStop.StopName });
            return Ok(busStop);
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