using Core.Application.Commons.ServiceResult;
using Core.Application.Model.Request;
using System.Net;

namespace Core.Application.Interface;

public interface IProductService
{
    public Task<ServiceResult<IEnumerable<ProductRequest>>> GetProductsAsync();
    public Task<CreateProductRequest> GetProductByIdAsync(Guid productId);
    public Task<ServiceResult<IEnumerable<ProductRequest>>> CreateProductsAsync(List<ProductRequest> requests);
    public Task<ServiceResult<HttpStatusCode>> DeleteProductAsync(Guid productId);
    public Task<ServiceResult<ProductRequest>> GetProductByNameAsync(string name);
}
