using Core.Application.Interface.Token;
using Infrastructure.Constants;
using Infrastructure.DotEnv;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace WebApi.Controllers.Base
{
    public class AuthorizeBaseController : Controller
    {
        private readonly ITokenServices _tokenServices;

        public AuthorizeBaseController(ITokenServices tokenServices)
        {
            _tokenServices = tokenServices;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(a => a.Value!.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .ToList();
                context.Result = new BadRequestObjectResult(errors);
            }

            var headers = context.HttpContext.Request.Headers;

            if (headers.ContainsKey("Authorization"))
            {
                bool isTokenValidated = false;
                var accessToken = headers[HeaderNames.Authorization].ToString().Replace("Bearer ", string.Empty);
                if (!string.IsNullOrEmpty(accessToken))
                {
                    isTokenValidated = _tokenServices
                    .ValidateTokenWithExpiryTime(
                    VariableBuilder.GetVariable(EnvFileConstants.ACCESS_TOKEN_SECRET),
                    VariableBuilder.GetVariable(EnvFileConstants.ISSUER), 
                    VariableBuilder.GetVariable(EnvFileConstants.AUDIENCE),
                    accessToken);
                }
                else
                {
                    context.Result = new UnauthorizedObjectResult("Invalid Token");
                }
                if (!isTokenValidated)
                {
                    context.Result = new UnauthorizedObjectResult("Invalid Token");
                }
            }
            
            base.OnActionExecuting(context);
        }


    }
}
