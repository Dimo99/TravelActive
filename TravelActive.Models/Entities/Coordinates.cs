using AutoMapper;
using TravelActive.Common.Mapping;
using TravelActive.Models.BindingModels;

namespace TravelActive.Models.Entities
{
    public class Coordinates : IHaveCustomMapping
    {
        public LatLng StartingPoint { get; set; }
        public LatLng EndPoint { get; set; }


        public void ConfigureMapping(Profile profile)
        {
            profile.CreateMap<CoordinatesBindingModel, Coordinates>()
                .ForMember(dest => dest.StartingPoint,
                    opt => opt.MapFrom(src => LatLng.Parse(src.StartingPoint)))
                .ForMember(dest => dest.EndPoint,
                    opt => opt.MapFrom(src => LatLng.Parse(src.DestinationPoint)));
        }
    }
}