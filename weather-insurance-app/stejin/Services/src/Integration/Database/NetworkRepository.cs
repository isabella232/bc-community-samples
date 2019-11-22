using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using WeatherInsurance.Integration.Database.Mappings;

namespace WeatherInsurance.Integration.Database
{
    public class NetworkRepository : RepositoryBase<Integration.Database.Model.Network, Domain.Model.Network>, IRepository<Domain.Model.Network>
    {
        public NetworkRepository(Context context)
            : base(context, new List<Profile>() { new NetworkMapperProfile() })
        { }

        public override async Task<long> AddNew(Domain.Model.Network model)
        {
            var entity = _mapper.Map<Database.Model.Network>(model);
            _context.Set<Database.Model.Network>().Add(entity);
            await _context.SaveChangesAsync();
            return entity.NetworkId;
        }

    }
}
