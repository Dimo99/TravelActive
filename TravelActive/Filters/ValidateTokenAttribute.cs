using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using TravelActive.Common.Utilities;
using TravelActive.Data;
using TravelActive.Models.Entities;

namespace TravelActive.Filters
{
    public class ValidateTokenAttribute : Attribute,IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userManager = context.HttpContext.RequestServices.GetService<UserManager<User>>();
            var dbContext = context.HttpContext.RequestServices.GetService<TravelActiveContext>();
            string token = context.HttpContext.Request.Headers["Authorization"].ToString();
            if (token.Length <= 7)
            {
                return;
            }
            token = token.Substring(7);

            string id;
            try
            {
                var jwtSecurity = new JwtSecurityToken(token);
                id = jwtSecurity.Claims.First(claim => claim.Type == Constants.Claims.Id).Value;
            }
            catch (Exception)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var user = Task.Run(() => userManager.FindByIdAsync(id)).Result;
            if (user == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            HashSet<string> tokens = dbContext
                .BlockedTokenses
                .Where(b => b.UserId == user.Id)
                .Select(x => x.Token)
                .ToHashSet();
            if (tokens.Contains(token))
            {
                context.Result = new UnauthorizedResult();
                return;
                
            }
            
        }
        
    }
}