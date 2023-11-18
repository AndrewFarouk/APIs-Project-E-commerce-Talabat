using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Talabat.APIs.Dtos;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;
using Talabat.Repository;

namespace Talabat.APIs.Controllers
{
    public class ProductsController : APIBaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        // private readonly IGenericRepository<Product> _productRepo;
        private readonly IMapper _mapper;
       // private readonly IGenericRepository<ProductType> _typeRepo;
        //private readonly IGenericRepository<ProductBrand> _brandRepo;

        public ProductsController(//IGenericRepository<Product> ProductRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper
           // IGenericRepository<ProductType> TypeRepo,
            //IGenericRepository<ProductBrand> BrandRepo
            )
        {
            _unitOfWork = unitOfWork;
            //_productRepo = ProductRepo;
            _mapper = mapper;
           // _typeRepo = TypeRepo;
           // _brandRepo = BrandRepo;
        }

        // Get All Products
        // BaseURl/Api/Products -> GET
        [CachedAttribute(300)]
        [HttpGet]
       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery]ProductSpecParams Params)
        {
            var Spec = new ProductWithBrandAndTypeSpecifications(Params);
            var Products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(Spec);
            var MappedProducts = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(Products);
            ///var ReturnedObject = new Pagination<ProductToReturnDto>()
            ///{
            ///    PageSize = Params.PageSize,
            ///    PageIndex = Params.PageIndex,
            ///    Data = MappedProducts
            ///};
            ///return Ok(ReturnedObject);
            var CountSpec = new ProductWithFiltrationForCountSpecfication(Params);
            var Count = await _unitOfWork.Repository<Product>().GetCountWithSpecAsync(CountSpec);
            return Ok(new Pagination<ProductToReturnDto>(Params.PageSize, Params.PageIndex, Count, MappedProducts));
        }


        // Get Product By Id
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var Spec = new ProductWithBrandAndTypeSpecifications(id);
            var Product = await _unitOfWork.Repository<Product>().GetByEntityWithSpecAsync(Spec);

            if(Product is null)
                return NotFound(new ApiResponse(404));

            var MappedProducts = _mapper.Map<Product, ProductToReturnDto>(Product);
            return Ok(MappedProducts);
        }

        //BaseUrl/api/Products/Types
        [HttpGet("Types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            var Types = await _unitOfWork.Repository<ProductType>().GetAllAsync();
            return Ok(Types);
        }

        //BaseUrl/api/Products/Brands
        [HttpGet("Brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            var Brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(Brands);
        }
    }
}
