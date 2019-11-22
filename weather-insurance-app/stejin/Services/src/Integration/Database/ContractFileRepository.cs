using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using WeatherInsurance.Integration.Database.Mappings;

namespace WeatherInsurance.Integration.Database
{
    public class ContractFileRepository : RepositoryBase<Integration.Database.Model.ContractFile, Domain.Model.ContractFile>, IRepository<Domain.Model.ContractFile>
    {
        public ContractFileRepository(Context context)
            : base(context, new List<Profile>() { new ContractFileMapperProfile() })
        { }

        public override async Task<long> AddNew(Domain.Model.ContractFile model)
        {
            var entity = _mapper.Map<Database.Model.ContractFile>(model);
            await _context.Set<Database.Model.ContractFile>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.ContractFileId;
        }

    }
}
