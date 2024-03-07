using Application.Interfaces.Repositories.User;
using Core.Application.Commons.ServiceResult;
using Core.Application.Interface.Token;
using Core.Application.Interfaces.Services.User;
using Core.Application.Model.Request;
using Core.Application.Model.Response;
using Infrastructure.Constants;
using Infrastructure.DotEnv;
using Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.User
{
    public class SignInUser : ISignInUser
    {
        private readonly ISignInManager _signInManager;
        private readonly IUserManager _userManager;
        private readonly ITokenServices _tokenServices;

        public SignInUser(ISignInManager signInManager, IUserManager userManager,
            ITokenServices tokenServices)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenServices = tokenServices;
        }

        public async Task<ServiceResult<UserSignInResponse>> SingInAsync(UserLoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.UserName!);
            if (user == null )
            {
                return new ServiceResult<UserSignInResponse>(HttpStatusCode.BadRequest);
            }


            var result = MyPasswordHasher.VerifyHashedPassword(user,user.Password, model.Password);
            /*SignInResult result = await _signInManager
                .PasswordSignInAsync(model.UserName!, model.Password!,
                isPersistent: false, lockoutOnFailure: false);*/

            if (!result)
            {
                return new ServiceResult<UserSignInResponse>(HttpStatusCode.Unauthorized);
            }


            var response = new UserSignInResponse
            {
                Token = _tokenServices
                .BuildToken(VariableBuilder.GetVariable(EnvFileConstants.ACCESS_TOKEN_SECRET),
                VariableBuilder.GetVariable(EnvFileConstants.ISSUER),
                VariableBuilder.GetVariable(EnvFileConstants.AUDIENCE), user),

                RefreshToken = _tokenServices.GenerateRefreshToken()
            };
            
            user.RefreshToken = response.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new ServiceResult<UserSignInResponse>(response);
        }
    }
}
