using Core.Application.Commons.ServiceResult;
using Core.Application.Interface;
using Core.Application.Model.Request;
using Core.Domain.Entity;
using System.Net;

namespace Infrastructure.Services;

public class ProductService : IProductService
{

    private readonly IRepository<ProductEntity> _repository;
    private readonly ICategoryService _categoryService;

    public ProductService(IRepository<ProductEntity> repository, ICategoryService categoryService)
    {

        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _categoryService = categoryService;
    }



    public async Task<ServiceResult<IEnumerable<ProductRequest>>> CreateProductsAsync(List<ProductRequest> requests)
    {
        var results = new List<ProductRequest>();

        foreach (var request in requests)
        {
            var categoryResult = await _categoryService.GetCategoryByNameAsync(request.CategoryName);

            if (categoryResult == null)
            {   
                results.Add(request);
                continue;
            }

            ProductEntity productEntity = new ProductEntity()
            {
                Name = request.Name,
                Description = request.Description,
                BarCode = request.BarCode,
                CategoryId = categoryResult.Result.CategoryId,
                ExpiredDate = request.ExpiredDate,
                Quantity = request.Quantity,
                Price = request.Price,
                MinimumReorderQuantity = request.MinimumReorderQuantity,
            };

            var result = await _repository.CreateAsync(productEntity);

            if (!result)
            {
                results.Add(request);
            }
        }

        // Check if all products were added successfully
        if (results.Count > 0)
        {
            return new ServiceResult<IEnumerable<ProductRequest>>(results, true, HttpStatusCode.OK, "All products added with success.");
        }
        else
        {
            return new ServiceResult<IEnumerable<ProductRequest>>(results, false, HttpStatusCode.BadRequest, "Some products were not saved successfully.");
        }
    }


    public Task<ServiceResult<HttpStatusCode>> DeleteProductAsync(Guid productId)
    {
        throw new NotImplementedException();
    }

    public Task<CreateProductRequest> GetProductByIdAsync(Guid productId)
    {
        throw new NotImplementedException();
    }

    public async Task<ServiceResult<ProductRequest>> GetProductByNameAsync(string name)
    {
        var result = await _repository.FindAsync(product => product.Name.Contains(name));

        if (result.Count > 0)
        {
            // Retrieve the first category from the result
            var firstCategory = result[0];

            // convert the entity in category model
            ProductRequest productModel = new ProductRequest()
            {
                Description = firstCategory.Description,
                Name = firstCategory.Name,
            };

            // Now, you can use categoryId as needed
            // For example, you might want to return it as part of your ServiceResult
            return new ServiceResult<ProductRequest>(productModel, true, HttpStatusCode.OK, "Success");
        }
        else
        {
            // Handle the case where no category was found with the specified name
            // You might want to return an error message or handle it in a way that fits your application's logic
            return new ServiceResult<ProductRequest>(new ProductRequest(), false, HttpStatusCode.BadRequest, "Failure");
        }
    }

    public async Task<ServiceResult<IEnumerable<ProductRequest>>> GetProductsAsync()
    {
        var result = await _repository.GetAllAsync(p => p.Category);
        IEnumerable<ProductRequest> product = result
        .Select(productEntity => new ProductRequest
        {   Id   = (Guid)productEntity.Id,
            Name = productEntity.Name,
            Description = productEntity.Description,
            BarCode= productEntity.BarCode,
            ExpiredDate = productEntity.ExpiredDate,
            CategoryName = productEntity.Category.Name,
            Price = productEntity.Price,
            Quantity = productEntity.Quantity,
            MinimumReorderQuantity = productEntity.MinimumReorderQuantity
            // Add other properties as needed
        });

        return new ServiceResult<IEnumerable<ProductRequest>>(product, true, HttpStatusCode.OK, "Category is added with success");
    }


   
}
