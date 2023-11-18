using Microsoft.Extensions.Configuration;
using Stripe;
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
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepository;

        public PaymentService(IConfiguration configuration,
                              IUnitOfWork unitOfWork,
                              IBasketRepository basketRepository)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _basketRepository = basketRepository;
        }
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string BasketId)
        {
            // Secret Key
            StripeConfiguration.ApiKey = _configuration["StripeSetting:Secretkey"];

            // Get basket
            var Basket = await _basketRepository.GetBasketAsync(BasketId);
            if (Basket is null) return null;

            //Create Payment Intent [Options = Total (SubToral + DM.Cost)]
            var ShippingPrice = 0M;
            if(Basket.DeliveryMethodId.HasValue)
            {
                var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(Basket.DeliveryMethodId.Value);
                ShippingPrice = DeliveryMethod.Cost;
            }

            //Check Price Of Items
            if(Basket.Items.Count > 0)
            {
                foreach(var item in Basket.Items)
                {
                    var Product = await _unitOfWork.Repository<Core.Entities.Product>().GetByIdAsync(item.Id);
                    if(item.Price != Product.Price)
                        item.Price = Product.Price;
                }
            }

            // Create Payment Intent
            var Service = new PaymentIntentService();
            PaymentIntent paymentIntent;

            if (string.IsNullOrEmpty(Basket.PaymentIntentId))
            {
                var Options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)Basket.Items.Sum(item => item.Price * item.Quantity * 100) + (long)ShippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };
                paymentIntent = await Service.CreateAsync(Options);
                Basket.PaymentIntentId = paymentIntent.Id;
                Basket.ClientSecret = paymentIntent.ClientSecret;
            }

            else // Update
            {
                var Options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)Basket.Items.Sum(item => item.Price * item.Quantity * 100) + (long)ShippingPrice * 100
                };
                paymentIntent = await Service.UpdateAsync(Basket.PaymentIntentId, Options);
                Basket.PaymentIntentId = paymentIntent.Id;
                Basket.ClientSecret = paymentIntent.ClientSecret;
            }

            await _basketRepository.UpdateBasketAsync(Basket);
            return Basket;
        }

        public async Task<Order> UpdatePaymentIntentToSucceedOrFailed(string PaymentIntentId, bool flag)
        {
            var Spec = new OrderWithPaymentIntentSpec(PaymentIntentId);
            var Order = await _unitOfWork.Repository<Order>().GetByEntityWithSpecAsync(Spec);
            if (flag)
                Order.Status = OrderStatus.PaymentReceived;
            else
                Order.Status = OrderStatus.PaymentFailed;

            _unitOfWork.Repository<Order>().Update(Order);
            await _unitOfWork.CompleteAsync();
            return Order;
        }
    }
}
