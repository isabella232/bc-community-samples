using System;
using System.Collections.Generic;
using AutoMapper;
using WeatherInsurance.Integration.Database.Mappings;

namespace WeatherInsurance.Integration.Database
{
    public class ContractFileRepository : RepositoryBase<Integration.Database.Model.ContractFile, Domain.Model.ContractFile>, IRepository<Domain.Model.ContractFile>
    {
        public ContractFileRepository(Context context)
            : base(context, new List<Profile>() { new ContractFileMapperProfile() })
        { }

        public override long AddNew(Domain.Model.ContractFile model)
        {
            var entity = _mapper.Map<Database.Model.ContractFile>(model);
            _context.Set<Database.Model.ContractFile>().Add(entity);
            _context.SaveChanges();
            return entity.ContractFileId;
        }

    }
}
