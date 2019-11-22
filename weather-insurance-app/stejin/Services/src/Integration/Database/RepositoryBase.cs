using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.EntityFrameworkCore;

namespace WeatherInsurance.Integration.Database
{
    public abstract class RepositoryBase<TData, TDomain>
         where TData : class
         where TDomain : class
    {

        protected readonly Context _context;
        protected readonly IMapper _mapper;


        protected RepositoryBase(Context context, Profile mapperProfile)
        {
            _context = context;
            var config = new MapperConfiguration(cfg => cfg.AddProfile(mapperProfile));
            _mapper = config.CreateMapper();
        }

        protected RepositoryBase(Context context, ICollection<Profile> mapperProfiles)
        {
            _context = context;
            var mappings = new MapperConfigurationExpression();
            foreach (var profile in mapperProfiles)
            {
                mappings.AddProfile(profile);
            }
            var config = new MapperConfiguration(mappings);
            _mapper = config.CreateMapper();
        }

        public virtual async Task<TDomain> Get(params object[] keys)
        {
            var model = await _context.Set<TData>().FindAsync(keys);
            return _mapper.Map<TDomain>(model);
        }

        public virtual async Task<IEnumerable<TDomain>> Get()
        {
            var models = await _context.Set<TData>().ToListAsync();
            return _mapper.Map<IEnumerable<TDomain>>(models);
        }

        public virtual async Task<IEnumerable<TDomain>> Find(Expression<Func<TData, bool>> query)
        {
            var models = await _context.Set<TData>().Where(query).ToListAsync();
            return _mapper.Map<IEnumerable<TDomain>>(models);
        }

        public virtual async Task<IEnumerable<TDomain>> Find(Expression<Func<TDomain, bool>> query)
        {
            return await Find(_mapper.MapExpression<Expression<Func<TData, bool>>>(query));
            //var models = _context.Set<TData>();
            //return _mapper.ProjectTo<TDomain>(models).Where(query);
            //return _context.Set<TData>().UseAsDataSource(_mapper).For<TDomain>().Where(query);
        }

        public virtual async Task<int> Update(TDomain model)
        {
            _context.ChangeTracker.Entries().ToList().ForEach(e => _context.Entry(e.Entity).State = EntityState.Detached);
            var entity = _mapper.Map<TData>(model);
            _context.Set<TData>().Attach(entity);
            _context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            return await _context.SaveChangesAsync();
        }

        public virtual async Task<long> AddNew(TDomain model)
        {
            var entity = _mapper.Map<TData>(model);
            await _context.Set<TData>().AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public virtual async Task<int> Delete(params object[] keys)
        {
            var entity = _context.Set<TData>().Find(keys);
            _context.Set<TData>().Remove(entity);
            return await _context.SaveChangesAsync();
        }
    }
}
