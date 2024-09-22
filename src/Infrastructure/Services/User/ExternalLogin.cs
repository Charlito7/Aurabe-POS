using Application.Interfaces.Repositories.User;
using Core.Application.Commons.ServiceResult;
using Core.Application.Interface.Token;
using Core.Application.Interfaces.Services.User;
using Core.Application.Model.Request;
using Core.Application.Model.Response;
using Infrastructure.Constants;
using Infrastructure.DotEnv;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Security.Claims;

namespace Infrastructure.Services.User
{
    public class MyExternalLoginService : IMyExternalLoginService
    {
        private readonly ISignInManager _signInManager;
        private readonly IUserManager _userManager;
        private readonly ICreateUserService _createUserService;
        private readonly ITokenServices _tokenServices;

        public MyExternalLoginService(ISignInManager signInManager, IUserManager userManager, ICreateUserService createUserService,
            ITokenServices tokenServices)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _createUserService = createUserService;
            _tokenServices = tokenServices;
        }

        //public Task<ServiceResult<ExternalLoginResponse>> ExternalLoginAsync(ExternalLoginInfo info)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<ServiceResult<ExternalLoginResponse>> ExternalLoginAsync(ExternalLoginInfo info)
        {
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                return new ServiceResult<ExternalLoginResponse>(System.Net.HttpStatusCode.BadRequest);
            }

            var domain = email.Split('@')[1].ToString();

            if (domain.ToLower() != "gmail.com")
            {
                return new ServiceResult<ExternalLoginResponse>(HttpStatusCode.Forbidden);
            }

            var user = await _userManager.FindByEmailAsync(email!);
            var response = new ExternalLoginResponse();

            if (user != null)
            {
                IdentityResult result = await _userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, true);
                }
            }

            else
            {
                var userToBeCreated = new CreateUserModel
                {
                    FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? info.Principal.FindFirstValue(ClaimTypes.Name)! ?? string.Empty,
                    LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty,
                    Email = email
                };

                var isLoggedIn = await _createUserService.CreateUserAsync(userToBeCreated, info);

                if (isLoggedIn.IsError)
                {
                    return new ServiceResult<ExternalLoginResponse>(System.Net.HttpStatusCode.InternalServerError);
                }

                user = await _userManager.FindByEmailAsync(email!);
            }


            var token = _tokenServices.BuildToken(VariableBuilder.GetVariable(EnvFileConstants.ACCESS_TOKEN_SECRET),
                VariableBuilder.GetVariable(EnvFileConstants.ISSUER), VariableBuilder.GetVariable(EnvFileConstants.AUDIENCE),
                user, info.Principal.Claims.ToArray());

            if (string.IsNullOrWhiteSpace(token))
            {
                return new ServiceResult<ExternalLoginResponse>(System.Net.HttpStatusCode.InternalServerError);
            }

            var refreshToken = _tokenServices.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            var myResponse = new ExternalLoginResponse
            {
                Token = token,
                RefreshToken = refreshToken
            };

            return new ServiceResult<ExternalLoginResponse>(myResponse);
        }
    }
}
