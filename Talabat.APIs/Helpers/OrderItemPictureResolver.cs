using AutoMapper;
using AutoMapper.Execution;
using Talabat.APIs.Dtos;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.APIs.Helpers
{
    public class OrderItemPictureResolver : IValueResolver<OrderItem, OrderItemDto, string>
    {
        private readonly IConfiguration _configuration;

        public OrderItemPictureResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(source.Product.PictureUrl)) // if find Photo
                return $"{_configuration["ApiBaseUrl"]}{source.Product.PictureUrl}";


            return string.Empty;
        }
    }
}
