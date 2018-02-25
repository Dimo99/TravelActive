using AutoMapper;
using TravelActive.Common.Mapping;
using TravelActive.Models.Entities;

namespace TravelActive.Models.ViewModels
{
    public class BicycleStopViewModel : StopViewModel, IHaveCustomMapping
    {
        public void ConfigureMapping(Profile profile)
        {
            profile.CreateMap<BicycleStop, BicycleStopViewModel>()
                .ForMember(dest => dest.LatLng,
                    opt => opt.MapFrom(src => new LatLng(double.Parse(src.Latitude), double.Parse(src.Longitude))));
        }
    }
}