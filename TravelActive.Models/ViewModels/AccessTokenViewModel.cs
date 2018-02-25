using System;

namespace TravelActive.Models.ViewModels
{
    public class AccessTokenViewModel
    {
        public string AccessToken { get; set; }
        public DateTimeOffset Expires { get; set; }
    }
}