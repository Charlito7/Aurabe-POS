using Core.Application.Interface;
using Core.Application.Model.Request;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAPI.Controllers;


[Route("category")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;

    public CategoryController(ICategoryService service)
    {
        _service = service;
    }

    [HttpPost("GetAll", Name ="GetAllCategory")]
    public async Task<ActionResult<IEnumerable<CreateCategoryRequest>>> GetProducts()
    {
        var category = await _service.GetCategoryAsync();
        return Ok(category);
    }

    [HttpPost("Create", Name = "CreateCategory")]
    public async Task<ActionResult<int>> CreateCategory(List<CreateCategoryRequest> categories)
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
        foreach (var category in categories) {
            var result = await _service.CreateCategoryAsync(category);
            
           
        }
        return Ok();
    }

}
