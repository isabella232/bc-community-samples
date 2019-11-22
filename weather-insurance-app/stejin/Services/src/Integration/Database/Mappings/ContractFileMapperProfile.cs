using System;
using AutoMapper;

namespace WeatherInsurance.Integration.Database.Mappings
{
    public class ContractFileMapperProfile : Profile
    {
        public ContractFileMapperProfile()
        {
            RecognizePrefixes("ContractFile");
            RecognizeDestinationPrefixes("ContractFile");

            CreateMap<Database.Model.ContractFile, Domain.Model.ContractFile>();

            CreateMap<Database.Model.ContractFile, Domain.Model.ContractFile>().ReverseMap();

        }
    }
}
