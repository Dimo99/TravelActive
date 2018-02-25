using System.Linq;

namespace TravelActive.Models.Entities
{
    public class LatLng
    {
        public LatLng(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }
        public static LatLng Parse(string latlng)
        {
            double[] tokens = latlng.Split(',').Select(double.Parse).ToArray();
            return new LatLng(tokens[0], tokens[1]);
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}