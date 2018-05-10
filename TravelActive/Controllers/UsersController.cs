using System.Linq;
using System.Threading;
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
    [Authorize]
    [Route("/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserService userService;
        public UsersController(UserService userService)
        {
            this.userService = userService;
        }
        
        [HttpGet]
        [Route("",Name = RouteNames.ListAllUsers)]
        public async Task<IActionResult> ListAllUsers()
        {
            Collection<UserViewModel> collection = new Collection<UserViewModel>()
            {
                Self = LinkGenerator.To(RouteNames.ListAllUsers),
                Value = await userService.GetAllUsers()
            };
            return Ok(collection);
        }

        [HttpGet()]
        [Route("{userId}",Name = RouteNames.UserId)]
        public async Task<IActionResult> UserResult(string userId)
        {
            return Ok(await userService.GetUserByIdAsync(userId));
        }
        [HttpPost(Name = RouteNames.UsersRegister)]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterForm registerForm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));
            var (succeded, errors) = await userService.CreateUserAsync(registerForm, ct);
            if (succeded)
            {
                return Ok();
            }

            return BadRequest(new ApiError(errors.First()));
        }
    }
}