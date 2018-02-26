using Api.ION;

namespace TravelActive.Models.ViewModels
{
    public class UsersRootResponse : Resource
    {
        public Form UserRegister { get; set; }
        public Link UserMe { get; set; }
        public Form ForgotenPassword { get; set; }
        public Form ResetPasswordWithToken { get; set; }
        public Form ChangePassword { get; set; }


    }
}