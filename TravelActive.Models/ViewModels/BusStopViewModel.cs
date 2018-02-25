using AutoMapper;
using TravelActive.Common.Mapping;
using TravelActive.Models.Entities;

namespace TravelActive.Models.ViewModels
{
    public class BusStopViewModel : StopViewModel, IHaveCustomMapping
    {
        public int Id { get; set; }
        public void ConfigureMapping(Profile profile)
        {
            profile.CreateMap<BusStop, BusStopViewModel>()
                .ForMember(dest => dest.LatLng,
                    opt => opt.MapFrom(src =>
                        new LatLng(double.Parse(src.Latitude), double.Parse(src.Longitude))));
        }
    }
}