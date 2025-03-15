using Application.Interfaces.Repositories.User;
using Core.Application.Commons.ServiceResult;
using Core.Application.Interface.Token;
using Core.Application.Interfaces.Services.User;
using Core.Application.Model.Request;
using Core.Application.Model.Response;
using Infrastructure.Constants;
using Infrastructure.DotEnv;
using Infrastructure.Security;
using System.Net;

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

        public async Task<ServiceResult<UserSignInResponse>> 
            
            SingInAsync(UserLoginModel model)
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

            IList<string> userRoles = await _userManager.GetRolesAsync(user);
            var response = new UserSignInResponse
            {
                Token = _tokenServices
                .BuildToken(VariableBuilder.GetVariable(EnvFileConstants.ACCESS_TOKEN_SECRET),
                VariableBuilder.GetVariable(EnvFileConstants.ISSUER),
                VariableBuilder.GetVariable(EnvFileConstants.AUDIENCE), user, userRoles),

                RefreshToken = _tokenServices.GenerateRefreshToken(),
                UserRoles = userRoles,
                FullName = user.FirstName + " " + user.LastName
            };
            
            user.RefreshToken = response.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new ServiceResult<UserSignInResponse>(response);
        }
    }
}
