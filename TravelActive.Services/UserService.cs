using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TravelActive.Common.Extensions;
using TravelActive.Data;
using TravelActive.Models.BindingModels;
using TravelActive.Models.Entities;
using TravelActive.Models.ViewModels;

namespace TravelActive.Services
{
    public class UserService : Service
    {
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;
        public UserService(TravelActiveContext context, IMapper mapper, UserManager<User> userManager) : base(context)
        {
            this.mapper = mapper;
            this.userManager = userManager;
        }

        public async Task<(bool Succeed,IEnumerable<string> Errors)> CreateUserAsync(RegisterForm registerForm, CancellationToken ct)
        {
            var user = mapper.Map<User>(registerForm);
            var result = await userManager.CreateAsync(user, registerForm.Password);
            if (!result.Succeeded)
            {
                return (false,result.Errors.Select(x=>x.Description));
            }

            return (true, null);
        }

        public async Task<User> GetUserAsync(ClaimsPrincipal claimsPrincipal)
        {
            var id = claimsPrincipal.GetId();
            return await userManager.FindByIdAsync(id);
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string emailToken)
        {
            var result = await userManager.ConfirmEmailAsync(user, emailToken);
            return result;
        }

        public async Task<string> GenerateEmailTokenAsync(User user)
        {
            return await userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<UserViewModel> GetUserViewModelAsync(ClaimsPrincipal user)
        {
            return Mapper.Map<UserViewModel>(await this.GetUserAsync(user));
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }
    }
}