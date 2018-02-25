using System;
using System.Linq;
using AutoMapper;

namespace TravelActive.Common.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            var types = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .First(a => a.GetName().Name == "TravelActive.Models")
                .GetTypes();
            types
                .Where(t => t.IsClass && !t.IsAbstract &&
                            t.GetInterfaces()
                                .Where(i => i.IsGenericType)
                                .Select(i => i.GetGenericTypeDefinition())
                                .Contains(typeof(IMapFrom<>)))
                .Select(t => new
                {
                    Destination = t,
                    Source = t
                        .GetInterfaces()
                        .Where(i => i.IsGenericType)
                        .Select(i => new
                        {
                            Definition = i.GetGenericTypeDefinition(),
                            Arguments = i.GetGenericArguments()
                        })
                        .Where(i => i.Definition == typeof(IMapFrom<>))
                        .SelectMany(i => i.Arguments)
                        .First()
                }).ToList()
                .ForEach(mapping=>this.CreateMap(mapping.Source,mapping.Destination));
            types
                .Where(t=>t.IsClass
                          && !t.IsAbstract
                          && typeof(IHaveCustomMapping).IsAssignableFrom(t))
                .Select(Activator.CreateInstance)
                .Cast<IHaveCustomMapping>()
                .ToList()
                .ForEach(mapping => mapping.ConfigureMapping(this));

        }
    }
}