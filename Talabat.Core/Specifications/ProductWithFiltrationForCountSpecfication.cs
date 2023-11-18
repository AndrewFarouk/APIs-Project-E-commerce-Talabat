using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class ProductWithFiltrationForCountSpecfication : BaseSpecifications<Product>
    {
        public ProductWithFiltrationForCountSpecfication(ProductSpecParams Params) : base(P =>
            (string.IsNullOrEmpty(Params.Search) || P.Name.Trim().ToLower().Contains(Params.Search))
            &&
            (!Params.TypeId.HasValue || P.ProductTypeId == Params.TypeId)
            &&
            (!Params.BrandId.HasValue || P.ProductBrandId == Params.BrandId))
        {
            
        }
    }
}
