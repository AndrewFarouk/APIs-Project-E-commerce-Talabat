using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
    public static class SpecificationsEvalutor<T> where T : BaseEntity
    {
        // Function To Build Query
        // _context.Set<T>().Where(P => P.id == id).Include(P => P.ProductBrand).Include(P => P.ProductType) ;

        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> Spec)
        {
            var Query = inputQuery; // _context.Set<T>()

            if (Spec.Criteria is not null)  // P => P.id == id
            {
                Query = Query.Where(Spec.Criteria);  // _context.Set<T>().Where(P => P.id == id)
            }
            //(OrderBy. OrderByDesc)
            //_context.Products.OrderBy(P => P.Name).Include(P => P.ProductBrand).Include(P => P.ProductType)
            if(Spec.OrderBy is not null)
                Query = Query.OrderBy(Spec.OrderBy);
            
            if(Spec.OrderByDescending is not null)
                Query = Query.OrderByDescending(Spec.OrderByDescending);

            // Pagination
            if(Spec.IsPaginationEnabled)
                Query = Query.Skip(Spec.Skip).Take(Spec.Take);

            // P => P.ProductBrand , P => P.ProductType
            Query = Spec.Includes.Aggregate(Query, (CurrentQuery, IncludeExpression) => CurrentQuery.Include(IncludeExpression));

            // _context.Set<T>().Where(P => P.id == id).Include(P => P.ProductBrand)
            // _context.Set<T>().Where(P => P.id == id).Include(P => P.ProductBrand).Include(P => P.ProductType) ;


            return Query;
        } 
    }
}
