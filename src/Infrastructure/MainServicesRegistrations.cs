using Core.Application.Interfaces.Services.User;
using Infrastructure.Services.User;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class MainServicesRegistrations
    {
        public static IServiceCollection RegisterMainServices(this IServiceCollection services)
        {

            //User
            services.AddScoped<ICreateUserService, CreateUserService>();
            services.AddScoped<ISignInUserService, SignInUserService>();
            services.AddScoped<IAdminAssignRoles, AdminAssignRoles>();    
            services.AddScoped<IAdminRemoveRoles, AdminRemoveRoles>();
            services.AddScoped<IDoesUserBelongToRole, DoesUserBelongToRole>();  
            services.AddScoped<ISignInUser, SignInUser>();
            services.AddScoped<IUpdateUserPassword, UpdateUserPassword>();
            services.AddScoped<IGetUsersProfileData, GetUserProfileData>();

            // Consumables


            //AWS/S3


            //Issue Items


            //EndPoints
 
            

            return services;
        }
    }
}
