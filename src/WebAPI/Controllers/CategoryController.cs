using Core.Application.Interface;
using Core.Application.Model.Request;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;


[Route("api/category")]
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
    public async Task<ActionResult<int>> CreateCategory(CreateCategoryRequest category)
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
        var result = await _service.CreateCategoryAsync(category);

        if (!result.IsOk)
        {
            return BadRequest(new { Errors = result.ErrorMessages });
        }

        return Ok(result.Status);
    }

}
