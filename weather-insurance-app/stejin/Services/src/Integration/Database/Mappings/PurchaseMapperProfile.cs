using System;
using System.Numerics;
using AutoMapper;

namespace WeatherInsurance.Integration.Database.Mappings
{

    public class PurchaseMapperProfile : Profile
    {
        public PurchaseMapperProfile()
        {
            RecognizePrefixes("Purchase");
            RecognizeDestinationPrefixes("Purchase");


            CreateMap<Database.Model.Purchase, Domain.Model.Purchase>().ReverseMap()
                .ForMember(dest => dest.Contract, opt => opt.Ignore())
                .ForMember(dest => dest.ContractAddress, opt => opt.MapFrom(o => o.Contract.Address))
                .ForMember(dest => dest.NetworkId, opt => opt.MapFrom(o => o.Contract.Network.Id));
        }
    }
}
