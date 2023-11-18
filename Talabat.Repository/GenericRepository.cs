using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;
using Talabat.Repository.DataContext;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreDbContext _context;

        public GenericRepository(StoreDbContext context)
        {
            _context = context;
        }
        #region Without Specifications
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Product))
                return (IReadOnlyList<T>)await _context.Products.Include(P => P.ProductBrand).Include(P => P.ProductType).ToListAsync();
            else
                return await _context.Set<T>().ToListAsync();
        }


        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
            //return await _context.Set<T>().Where(P => P.id == id).Include(P => P.ProductBrand).Include(P => P.ProductType) ;
        }
        #endregion

        #region With Specifications
        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> Spec)
        {
            //return await SpecificationsEvalutor<T>.GetQuery(_context.Set<T>(), Spec).ToListAsync();
            return await ApplySpecification(Spec).ToListAsync();
        }

        public async Task<T> GetByEntityWithSpecAsync(ISpecifications<T> Spec)
        {
            //return await SpecificationsEvalutor<T>.GetQuery(_context.Set<T>(), Spec).FirstOrDefaultAsync();
            return await ApplySpecification(Spec).FirstOrDefaultAsync();
        }
        public async Task<int> GetCountWithSpecAsync(ISpecifications<T> Spec)
        {
            return await ApplySpecification(Spec).CountAsync();
        }
        
        private IQueryable<T> ApplySpecification(ISpecifications<T> Spec)
        {
            return SpecificationsEvalutor<T>.GetQuery(_context.Set<T>(), Spec);
        }


        #endregion

        public async Task AddAsync(T Item)
            => await _context.Set<T>().AddAsync(Item);

        public void Delete(T Item)
            => _context.Set<T>().Remove(Item);

        public void Update(T Item)
            => _context.Set<T>().Update(Item);
    }
}
