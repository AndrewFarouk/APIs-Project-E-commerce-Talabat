using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories;
using Talabat.Repository;
using Talabat.Repository.DataContext;
using Talabat.Repository.IdentityContext;

namespace Talabat.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            #region  Configure Services Add services to the container
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<StoreDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddSingleton<IConnectionMultiplexer>(Options =>
            {
                var Connection = builder.Configuration.GetConnectionString("RedisConnection");
                return ConnectionMultiplexer.Connect(Connection);
            });

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });

            builder.Services.AddApplicationSerives();

            builder.Services.AddIdentityServices(builder.Configuration);
            
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", Options =>
                {
                    Options.AllowAnyMethod();
                    Options.AllowAnyHeader();
                    Options.WithOrigins(builder.Configuration["FrontBaseUrl"]);
                });
            });
            #endregion

            var app = builder.Build();

            #region Update-Database
            //Group Of Services LifeTime Scoped
            using var Scope = app.Services.CreateScope();
            // Service Its Self
            var Services = Scope.ServiceProvider;
           
            var LoggerFactory = Services.GetRequiredService<ILoggerFactory>(); 
            try
            {
                // ASK CLR For Creating Object From DbContext Explicitly
                var Context = Services.GetRequiredService<StoreDbContext>();
                await Context.Database.MigrateAsync(); // Update-Database

                await StoreContextSeed.SeedAsync(Context, LoggerFactory);

                // ASK CLR For Creating Object From DbContext Explicitly
                var IdentityDbContext = Services.GetRequiredService<AppIdentityDbContext>();
                await IdentityDbContext.Database.MigrateAsync(); // Update-Database
                
                var  UserManager = Services.GetRequiredService<UserManager<AppUser>>();
                await AppIdentityDbContextSeed.SeedUserAsync(UserManager);
            }
            catch (Exception ex)
            {
                var Logger = LoggerFactory.CreateLogger<Program>();
                Logger.LogError(ex, "An Error Occured During Apply The Migration");
            }
            #endregion

            #region Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMiddleware<ExceptionMiddleWare>();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStatusCodePagesWithReExecute("/errors/{0}"); // prefer ReExecute on Redirects because send one request

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("MyPolicy");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers(); 
            #endregion

            app.Run();
        }
    }
}