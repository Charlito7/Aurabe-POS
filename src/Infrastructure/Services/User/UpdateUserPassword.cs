using Application.Interfaces.Repositories.User;
using Core.Application.Commons.ServiceResult;
using Core.Application.Interfaces.Services.User;
using Core.Application.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.User
{
    public class UpdateUserPassword : IUpdateUserPassword
    {
        private readonly IUserManager _userManager;

        public UpdateUserPassword(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<ServiceResult<bool>> UpdateUserPassowrdAsync(UpdateUserPasswordModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email!);
            
            if(user == null)
            {
                return new ServiceResult<bool>(System.Net.HttpStatusCode.NotFound);
            }

            var result = await _userManager.UpdatePasswordWithoutToken(user, model.NewPassword!);

            if (!result.Succeeded)
            {
                return new ServiceResult<bool>(System.Net.HttpStatusCode.ServiceUnavailable);
            }

            return new ServiceResult<bool>(true);
        }
    }
}
