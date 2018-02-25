namespace TravelActive.Models.ViewModels
{
    public class MiddleStop
    {
        public int StopId { get; set; }
        public int BusId { get; set; }
        public override int GetHashCode()
        {
            return StopId.GetHashCode() ^ BusId.GetHashCode();
        }
    }
}