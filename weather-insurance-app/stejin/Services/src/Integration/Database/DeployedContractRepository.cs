using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WeatherInsurance.Integration.Database.Mappings;

namespace WeatherInsurance.Integration.Database
{
    public class DeployedContractRepository : RepositoryBase<Integration.Database.Model.DeployedContract, Domain.Model.DeployedContract>, IRepository<Domain.Model.DeployedContract>
    {

        public DeployedContractRepository(Context context)
            : base(context, new List<Profile>() {
            new DeployedContractMapperProfile(),
            new ContractFileMapperProfile(),
            new NetworkMapperProfile() })
        { }

        public override Domain.Model.DeployedContract Get(params object[] keys)
        {
            string contractAddress = keys.First().ToString();
            var model = _context.Set<Database.Model.DeployedContract>()
                                .Include(e => e.Network)
                                .Include(e => e.ContractFile)
                                .SingleOrDefault(e => e.ContractAddress == contractAddress);
            return _mapper.Map<Domain.Model.DeployedContract>(model);
        }

        public override IEnumerable<Domain.Model.DeployedContract> Find(Expression<Func<Database.Model.DeployedContract, bool>> query)
        {
            var models = _context.Set<Database.Model.DeployedContract>()
                .Include(m => m.ContractFile)
                .Include(m => m.Network)
                .Where(query);
            return _mapper.Map<IEnumerable<Domain.Model.DeployedContract>>(models);
        }

    }
}
