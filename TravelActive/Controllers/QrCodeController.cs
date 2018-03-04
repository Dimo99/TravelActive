using Microsoft.AspNetCore.Mvc;
using TravelActive.Services;

namespace TravelActive.Controllers
{
    
    [Route("/[controller]")]
    public class QrCodeController : Controller
    {
        private readonly QrCodeService qrCodeService;
        private readonly BusService busService;
        public QrCodeController(QrCodeService qrCodeservice, BusService busService)
        {
            this.qrCodeService = qrCodeservice;
            this.busService = busService;
        }

        [HttpGet("{busId}")]
        public IActionResult GetQrCode(int busId)
        {
            var busName = busService.GetBus(busId).BusName;
            var qrCodeContent = new
            {
                BusName = busName
            };
            return Ok(qrCodeService.GenerateQrCodeImage(qrCodeContent));

        }
    }
}