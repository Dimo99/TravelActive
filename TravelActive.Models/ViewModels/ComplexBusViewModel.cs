using Api.ION;
using AutoMapper;
using TravelActive.Common.Mapping;
using TravelActive.Common.Utilities;
using TravelActive.Models.Entities;

namespace TravelActive.Models.ViewModels
{
    public class ComplexBusViewModel : Resource, IHaveCustomMapping
    {
        public string BusName { get; set; }
        public Link StopSequence { get; set; }
        public Link DepartureTimes { get; set; }
        public void ConfigureMapping(Profile profile)
        {
            profile.CreateMap<Bus, ComplexBusViewModel>()
                .ForMember(x => x.DepartureTimes,
                    options => options.MapFrom(x =>
                        LinkGenerator.ToCollection(RouteNames.DepartureTimes, new { busId = x.Id })))
                .ForMember(x => x.StopSequence,
                    options => options.MapFrom(x =>
                        LinkGenerator.ToCollection(RouteNames.StopSequence, new { busId = x.Id })))
                .ForMember(x => x.Self,
                    opt => opt.MapFrom(x =>
                        LinkGenerator.To(RouteNames.Bus, new { busId = x.Id })));
        }
    }
}