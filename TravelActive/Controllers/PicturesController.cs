using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TravelActive.Common.Utilities;
using TravelActive.Filters;
using TravelActive.Models.BindingModels;
using TravelActive.Models.ViewModels;
using TravelActive.Services;

namespace TravelActive.Controllers
{
    [Route("/[controller]")]
    public class PicturesController : Controller
    {
        private readonly FileService fileService;

        public PicturesController(FileService fileService)
        {
            this.fileService = fileService;
        }

        [Authorize]
        [ValidateToken]
        [ValidateEmailConfirmed]
        [HttpGet]
        public async Task<IActionResult> ProfilePicture()
        {
            PictureViewModel picture = await fileService.GetProfilePicture(User);
            return Ok(picture);
        }
        [Authorize]
        [ValidateToken]
        [ValidateEmailConfirmed]
        [HttpPost()]
        public async Task<IActionResult> ProfilePicture([FromBody]PictureBindingModel profilePicture)
        {
            if (!profilePicture.MediaType.EndsWith("jpg") && !profilePicture.MediaType.EndsWith("png"))
            {
                return BadRequest(new ApiError("Unsupported file format! Allowed formats png and jpg"));
            }

            if (profilePicture.Value.Length > Constants.MaxProfilePictureSize)
            {
                return BadRequest(new ApiError("ProfilePicture should be smaller than 4Mb"));
            }
            await fileService.SaveProfilePicture(profilePicture, User);
            return Ok();
        }
    }
}