using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public interface ISpecifications<T> where T : BaseEntity
    {
        //_context.Products.Where(P => P.id == id).Include(P => P.ProductBrand).Include(P => P.ProductType).ToListAsync();

        // Sign For Property For Where Condition [Where(P => P.id == id)]
        public Expression<Func<T, bool>> Criteria { get; set; } // Signituer of Property

        // Sign For Property for List of Include [Include(P => P.ProductBrand).Include(P => P.ProductType)]
        public List<Expression<Func<T, object>>> Includes { get; set; } // Signituer of Property
    
       // OrderBy
       public Expression<Func<T, object>> OrderBy { get; set; }

       // OrderByDesc
       public Expression<Func<T, object>> OrderByDescending { get; set; }
        
        // Skip
        public int Skip { get; set; }
        // Take
        public int Take { get; set; }  
        public bool IsPaginationEnabled { get; set; }
    }
}
