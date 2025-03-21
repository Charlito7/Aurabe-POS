using Core.Application.Commons.ServiceResult;
using Core.Application.Interfaces.Services.User;
using Core.Application.Model.Request;
using Core.Application.Model.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.User;

[ApiController]
[Route("/identity/user")]
public class SignInUserController : BaseController
{
    private readonly ISignInUser _service;

    public SignInUserController(ISignInUser service)
    {
        _service = service;
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("login", Name = "UserLogin")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    //[NonAction]
    public async Task<ActionResult<UserSignInResponse>> LoginUserAsync(UserLoginModel model)
    {
        if (!ModelState.IsValid)
        {
            // Extract error messages from ModelState
            var errorMessages = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            // Return BadRequest with error messages
            return BadRequest(new { Errors = errorMessages });
        }
        var result = await _service.SingInAsync(model);

        if (result.IsError)
        {
            return BadRequest(result);
        }

        var response = result.Result;

        Response.Cookies.Append("SessionId", response.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(60)
        });


        Response.Cookies.Append("RefreshToken", response.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });

        UserSignInResponse res = new UserSignInResponse()
        {
            FirstName = response.FirstName,
            LastName = response.LastName,
            Initial = response.Initial,
            UserRoles = response.UserRoles
        };
        
        return Ok(new ServiceResult<UserSignInResponse>(res));
    }

}
