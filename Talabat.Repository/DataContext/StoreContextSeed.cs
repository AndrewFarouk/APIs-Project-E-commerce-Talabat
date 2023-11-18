using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Repository.DataContext
{
    public class StoreContextSeed
    {
        //Seeding
        public static async Task SeedAsync(StoreDbContext context, ILoggerFactory loggerFactory)
        {
			try
			{
				if(context.ProductBrands != null && !context.ProductBrands.Any())
				{
					var BrandsData = File.ReadAllText("../Talabat.Repository/DataContext/SeedData/brands.json");
					var Brands = JsonSerializer.Deserialize<List<ProductBrand>>(BrandsData);
					
                    if(Brands?.Count > 0)
                    {
					    foreach (var Brand in Brands)
						    await context.Set<ProductBrand>().AddAsync(Brand);

					    await context.SaveChangesAsync();
                    }
				}
                if (context.ProductTypes != null && !context.ProductTypes.Any())
                {
                    var TypesData = File.ReadAllText("../Talabat.Repository/DataContext/SeedData/types.json");
                    var Types = JsonSerializer.Deserialize<List<ProductType>>(TypesData);

                    if(Types?.Count > 0)
                    {
                        foreach (var Type in Types)
                            await context.ProductTypes.AddAsync(Type);

                        await context.SaveChangesAsync();
                    }
                }
                if (context.Products != null && !context.Products.Any())
                {
                    var ProductsData = File.ReadAllText("../Talabat.Repository/DataContext/SeedData/products.json");
                    var Products = JsonSerializer.Deserialize<List<Product>>(ProductsData);

                    if(Products?.Count > 0)
                    { 
                        foreach (var Product in Products)
                            await context.Products.AddAsync(Product);

                        await context.SaveChangesAsync();
                    }
                }
                if (context.DeliveryMethods != null && !context.DeliveryMethods.Any())
                {
                    var DeliveryMethodsData = File.ReadAllText("../Talabat.Repository/DataContext/SeedData/delivery.json");
                    var DeliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodsData);

                    if (DeliveryMethods?.Count > 0)
                    {
                        foreach (var DeliveryMethod in DeliveryMethods)
                            await context.DeliveryMethods.AddAsync(DeliveryMethod);

                        await context.SaveChangesAsync();
                    }
                }
            }
			catch (Exception ex)
			{
                var logger = loggerFactory.CreateLogger<StoreContextSeed>();
                logger.LogError(ex.Message);
			}
        }
    }
}
