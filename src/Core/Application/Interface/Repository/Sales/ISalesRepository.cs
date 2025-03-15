using Core.Application.Model.Response;
using Core.Domain.Procedures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interface.Repository.Sales;

public interface ISalesRepository
{
    Task<IEnumerable<GetAllSalesMetadata>> GetAllSalesMetadataPaginationAsync(int pageNumber, int pageSize);
    Task<IEnumerable<GetAllSalesMetadata>> GetAllSalesMetadataPaginationBySellerAsync(int pageNumber, int pageSize, string? sellerId);
    Task<IEnumerable<GetAllSalesMetadata>> GetAllSalesMetadataAsync();

}
