using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TravelActive.Common.Utilities;
using TravelActive.Filters;
using TravelActive.Services;

namespace TravelActive.Controllers
{
    [Route("/[controller]")]
    public class FilesController : Controller
    {
        private readonly FileService fileService;

        public FilesController(FileService fileService)
        {
            this.fileService = fileService;
        }

        [Authorize]
        [ValidateToken]
        [HttpPost("ProfilePicture")]
        public async Task<IActionResult> ProfilePicture([FromForm]IFormFile profilePicture)
        {
            if (!profilePicture.FileName.EndsWith(".jpg") && !profilePicture.FileName.EndsWith(".png"))
            {
                return BadRequest(new { Error = "Unsupported picture format!", AllowedFormats = "jpg, png" });
            }

            if (profilePicture.Length > Constants.MaxProfilePictureSize)
            {
                return BadRequest(new { Error = "ProfilePicture should be smaller than 4Mb" });
            }
            await fileService.SaveProfilePicture(profilePicture, User);

            return Ok();
        }
    }
}