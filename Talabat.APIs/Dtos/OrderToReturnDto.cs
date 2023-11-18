using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.APIs.Dtos
{
    public class OrderToReturnDto
    {
        public int Id { get; set; }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public string Status { get; set; }
        public Address ShippingAddress { get; set; } 
        public string DeliveryMethod { get; set; } // name 
        public decimal DeliveryMethodCost { get; set; } // cost 
        public ICollection<OrderItemDto> Items { get; set; } = new HashSet<OrderItemDto>();
        public decimal SubTotal { get; set; } // Price of product * Quantity
        public decimal Total { get; set; }
        public string PaymentIntentId { get; set; }
    }
}
