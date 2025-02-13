using Core.Application.Interface;
using Core.Application.Interface.Services.Sales;
using Core.Application.Model.Request;
using Core.Application.Model.Request.Sales;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers.Sales;

[Route("api/sales")]
[ApiController]
public class SalesController : ControllerBase
{
        private readonly ICreateSalesService _service;

    public SalesController(ICreateSalesService service)
    {
            _service = service;
    }

    [HttpPost]
    [Route("CreateSales", Name = "CreateSales")]
    public async Task<ActionResult<HttpStatusCode>> CreateSales(CreateSalesRequest sales)
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

        var result = await _service.CreateSalesAsync(User, sales);
        if (result.IsOk)
        {
            return Ok(result);
        }
        return BadRequest(result);
    }

}
