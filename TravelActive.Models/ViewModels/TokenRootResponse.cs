using Api.ION;

namespace TravelActive.Models.ViewModels
{
    public class TokenRootResponse : Resource
    {
        public Form LoginForm { get; set; }
        public Form ConfirmEmailForm { get; set; }
        public Link GetConfirmToken { get; set; }
    }
}