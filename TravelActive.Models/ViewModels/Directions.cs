using System.Collections.Generic;
using Api.ION;

namespace TravelActive.Models.ViewModels
{
    public class Directions : Resource
    {
        public decimal Distance { get; set; }
        public decimal Duration { get; set; }
        public ICollection<string> Polylines { get; set; } = new List<string>();

        public static Directions operator +(Directions directions,Directions directions2)
        {
            Directions toReturn = new Directions();
            toReturn.Distance = directions.Distance + directions2.Distance;
            toReturn.Duration = directions.Duration + directions2.Duration;
            foreach (var directionsPolyline in directions.Polylines)
            {
                toReturn.Polylines.Add(directionsPolyline);
            }
            foreach (var polyline in directions2.Polylines)
            {
                toReturn.Polylines.Add(polyline);
            }
            return toReturn;
        }
    }
}