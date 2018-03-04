using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TravelActive.Common.Extensions;
using TravelActive.Common.Utilities;
using TravelActive.Data;
using TravelActive.Models.BindingModels;
using TravelActive.Models.Entities;
using TravelActive.Models.ViewModels;

namespace TravelActive.Services
{
    public class FileService : Service
    {
        private readonly IMapper mapper;
        public FileService(TravelActiveContext context, IMapper mapper) : base(context)
        {
            this.mapper = mapper;
        }

        public async Task SaveProfilePicture(PictureBindingModel profilePicture, ClaimsPrincipal user)
        {
            Picture profilePictureEntity = mapper.Map<Picture>(profilePicture);
            var userDb = await Context.Users.FindAsync(user.Claims.First(c => c.Type == Constants.Claims.Id).Value);
            userDb.ProfilePicture = profilePictureEntity;
            Context.Users.Update(userDb);
            await Context.SaveChangesAsync();
        }


        public async Task<PictureViewModel> GetProfilePicture(ClaimsPrincipal user)
        {
            var profilePicture = await Context.Users.Select(x => new { x.ProfilePicture, userId = x.Id }).FirstAsync(x => x.userId == user.GetId());
            return mapper.Map<PictureViewModel>(profilePicture.ProfilePicture);
        }
    }
}