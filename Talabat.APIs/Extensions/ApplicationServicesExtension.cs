using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Repositories;
using Talabat.Core.Service;
using Talabat.Repository;
using Talabat.Service;

namespace Talabat.APIs.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddApplicationSerives(this IServiceCollection services)
        {
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            //services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
            services.AddScoped(typeof(IOrderService), typeof(OrderService));
            services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
            //builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));
            services.AddAutoMapper(typeof(MappingProfiles));

            #region Error Handling
            services.Configure<ApiBehaviorOptions>(Options =>
                {
                    Options.InvalidModelStateResponseFactory = (actionContext) =>
                    {
                        // ModelState => Dic [KeyValuePair]
                        // Key => Name of Param
                        // Value => Errors

                        var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                                                  .SelectMany(P => P.Value.Errors)
                                                  .Select(E => E.ErrorMessage)
                                                  .ToArray();

                        var ValidationErrorResponse = new ApiValidationErrorResponse()
                        {
                            Errors = errors
                        };

                        return new BadRequestObjectResult(ValidationErrorResponse);
                    };
                }); 
            #endregion

            return services;
        }
    }
}
