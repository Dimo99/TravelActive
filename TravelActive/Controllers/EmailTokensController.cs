using System.Linq;
using System.Threading.Tasks;
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
    public class EmailTokensController : Controller
    {
        private readonly UserService userService;
        private readonly EmailService emailService;
        public EmailTokensController(UserService userService, EmailService emailService)
        {
            this.userService = userService;
            this.emailService = emailService;
        }

        [HttpPost(Name = RouteNames.ConfirmEmailPost)]
        public async Task<IActionResult> ConfirmEmailAsync([FromBody] EmailConfirmationBindingModel ecbm)
        {
            var user = await userService.GetUserAsync(User);
            var result = await userService.ConfirmEmailAsync(user,ecbm.Token);
            if (result.Succeeded)
            {
                return Ok(new SuccessObject("Email confirmation link has been send."));
            }

            return BadRequest(result.Errors.First());
        }

        [HttpGet(Name = RouteNames.ConfirmEmailGet)]
        public async Task<IActionResult> GetConfirmEmailTokenAsync()
        {
            var user = await userService.GetUserAsync(User);
            var code = await userService.GenerateEmailTokenAsync(user);
            await emailService.SendEmailConfirmationAsync(user.Email,code);
            return Ok();
        }

    }
}