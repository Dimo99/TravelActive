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
        private BusService busService;

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
        public IActionResult Bus(int id)
        {
            ComplexBusViewModel bus = busService.GetBus(id);
            return Ok(bus);
        }

    }
}