using Microsoft.AspNetCore.Identity;

namespace TravelActive.Models.Entities
{
    public class UserRole : IdentityRole
    {
        public UserRole()
        {

        }

        public UserRole(string roleName) 
            : base(roleName)
        {

        }
    }
}