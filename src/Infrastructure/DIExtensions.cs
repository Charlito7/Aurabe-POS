using Application.Interfaces.Repositories.User;
using Core.Application.Interface;
using Core.Application.Interface.Repository;
using Core.Application.Interface.Repository.Sales;
using Core.Application.Interface.Services.Sales;
using Core.Application.Interface.Token;
using Infrastructure.Repositories.User;
using Infrastructure.Repository;
using Infrastructure.Repository.Product;
using Infrastructure.Repository.Sales;
using Infrastructure.Security;
using Infrastructure.Services;
using Infrastructure.Services.Products;
using Infrastructure.Services.Sales;
using Infrastructure.Token;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DIExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            MainServicesRegistrations.RegisterMainServices(services);

            //Repositories
            services.AddScoped<IUserManager, UserManagerWrapper>();
            services.AddScoped<ISignInManager, SignInManagerWrapper>();
            services.AddScoped<IRoleManager, RoleManagerWrapper>();
            
            //DbContext



            //Token
            services.AddScoped<ITokenServices, TokenServices>();

            //Email
            //services.AddScoped<IEmailSender, EmailSender>();


            // Register Repository and Service using interfaces
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductUpdateService, ProductUpdateService>();
            services.AddScoped<ICreateSalesService, CreateSalesService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISalesRepository, SalesRepository>();
            services.AddScoped<IGetSalesService, GetSalesServices>();
            services.AddScoped<IGetSellerDailySalesResumeService, GetSellerDailySalesResumeService>();
            


            //Security
            services.AddScoped<IHashingServices, HashingServices>();



            return services;
        }
    }
}
