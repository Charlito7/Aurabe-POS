﻿using Core.Application.Commons.ServiceResult;
using Core.Application.Interface;
using Core.Application.Model.Request;
using Core.Domain.Entity;
using System.Net;

namespace Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly IRepository<CategoryEntity> _repository;

    public CategoryService(IRepository<CategoryEntity> repository)
    {

        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    public async Task<ServiceResult<HttpStatusCode>> CreateCategoryAsync(CreateCategoryRequest request)
    {
        //Logic
        CategoryEntity category = new CategoryEntity()
        {
            Name = request.Name,
            Description = request.Description
        };
        var result = await _repository.CreateAsync(category);

        if (!result)
        {
            return new ServiceResult<HttpStatusCode>(HttpStatusCode.BadRequest, true, HttpStatusCode.BadRequest, "Category not save");
        }

        return new ServiceResult<HttpStatusCode>(HttpStatusCode.OK, true, HttpStatusCode.OK, "Category is added with success");
    }

    public Task<ServiceResult<HttpStatusCode>> DeleteCategoryAsync(int productId)
    {
        throw new NotImplementedException();
    }


    public Task<ServiceResult<CreateCategoryRequest>> GetCategoryByIdAsync(int productId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResult<GetCategoryRequest>> GetCategoryByNameAsync(string name)
    {
        var result = await _repository.FindListAsync(category => category.Name.Contains(name));

        if (result.Count > 0)
        {
            // Retrieve the first category from the result
            var firstCategory = result[0];

            // convert the entity in category model
#pragma warning disable CS8629 // Nullable value type may be null.
            GetCategoryRequest categoryModel = new GetCategoryRequest()
            {
                Description = firstCategory.Description,
                Name = firstCategory.Name,
                CategoryId = (Guid)firstCategory.Id
            };
#pragma warning restore CS8629 // Nullable value type may be null.

            // Now, you can use categoryId as needed
            // For example, you might want to return it as part of your ServiceResult
            return new ServiceResult<GetCategoryRequest>(categoryModel, true, HttpStatusCode.OK, "Success");
        }
        else
        {
            // Handle the case where no category was found with the specified name
            // You might want to return an error message or handle it in a way that fits your application's logic
            return new ServiceResult<GetCategoryRequest>(new GetCategoryRequest(), false, HttpStatusCode.BadRequest, "Failure");
        }

    }

    public Task<ServiceResult<HttpStatusCode>> UpdateCategoryAsync(int productId, CreateCategoryRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResult<IEnumerable<CreateCategoryRequest>>> GetCategoryAsync()
    {
        var result = await _repository.GetAllAsync();

        IEnumerable<CreateCategoryRequest> category = result
    .Select(categoryEntity => new CreateCategoryRequest
    {
        Name = categoryEntity.Name,
        Description = categoryEntity.Description
        // Add other properties as needed
    });

        return new ServiceResult<IEnumerable<CreateCategoryRequest>>(category, true, HttpStatusCode.OK, "Category is added with success");
    }
}
