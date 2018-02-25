using System.ComponentModel;
using Newtonsoft.Json;

namespace Api.ION
{
    public class Link
    {
        [JsonProperty(Order = -4)]
        public string Href { get; set; }
        [JsonProperty(Order = -3, NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        [DefaultValue(LinkGenerator.GetMethod)]
        public string Method { get; set; }
        [JsonProperty(Order = -2, NullValueHandling = NullValueHandling.Ignore, PropertyName = "rel")]
        public string[] Relations { get; set; }
        [JsonIgnore]
        public string RouteName { get; set; }
        [JsonIgnore]
        public object RouteValues { get; set; }
    }
}