using System;
using AutoMapper;

namespace WeatherInsurance.Integration.Database.Mappings
{
    public class DeployedContractMapperProfile : Profile
    {

        public DeployedContractMapperProfile()
        {
            RecognizePrefixes("Contract");
            RecognizeDestinationPrefixes("Contract");

            CreateMap<Database.Model.DeployedContract, Domain.Model.DeployedContract>()
            .ForMember(dest => dest.ContractType, opt => opt.MapFrom(o => (Domain.Model.ContractType)o.ContractType));

            CreateMap<Domain.Model.DeployedContract, Database.Model.DeployedContract>()
                .ForMember(dest => dest.ContractFile, opt => opt.Ignore())
                .ForMember(dest => dest.ContractFileId, opt => opt.MapFrom(o => o.ContractFile.Id))
                .ForMember(dest => dest.Network, opt => opt.Ignore())
                .ForMember(dest => dest.NetworkId, opt => opt.MapFrom(o => o.Network.Id));
     
        }
    }

}