namespace TravelActive.Models.ViewModels
{
    public class SuccessObject
    {
        public SuccessObject(string value)
        {
            this.Success = value;
        }
        public string Success { get; set; }
    }
}