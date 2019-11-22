using System;
using AutoMapper;

namespace WeatherInsurance.Integration.Database.Mappings
{
    public class FeeMapperProfile : Profile
    {
        public FeeMapperProfile()
        {
            RecognizePrefixes("Fee");
            RecognizeDestinationPrefixes("Fee");

            CreateMap<Database.Model.Fee, Domain.Model.Fee>();

            CreateMap<Database.Model.Fee, Domain.Model.Fee>().ReverseMap()
                .ForMember(dest => dest.Contract, opt => opt.Ignore())
                .ForMember(dest => dest.ContractAddress, opt => opt.MapFrom(o => o.Contract.Address))
                .ForMember(dest => dest.NetworkId, opt => opt.MapFrom(o => o.Contract.Network.Id));

        }
    }
}
