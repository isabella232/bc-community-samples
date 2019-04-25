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
    public class FeeRepository : RepositoryBase<Integration.Database.Model.Fee, Domain.Model.Fee>, IRepository<Domain.Model.Fee>
    {
        public FeeRepository(Context context)
            : base(context, new List<Profile>() {
            new FeeMapperProfile(),
            new DeployedContractMapperProfile(),
            new ContractFileMapperProfile(),
            new NetworkMapperProfile() })
        { }

        public override async Task<Domain.Model.Fee> Get(params object[] keys)
        {
            long feeId = (long)keys.First();
            var model = await _context.Set<Database.Model.Fee>()
                                .Include(e => e.Contract).ThenInclude(e => e.Network)
                                .SingleOrDefaultAsync(e => e.FeeId == feeId);

            return _mapper.Map<Domain.Model.Fee>(model);
        }

        public override async Task<IEnumerable<Domain.Model.Fee>> Find(Expression<Func<Database.Model.Fee, bool>> query)
        {
            var models = await _context.Set<Database.Model.Fee>()
                .Include(m => m.Contract).ThenInclude(e => e.Network)
                .Where(query)
                .ToListAsync();
            return _mapper.Map<IEnumerable<Domain.Model.Fee>>(models);
        }

        public override async Task<long> AddNew(Domain.Model.Fee model)
        {
            var entity = _mapper.Map<Database.Model.Fee>(model);
            await _context.Set<Database.Model.Fee>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.FeeId;
        }

    }
}

