using Core.Application.Interfaces.Services.User;
using Core.Application.Model.Request;
using Core.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Controllers.Base;

namespace WebApi.Controllers.User
{
    //[ApiController]
    [NonController]
    [Route("identity/admin/password")]
    public class UpdateUserPasswordController : BaseController
    {
        private readonly IUpdateUserPassword _service;
        private readonly IDoesUserBelongToRole _roleService;

        public UpdateUserPasswordController(IUpdateUserPassword service, IDoesUserBelongToRole roleService)
        {
            _service = service;
            _roleService = roleService;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("update")]
        public async Task<IActionResult> UpdateUserPasswordAsync([FromBody] UpdateUserPasswordModel model)
        {
            var userEmail = this.User.FindFirst(ClaimTypes.Email)?.Value;
            if (userEmail == null || string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var isAdminResult = await _roleService.DoesUserBelongToRoleAsync(UserRoleEnums.Admin.ToString(), userEmail);

            if (isAdminResult.Result != true)
            {
                return Unauthorized();
            }
            var result = await _service.UpdateUserPassowrdAsync(model);

            if (result.IsError)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
