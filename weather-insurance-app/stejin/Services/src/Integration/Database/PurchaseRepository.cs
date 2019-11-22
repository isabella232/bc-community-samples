using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WeatherInsurance.Integration.Database.Mappings;

namespace WeatherInsurance.Integration.Database
{
    public class PurchaseRepository : RepositoryBase<Integration.Database.Model.Purchase, Domain.Model.Purchase>, IRepository<Domain.Model.Purchase>
    {
        public PurchaseRepository(Context context)
            : base(context, new List<Profile>() {
            // new BigIntegerMapperProfile(),
            new PurchaseMapperProfile(),
            new DeployedContractMapperProfile(),
            new ContractFileMapperProfile(),
            new NetworkMapperProfile() })
        { }

        public override async Task<Domain.Model.Purchase> Get(params object[] keys)
        {
            long id = (long)keys.First();
            var model = await _context.Set<Database.Model.Purchase>()
                                .Include(e => e.Contract).ThenInclude(e => e.Network)
                                .SingleOrDefaultAsync(e => e.PurchaseId == id);

            return _mapper.Map<Domain.Model.Purchase>(model);
        }

        public override async Task<IEnumerable<Domain.Model.Purchase>> Find(Expression<Func<Database.Model.Purchase, bool>> query)
        {
            var models = await _context.Set<Database.Model.Purchase>()
                .Include(m => m.Contract).ThenInclude(e => e.Network)
                .Where(query)
                .ToListAsync();
            return _mapper.Map<IEnumerable<Domain.Model.Purchase>>(models);
        }

    }
}

