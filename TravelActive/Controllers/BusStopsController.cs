using System;
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
            var busStops = await busService.GetAllBusStopsAsync(searchOptions);
            var busStopsResponse = new BusStopstResponse()
            {
                Self = LinkGenerator.ToCollection(RouteNames.ListBusStops),
                Value = busStops.ToArray(),
                BusStopSequence = LinkGenerator.ToCollection(RouteNames.StopSequence, new { busId = "exampleBusId" }),
                BusStopForm = FormMetadata.FromModel(new BusStopBindingModel(), LinkGenerator.ToForm(RouteNames.PostBusStop, null, LinkGenerator.PostMethod, Form.CreateRelation)),
                BusStopByName = LinkGenerator.To(RouteNames.BusStop, new { id = "exampleBusStopId" }),
                DepartureTimes = LinkGenerator.ToCollection(RouteNames.DepartureTimes, new { busId = "exampleBusId" }),
                BusStopsQueryForm = FormHelper.FromResource<BusStopViewModel>(LinkGenerator.ToForm(RouteNames.ListBusStops, null, LinkGenerator.GetMethod, Form.QueryRelation)),

            };
            return Ok(busStopsResponse);
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

        [HttpGet("{id}", Name = RouteNames.BusStop)]
        public async Task<IActionResult> BusStop(int id, [FromQuery]string dateTime)
        {
            DateTime dateTimeObj = DateTime.Parse(dateTime);
            var busStop = await busService.GetBusStop(id, dateTimeObj);
            if (busStop == null)
            {
                return BadRequest(new ApiError($"There is no {id} stop in the database"));
            }
            busStop.Self = LinkGenerator.To(RouteNames.BusStop, new { id = id, dateTime = dateTime });
            return Ok(busStop);
        }


    }
}