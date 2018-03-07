namespace TravelActive.Common.Utilities
{
    public static class Constants
    {
        public static readonly Claims Claims = new Claims()
        {
            Id = "id",
            Username = "username",
            Email = "Email",
            EmailConfirmed = "emailConfirmed",
            Exparation = "exp",
            Roles = "roles"
        };

        public const int MaxProfilePictureSize = 4000000;
        public const string BicycleRouteUrl = "http://127.0.0.1:5001/route/v1/bike/";
        public const string FootRouteUrl = "http://127.0.0.1:5002/route/v1/foot/";
        public const string CarRouteUrl = "http://127.0.0.1:5003/route/v1/car/";
        public const string CoordinatesRegex = "^[0-9]+\\.[0-9]+\\,[0-9]+\\.[0-9]+$";
    }
}