using Core.Application.Interface;
using Core.Application.Interface.Services.Sales;
using Core.Application.Interface.Token;
using Core.Application.Model.Request;
using Core.Application.Model.Request.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApi.Controllers.Base;

namespace WebAPI.Controllers.Sales;

[Route("api/sales")]
[ApiController]
public class SalesController : AuthorizeBaseController
{
        private readonly ICreateSalesService _service;
    private readonly IGetSalesService _getSalesService;

    public SalesController(
        ICreateSalesService service, 
        IGetSalesService getSalesService,
        ITokenServices tokenServices) : base(tokenServices)
    {
        _service = service;
        _getSalesService = getSalesService;
    }

    [Authorize]
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
        if (!result.IsOk) 
        {
            return BadRequest(result);
        }
        return Ok(result);
       
    }

    [Authorize]
    [HttpPost]
    [Route("GetSalesListPagination", Name = "GetSalesListPagination")]
    public async Task<ActionResult<HttpStatusCode>> GetSalesListAsync(int page = 1, int pageSize = 10)
    {
        var sales = await _getSalesService.GetAllSalesMetadataServiceAsync(User, page, pageSize);
        return Ok(sales);

    }
}
