using Core.Application.Interfaces.Services.User;
using Core.Application.Model.Request;
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
    [Route("login", Name ="UserLogin")]
    //[ApiExplorerSettings(IgnoreApi = true)]
    //[NonAction]
    public async Task<IActionResult> LoginUserAsync([FromForm]UserLoginModel model)
    {
        var result = await _service.SingInAsync(model);
        if(result.IsError)
        {
            return BadRequest(result);
        }
     
        return Ok(result);
    }
}
