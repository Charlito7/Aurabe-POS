﻿using Application.Interfaces.Repositories.User;
using Core.Application.Interface;
using Core.Application.Interface.Token;
using Infrastructure.Repositories.User;
using Infrastructure.Repository;
using Infrastructure.Security;
using Infrastructure.Services;
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


            //Security
            services.AddScoped<IHashingServices, HashingServices>();



            return services;
        }
    }
}
