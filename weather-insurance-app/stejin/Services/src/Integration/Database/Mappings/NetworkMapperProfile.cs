using System;
using AutoMapper;

namespace WeatherInsurance.Integration.Database.Mappings
{
    public class NetworkMapperProfile : Profile
    {
        public NetworkMapperProfile()
        {
            RecognizePrefixes("Network");
            RecognizeDestinationPrefixes("Network");

            CreateMap<Database.Model.Network, Domain.Model.Network>()
                .ForMember(dest => dest.Platform, opt => opt.MapFrom(o => (Domain.Model.Platform)o.Platform))
                .ForMember(dest => dest.Url, opt => opt.MapFrom(o => new Uri(o.Url)));

            CreateMap<Database.Model.Network, Domain.Model.Network>().ReverseMap();

        }
    }
}
