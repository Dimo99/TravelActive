using System.Threading.Tasks;
using Api.ION;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TravelActive.Common.Utilities;
using TravelActive.Filters;
using TravelActive.Models.BindingModels;
using TravelActive.Models.Entities;
using TravelActive.Models.ViewModels;
using TravelActive.Services;

namespace TravelActive.Controllers
{
    [Route("/[controller]")]
    public class TokensController : Controller
    {
        private readonly TokenService tokenService;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        public TokensController(TokenService tokenService, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            this.tokenService = tokenService;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet(Name = RouteNames.TokenRoot)]
        public IActionResult TokenRoot()
        {
            var response = new TokenRootResponse()
            {
                Self = LinkGenerator.To(RouteNames.TokenRoot),
                LoginForm = FormMetadata.FromModel(new LoginBindingModel(), LinkGenerator.ToForm(RouteNames.TokenLogin,null,LinkGenerator.PostMethod,Form.Relation)),
                ConfirmEmailForm = FormMetadata.FromModel(new EmailConfirmationBindingModel(), LinkGenerator.ToForm(RouteNames.ConfirmEmailPost,null,LinkGenerator.PostMethod,Form.Relation)),
                GetConfirmToken = LinkGenerator.To(RouteNames.ConfirmEmailGet)
            };
            return Ok(response);
        }
        [HttpPost(Name = RouteNames.TokenLogin)]
        public async Task<IActionResult> LoginUserAsync([FromBody]LoginBindingModel lbm)
        {
            if (!ModelState.IsValid) return BadRequest(new ApiError(ModelState));
            var result = await signInManager.PasswordSignInAsync(lbm.Email, lbm.Password, isPersistent: false,
                lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return Unauthorized();
            }
            var user = await userManager.FindByNameAsync(lbm.Email);
            var accessToken = tokenService.GetAccessTokenAsync(user);
            return Ok(accessToken);
        }

        [Authorize]
        [HttpDelete]
        [ValidateToken]
        public async Task<IActionResult> LogoutAsync()
        {
            await tokenService.LogOut(HttpContext.Request.Headers["Authorization"]);
            return Ok();
        }
    }
}