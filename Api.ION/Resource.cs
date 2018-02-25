using Newtonsoft.Json;

namespace Api.ION
{
    public abstract class Resource : Link
    {
        [JsonIgnore]
        public Link Self { get; set; }
    }
}