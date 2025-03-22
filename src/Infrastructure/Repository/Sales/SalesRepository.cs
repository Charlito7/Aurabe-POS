using Core.Application.Interface.Repository.Sales;
using Core.Domain.Entity;
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
    
    public async Task<IEnumerable<SalesMetadataAndProductResponse>> GetSaleDetailsBySellerAsync(Guid salesMetadataId, string userId)
    {
        return await _context.SalesMetadataAndProductResponses
.FromSqlRaw("CALL GetSalesMetadataAndProductBySeller({0}, {1})", salesMetadataId, userId)
.ToListAsync();
    }
    public async Task<IEnumerable<SalesMetadataAndProductResponse>> GetAllSaleDetailsAsync(Guid salesMetadataId)
    {
        return await _context.SalesMetadataAndProductResponses
.FromSqlRaw("CALL GetAllSalesMetadataAndProduct({0})", salesMetadataId)
.ToListAsync();
    }

    public async Task<SellerDailyResumeEntity> GetSellerDailyResumeAsync(Guid? userId)
    {
        try
        {
            return await _context.SellerDailyResumes
    .FirstAsync(c => c.SellerId == userId && c.Date == DateOnly.FromDateTime(DateTime.UtcNow));
        }
        catch
        {
            SellerDailyResumeEntity sellerDailyResumeEntity = new SellerDailyResumeEntity()
            {
                AmountIN = 0
            };
            return sellerDailyResumeEntity;
        }
       // return await _context.SellerDailyResumes.FirstAsync(c => c.SellerId == userId);
    }
   
    public async Task<IEnumerable<GetSellerSalesTotalPriceAndQuantityToday>> GetSellerSalesTotalPriceAndQuantityTodayAsync(string userId)
    {
        return await _context.GetSellerSalesTotalPriceAndQuantityTodays
.FromSqlRaw("CALL GetTodaySellerDashboard({0})", userId)
.ToListAsync();
    }
}
