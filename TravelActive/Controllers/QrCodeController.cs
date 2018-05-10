using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelActive.Models.Entities;
using TravelActive.Models.ViewModels;
using TravelActive.Services;

namespace TravelActive.Controllers
{
    [Authorize]
    [Route("/[controller]")]
    public class QrCodeController : Controller
    {
        private readonly QrCodeService qrCodeService;
        private readonly BusService busService;
        private readonly UserService userService;
        private readonly RidesService ridesService;
        public QrCodeController(QrCodeService qrCodeservice, BusService busService, UserService userService, RidesService ridesService)
        {
            this.qrCodeService = qrCodeservice;
            this.busService = busService;
            this.userService = userService;
            this.ridesService = ridesService;
        }

        [HttpGet("readed/{busId}")]
        public async Task<IActionResult> UserTraveling(int busId, [FromQuery] DateTime dateTime, CancellationToken ct)
        {

            User currentUser = await userService.GetUserAsync(User);
            Ride ride = new Ride()
            {
                BusId = busId,
                DateTime = dateTime,
                User = currentUser
            };
            await ridesService.AddRide(ride, ct);
            return Ok();
        }
        [HttpGet("{busId}")]
        public IActionResult GetQrCode(int busId)
        {
            var busName = busService.GetBus(busId).BusName;
            var qrCodeContent = new QrCodeContent
            {
                BusName = busName,
                BusId = busId
            };
            return Ok(qrCodeService.GenerateQrCodeImage(qrCodeContent));

        }
    }
}