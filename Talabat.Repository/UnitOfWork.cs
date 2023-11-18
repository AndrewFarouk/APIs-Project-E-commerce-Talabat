using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Repository.DataContext;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private Hashtable _repositories;
        private readonly StoreDbContext _dbContext;

        public UnitOfWork(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new Hashtable();
        }

        public async Task<int> CompleteAsync()
            => await _dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
            => await _dbContext.DisposeAsync();

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity).Name; // Product, ProductBrand ...... 
            if (!_repositories.ContainsKey(type))
            {
                // Create
                var Repository = new GenericRepository<TEntity>(_dbContext);
                _repositories.Add(type, Repository);
            }

            return _repositories[type] as IGenericRepository<TEntity>; //Emplicitly Casting
        }
    }
}
