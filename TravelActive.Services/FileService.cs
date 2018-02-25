using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TravelActive.Common.Extensions;
using TravelActive.Common.Utilities;
using TravelActive.Data;

namespace TravelActive.Services
{
    public class FileService : Service
    {
        public FileService(TravelActiveContext context) : base(context)
        {
        }

        public async Task SaveProfilePicture(IFormFile profilePicture, ClaimsPrincipal user)
        {
            byte[] byteArray = await profilePicture.ToByteArrayAsync();

            string pictureUrl = "wwwroot/ProfilePictures/" + user.Claims.First(x => x.Type == Constants.Claims.Username).Value + "ProfilePicture." + profilePicture.GetFileType();
            await File.WriteAllBytesAsync(path: pictureUrl, bytes: byteArray);
            var userDb = await Context.Users.FindAsync(user.Claims.First(c => c.Type == Constants.Claims.Id).Value);
            userDb.PictureUrl = pictureUrl;
            Context.Users.Update(userDb);
            await Context.SaveChangesAsync();
        }

        
    }
}