using System.Collections.Generic;
using System.Threading.Tasks;
using Api.ION;
using Api.Query;
using AutoMapper;
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
    public class DirectionsController : Controller
    {
        private BicycleDirectionsService bicycleDirectionsService;
        private BusDirectionsService busDirectionsService;
        private IMapper mapper;
        public DirectionsController(IMapper mapper, BicycleDirectionsService bicycleDirectionsService,
            BusDirectionsService busDirectionsService)
        {
            this.mapper = mapper;
            this.bicycleDirectionsService = bicycleDirectionsService;
            this.busDirectionsService = busDirectionsService;
        }
        [HttpGet(Name = RouteNames.DirectionsRoot)]
        public IActionResult Root()
        {
            var responese = new DirectionsRootResponse
            {
                BusQueryForm = FormHelper.DirectionsQuery<CoordinatesBindingModel>(
                    LinkGenerator.ToForm(RouteNames.BusQuery, null, LinkGenerator.GetMethod, Form.QueryRelation)),
                BicycleQueryForm = FormHelper.DirectionsQuery<CoordinatesBindingModel>(
                    LinkGenerator.ToForm(RouteNames.CycleQuery, null, LinkGenerator.GetMethod, Form.QueryRelation)),
                Self = LinkGenerator.To(RouteNames.DirectionsRoot),
                BicycleStop = FormMetadata.FromModel(new BicycleStopBindingModel(),
                    LinkGenerator.ToForm(RouteNames.PostCycleStop, null, LinkGenerator.PostMethod, Form.CreateRelation))
            };
            return Ok(responese);
        }
        [Authorize(Roles = "Moderator")]
        [HttpPost("cycle", Name = RouteNames.PostCycleStop)]
        public async Task<IActionResult> Cycle([FromBody] BicycleStopBindingModel bicycleStop)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiError(ModelState));
            }
            await bicycleDirectionsService.AddBicycleStop(bicycleStop);
            // TODO: Come up with something nice
            return Ok();
        }

        [HttpGet("bus", Name = RouteNames.BusQuery)]
        public async Task<IActionResult> Bus([FromQuery] CoordinatesBindingModel cbm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiError(ModelState));
            }
            Coordinates coordinates = mapper.Map<Coordinates>(cbm);
            List<BusDirections> directions = await busDirectionsService.BusAlgorithm(coordinates);
            List<BusDirectionsView> busDirections = new List<BusDirectionsView>();
            foreach (var busDirectionse in directions)
            {
                busDirections.Add(new BusDirectionsView()
                {
                    SubBusDirectionses = busDirectionse.Directions.ToArray()
                });
            }
            Collection<BusDirectionsView> collection = new Collection<BusDirectionsView>()
            {
                Value = busDirections.ToArray()
            };

            return Ok(collection);
        }

        [HttpGet("cycle", Name = RouteNames.CycleQuery)]
        public async Task<IActionResult> Cycle([FromQuery] CoordinatesBindingModel cbm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiError(ModelState));
            }
            //TODO : think if this should be done in service
            Coordinates coordinates = mapper.Map<Coordinates>(cbm);
            BicycleStopViewModel nearestBicycleStation =
                await bicycleDirectionsService.FindNEarestBicycleStop(coordinates.StartingPoint);
            BicycleStopViewModel nearestBicycleStationToLocation =
                await bicycleDirectionsService.FindNEarestBicycleStop(coordinates.EndPoint);
            Directions directionsToNearestStation =
                await bicycleDirectionsService.GetDirectionsByFoot(coordinates.StartingPoint, nearestBicycleStation.LatLng);
            Directions directionsBetweenStations =
                await bicycleDirectionsService.GetDirectionsBicycle(nearestBicycleStation.LatLng, nearestBicycleStationToLocation.LatLng);
            Directions directionsToTheEnd =
                await bicycleDirectionsService.GetDirectionsByFoot(nearestBicycleStationToLocation.LatLng, coordinates.EndPoint);
            Directions finalBycicleDirections = bicycleDirectionsService.SumDirections(directionsToNearestStation, directionsBetweenStations,
                directionsToTheEnd);
            Directions directionsByFoot =
                await bicycleDirectionsService.GetDirectionsByFoot(coordinates.StartingPoint, coordinates.EndPoint);
            if (bicycleDirectionsService.Compare(finalBycicleDirections, directionsByFoot) > 0)
            {

                return Ok(finalBycicleDirections);
            }

            return Ok(directionsByFoot);
        }

    }
}