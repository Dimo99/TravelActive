using AutoMapper;

namespace TravelActive.Common.Mapping
{
    public interface IHaveCustomMapping
    {
        void ConfigureMapping(Profile profile);
    }
}