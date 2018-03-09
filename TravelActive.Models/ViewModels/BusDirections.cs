namespace TravelActive.Models.ViewModels
{
    public class BusDirections
    {
        public string Polyline { get; set; }
        public decimal Distance { get; set; }
        public decimal Duration { get; set; }
        public string Method { get; set; }
        public string Bus { get; set; }
        public string LocationStart { get; set; }
        public string LocationEnd { get; set; }
    }
    
}