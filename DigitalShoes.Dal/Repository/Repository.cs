using DigitalShoes.Dal.Context;
using DigitalShoes.Dal.Repository.Interfaces;
using DigitalShoes.Domain;
using DigitalShoes.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;


namespace DigitalShoes.Dal.Repository
{
    public class Repository<T>:IRepository<T> where T : BaseEntity
    {
        private readonly ApplicationDbContext _dbContext;
        protected readonly DbSet<T> _table;
        public Repository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _table = _dbContext.Set<T>();
        }
        public async Task CreateAsync(T entity)
        {            
            await _table.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<T>? GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true, string? includeProperties = null)
        {
            IQueryable<T>? query = _table;
            if (filter is not null)
            {
                query = query.Where(filter);
            }
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (includeProperties!=null)
            {
                foreach (var property in includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null, int pageSize = 0, int pageNumber = 1)
        {
            IQueryable<T> query = _table;
            if (filter is not null)
            {
                query = query.Where(filter);
            }

            //pagination
            //with condition below if pageSize is provided it filters as given value, else it retrieves all the records
            if (pageSize>0)
            {
                if (pageSize>100)
                {
                    // pageSize defines how many records should be retrieved at a time
                    pageSize = 100;
                }

                // this is basic formula, it skips all records except last page's records that is why we write -1 , and takes records as same inside pageSize quantity
                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }

            if (includeProperties != null)
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }

            //query is executed in line below, ToListAsync() method executes the query, this is deferred execution
            return await query.ToListAsync();
        }

        public async Task RemoveAsync(T entity)
        {
            _table.Remove(entity);
            await SaveAsync();
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
        
        public async Task UpdateAsync(T _object)
        {
            if (_object is null) throw new InvalidOperationException("there is not such instance");
            _object.DataStatus = StaticDetails.DataStatus.Updated;
            _object.ModifiedDate = DateTime.Now;
            _table.Update(_object);
            await SaveAsync();
        }

        public int Count()
        {
            return _table.Count();
        }

        public bool Any(Expression<Func<T, bool>> expression)
        {
            return _table.Any(expression);
        }

        public List<T> GetActives()
        {
            return _table.Where(t => t.DataStatus != StaticDetails.DataStatus.Deleted).ToList();
        }

        public async Task HardDeleteAsync(int id)
        {
            T _object = await GetAsync(x=>x.Id==id);
            _table.Remove(_object);
            await SaveAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            T _object = await GetAsync(x => x.Id == id);
            _object.DataStatus = StaticDetails.DataStatus.Deleted;
            _object.ModifiedDate = DateTime.Now;
            _table.Update(_object);
            await SaveAsync();
        }

        public T Default(Expression<Func<T, bool>> exp)
        {
            return _table.FirstOrDefault(exp);
        }
    }
}
