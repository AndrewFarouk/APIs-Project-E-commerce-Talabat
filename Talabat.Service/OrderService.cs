using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repositories;
using Talabat.Core.Service;
using Talabat.Core.Specifications.Order_Specifications;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basket;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(IBasketRepository basket,
                            IUnitOfWork unitOfWork,
                            IPaymentService paymentService)
        {
            _basket = basket;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string buyerEmail, string basketId, int DeliveryMethodId, Address shippingAddress)
        {
            // 1.Get Basket From Basket Repo
            var Basket = await _basket.GetBasketAsync(basketId);

            // 2.Get Selected Items at Basket From Product Repo
            var OrderItems = new List<OrderItem>();

            if(Basket?.Items.Count > 0)
            {
                foreach(var item in Basket.Items)
                {
                    var Product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    var ProductItemOrdered = new ProductItemOrder(item.Id, Product.Name, Product.PictureUrl);
                    var OrderItem = new OrderItem(ProductItemOrdered, Product.Price, item.Quantity);
                    OrderItems.Add(OrderItem);
                }
            }

            // 3.Calculate SubTotal => Price * Quantity
            var SubTotal = OrderItems.Sum(item => item.Price * item.Quantity);

            // 4.Get Delivery Method From DeliveryMethod Repo
            var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(DeliveryMethodId);

            // 5.Create Order
            var Spec = new OrderWithPaymentIntentSpec(Basket.PaymentIntentId);
            var ExOrder = await _unitOfWork.Repository<Order>().GetByEntityWithSpecAsync(Spec);
            if(ExOrder is not null)
            {
                _unitOfWork.Repository<Order>().Delete(ExOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            }

            var Order = new Order(buyerEmail, shippingAddress, DeliveryMethod, OrderItems, SubTotal, Basket.PaymentIntentId);

            // 6.Add Order Locally
            await _unitOfWork.Repository<Order>().AddAsync(Order);
           
            // 7.Save Order To Database[ToDo]
            var Result = await _unitOfWork.CompleteAsync();

            if (Result < 0) return null;

            return Order;
        }

        public async Task<Order> GetOrderByIdForSpecificUserAsync(string buyerEmail, int orderId)
        {
            var Spec = new OrderSpecifications(buyerEmail, orderId);
            var Order = await _unitOfWork.Repository<Order>().GetByEntityWithSpecAsync(Spec);
            return Order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string buyerEmail)
        {
            var Spec = new OrderSpecifications(buyerEmail);
            var Orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(Spec);
            return Orders;
        }
    }
}
