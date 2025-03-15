using Core.Application.Interface.Repository.Sales;
using Core.Domain.Procedures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Sales;

public class SalesRepository : ISalesRepository
{
    private readonly AppDbContext _context;

    public SalesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<GetAllSalesMetadata>> GetAllSalesMetadataAsync()
    {
        return await _context.GetAllSalesMetadata
.FromSqlRaw("CALL GetAllSalesMetadata()")
.ToListAsync();
    }
    

    public async Task<IEnumerable<GetAllSalesMetadata>> GetAllSalesMetadataPaginationAsync(int pageNumber, int pageSize)
    {
        return await _context.GetAllSalesMetadata
.FromSqlRaw("CALL GetAllSalesMetadataPagination({0}, {1})", pageNumber, pageSize)
.ToListAsync();
    }

    public async Task<IEnumerable<GetAllSalesMetadata>> GetAllSalesMetadataPaginationBySellerAsync(int pageNumber, int pageSize, string sellerId)
    {
        return await _context.GetAllSalesMetadata
.FromSqlRaw("CALL GetAllSalesMetadataPaginationBySeller({0}, {1}, {2})", pageNumber, pageSize, sellerId)
.ToListAsync();
    }
}
