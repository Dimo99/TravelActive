using Api.ION;
using AutoMapper;
using Newtonsoft.Json;
using TravelActive.Common.Mapping;
using TravelActive.Common.Utilities;
using TravelActive.Models.Entities;

namespace TravelActive.Models.ViewModels
{
    public class BusStopViewModel : StopViewModel, IHaveCustomMapping
    {
        public int Id { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Delay { get; set; }

        public void ConfigureMapping(Profile profile)
        {
            profile.CreateMap<BusStop, BusStopViewModel>()
                .ForMember(dest => dest.LatLng,
                    opt => opt.MapFrom(src =>
                        new LatLng(double.Parse(src.Latitude), double.Parse(src.Longitude))))
                .ForMember(dest => dest.Self, 
                    opt => opt.MapFrom(src =>
                        LinkGenerator.To(RouteNames.BusStop, new { name = src.StopName })));
            profile.CreateMap<StopOrdered, BusStopViewModel>()
                .ForMember(dest => dest.Delay,
                    opt => opt.MapFrom(src =>
                        $"{src.Delay.ToString().Substring(0, 2)}:{src.Delay.ToString().Substring(2)}"))
                .ForMember(dest => dest.StopName,
                    opt => opt.MapFrom(src => src.BusStop.StopName))
                .ForMember(dest => dest.Self, 
                    opt=>opt.MapFrom(src=>LinkGenerator.To(RouteNames.BusStop,new {name = src.BusStop.StopName})));
        }
    }
}