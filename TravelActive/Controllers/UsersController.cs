using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TravelActive.Common.Extensions;
using TravelActive.Common.Utilities;
using TravelActive.Filters;
using TravelActive.Models.BindingModels;
using TravelActive.Models.Entities;
using TravelActive.Models.Forms;
using TravelActive.Models.ViewModels;
using TravelActive.Services;

namespace TravelActive.Controllers
{
    [Route("/[controller]")]
    public class UsersController : Controller
    {
        private readonly UserService userService;
        private readonly UserManager<User> userManager;
        private readonly EmailService emailService;

        public UsersController(UserService userService, UserManager<User> userManager, EmailService emailService)
        {
            this.userService = userService;
            this.userManager = userManager;
            this.emailService = emailService;
        }

        [HttpPost(Name = RouteNames.UsersRegister)]
        public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterForm registerForm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));
            var (succeded, errors) = await userService.CreateUserAsync(registerForm, ct);
            if (succeded)
            {
                return Created(Url.Link(nameof(GetMeAsync),null),null);
            }

            return BadRequest(new ApiError(errors.First()));
        }
        [Authorize]
        [HttpGet("me", Name = RouteNames.UsersMe)]
        public async Task<IActionResult> GetMeAsync(CancellationToken ct)
        {
            var user = await userService.GetUserViewModelAsync(User);
            return Ok(user);
        }

        [HttpPost("forgotenpassword", Name = RouteNames.SendPasswordRecovory)]
        public async Task<IActionResult> SendPasswordRecovoryAsync([FromBody] ForgotenPasswordBindingModel fpbm,
            CancellationToken ct)
        {
            var user = await userService.GetUserByEmailAsync(fpbm.Email);
            if (user == null)
            {
                return BadRequest(new ApiError("No user with such email!"));
            }
            if (!await userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest(new ApiError("We are sorry but this email wasn't confirmed!"));
            }
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            var message = $"{user.Id}&{code}";
            await emailService.SendForgotenPasswordEmailAsync(user.Email, message);
            return Ok(new SuccessObject("Check your email for password recovory information"));
        }
        [HttpPost("resetpassword",Name = RouteNames.ResetPassword)]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordBindingModel rpbm)
        {
            string userId = rpbm.Token.Split('&')[0];
            string code = rpbm.Token.Split('&')[1];
            var user = await userManager.FindByIdAsync(userId);
            var result = await userManager.ResetPasswordAsync(user, code, rpbm.Password);
            if (result.Succeeded)
            {
                return Ok(new SuccessObject("Successfuly reseted password"));
            }

            return BadRequest(result.Errors);
        }
        [Authorize]
        [ValidateToken]
        [ValidateEmailConfirmed]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordBindingModel cpbm)
        {
            var user = await userManager.FindByIdAsync(User.GetId());
            

            var changePasswordResult = await userManager.ChangePasswordAsync(user, cpbm.OldPassword, cpbm.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                return BadRequest(changePasswordResult.Errors);
            }

            return Ok(new SuccessObject("Your password has been changed"));
        }
    }
}