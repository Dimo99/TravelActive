using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TravelActive.Common.Utilities;

namespace TravelActive.Filters
{
    public class ValidateEmailConfirmedAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string token = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (token.Length <= 7)
            {
                return;
            }
            token = token.Substring(7);
            bool emailConfirmed;
            try
            {
                if (token.Length <= 7)
                {
                    return;
                }

                token = token.Substring(7);
                JwtSecurityToken jwtToken = new JwtSecurityToken(token);
                emailConfirmed =
                    bool.Parse(jwtToken.Claims.First(x => x.Type == Constants.Claims.EmailConfirmed).Value);
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            if (!emailConfirmed)
            {
                 context.Result =  new BadRequestObjectResult(new
                {
                    Error = "Email isn't confirmed",
                    Info = "To resend confirmation link make request to account/resendconfirmlink"
                });
                return;
            }
        }
    }
}