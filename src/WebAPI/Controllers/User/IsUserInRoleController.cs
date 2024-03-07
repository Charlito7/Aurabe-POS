using Application.Interfaces.Repositories.User;
using Core.Application.Interface.Token;
using Core.Application.Interfaces.Services.User;
using Infrastructure.Token;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.User
{
    [ApiController]
    [Route("user/role")]
    public class IsUserInRoleController : AuthorizeBaseController
    {
        private readonly IDoesUserBelongToRole _service;
        private readonly IUserManager _userManager;

        public IsUserInRoleController(IDoesUserBelongToRole service, IUserManager userManager, ITokenServices tokenServices)
            : base(tokenServices)
        {
            _service = service;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        [Route("check")]
        public async Task<IActionResult> IsUserInRoleAsync(string role)
        {
            
            var userEmail = this.User.FindFirst(ClaimTypes.Email)?.Value;
            if (userEmail == null || string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return BadRequest();

            var result = await _service.DoesUserBelongToRoleAsync(role, userEmail);
            return Ok(result);
        }
    }
}
