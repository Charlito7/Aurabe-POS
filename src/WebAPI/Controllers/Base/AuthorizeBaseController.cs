
using Core.Application.Interface.Token;
using Infrastructure.Constants;
using Infrastructure.DotEnv;
using Microsoft.AspNetCore.Mvc;
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

            // Read the token from the cookie
            var accessToken = context.HttpContext.Request.Cookies["SessionId"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                bool isTokenValidated = _tokenServices
                    .ValidateTokenWithExpiryTime(
                        VariableBuilder.GetVariable(EnvFileConstants.ACCESS_TOKEN_SECRET),
                        VariableBuilder.GetVariable(EnvFileConstants.ISSUER),
                        VariableBuilder.GetVariable(EnvFileConstants.AUDIENCE),
                        accessToken);

                if (!isTokenValidated)
                {
                    context.Result = new UnauthorizedObjectResult("Invalid Token");
                }
            }
            else
            {
                context.Result = new UnauthorizedObjectResult("Token not found");
            }

            base.OnActionExecuting(context);
        }


    }
}
