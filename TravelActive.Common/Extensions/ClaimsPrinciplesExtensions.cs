using System.Linq;
using System.Security.Claims;
using TravelActive.Common.Utilities;

namespace TravelActive.Common.Extensions
{
    public static class ClaimsPrinciplesExtensions
    {
        public static string GetId(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.First(x => x.Type == Constants.Claims.Id).Value;
        }
    }
}