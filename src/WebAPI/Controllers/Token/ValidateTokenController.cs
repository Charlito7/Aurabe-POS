using Core.Application.Interface.Token;
using Infrastructure.Constants;
using Infrastructure.DotEnv;
using Infrastructure.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.Token
{
    [Route("user/token")]
    public class ValidateToken : AuthorizeBaseController
    {

        public ValidateToken(ITokenServices services) : base(services)
        {
        }


        [Authorize]
        [HttpPost]
        [Route("validate")]
        public async Task<IActionResult> ValidateTokenAsync()
        {
            return Ok();
        }
    }
}
