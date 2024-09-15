using AutoMapper;
using Azure.Core;
using Core.Application.Commons.ServiceResult;
using Core.Application.Interface;
using Core.Application.Interface.Services.Sales;
using Core.Application.Model.Request;
using Core.Application.Model.Request.Sales;
using Core.Application.Model.Response.Sales;
using Core.Domain.Entity;
using Core.Domain.Enums;
using Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Sales;

public class CreateSalesService : ICreateSalesService
{
    private readonly IRepository<SalesMetadataEntity> _salesMetadataRepository;
    private readonly IRepository<ProductSalesEntity> _productSalesRepository;
    private readonly IRepository<ProductEntity> _productRepository;
    private IMapper _mapper;

    public CreateSalesService(IRepository<SalesMetadataEntity> salesMetadataRepository,
                              IRepository<ProductSalesEntity> productSalesRepository,
                              IRepository<ProductEntity> productRepository,IMapper mapper)
    {
        _salesMetadataRepository = salesMetadataRepository;
        _productSalesRepository = productSalesRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }
    public async Task<ServiceResult<CreateSalesResponse>> CreateSalesAsync(CreateSalesRequest request)
    {
        CreateSalesResponse salesResponse = new CreateSalesResponse();
        decimal amount = 0;
        //Check request
        if (string.IsNullOrEmpty(request.SalesMetadata.Status.ToString()))
        {
            return new ServiceResult<CreateSalesResponse>(salesResponse, false, HttpStatusCode.BadRequest, "");

        }

        //Check seller info

        // Check
        if (request.SalesMetadata.ChangeDue != (request.SalesMetadata.CashReceived - request.SalesMetadata.TotalAmount))
        {
            return new ServiceResult<CreateSalesResponse>(salesResponse, false, HttpStatusCode.BadRequest, 
                "Error on the refund");

        }

        //check 
        if (request.SalesMetadata.PaymentType == PaymentMethodEnum.MONCASH.ToString() || request.SalesMetadata.PaymentType == PaymentMethodEnum.NATCASH.ToString() || request.SalesMetadata.PaymentType == PaymentMethodEnum.PAV.ToString())
        {
            if (string.IsNullOrEmpty(request.SalesMetadata.PaymentTypeTransactionID))
            {
                return new ServiceResult<CreateSalesResponse>(salesResponse, false, HttpStatusCode.BadRequest,
                    request.SalesMetadata.PaymentType+ " Transaction ID is missing");

            }
        }


        foreach (var req in request.ProductSales) {
            var prod = await _productRepository.FindAsync(c => c.BarCode == req.ProductCode);
            if (prod == null) {
                return new ServiceResult<CreateSalesResponse>(salesResponse, false, HttpStatusCode.BadRequest,
                    "Product name: " + req.ProductName + " not on sales please remove it");
            }

            if (prod.Price != req.UnitCost)
            {
                return new ServiceResult<CreateSalesResponse>(salesResponse, false, HttpStatusCode.BadRequest, 
                    "We have only " + prod.Quantity + "The price for " + req.ProductName + " is update, remove it and scan it again");
            }

            if (req.Quantity > prod.Quantity) {
                return new ServiceResult<CreateSalesResponse>(salesResponse, false, HttpStatusCode.BadRequest, "We have only " + prod.Quantity + " available   for remove it and scan it again, to continue");
            }
            amount += prod.Price*req.Quantity;
        }

        if(amount != request.SalesMetadata.TotalAmount)
        {
            return new ServiceResult<CreateSalesResponse>(salesResponse, false, HttpStatusCode.BadRequest, "");
        }

        // Save the Metadata
        var salesMetadataEntity = new SalesMetadataEntity()
        {
            TransactionDate = request.SalesMetadata.TransactionDate,
            CustomerCode = request.SalesMetadata.CustomerCode,
            OrderTaxPercentage = request.SalesMetadata.OrderTaxPercentage,
            ShippingCost = request.SalesMetadata.ShippingCost,
            ShippingAddress = request.SalesMetadata.ShippingAddress,
            Status = request.SalesMetadata.Status.ToString(),
            Notes = request.SalesMetadata.Notes ?? string.Empty,
            SellerCode = request.SalesMetadata.SellerCode,
            SellerName = request.SalesMetadata.SellerName,
            TotalAmount = request.SalesMetadata.TotalAmount,
            CashReceived = request.SalesMetadata.CashReceived,
            ChangeDue = request.SalesMetadata.ChangeDue,
            PaymentType = request.SalesMetadata.PaymentType.ToString(),
            PaymentTypeTransactionID = request.SalesMetadata.PaymentTypeTransactionID

        };
        var result = await _salesMetadataRepository.CreateTAsync(salesMetadataEntity);

        //Save the Products sales
        if (result != null)
        {
            foreach (var item in request.ProductSales) {
                ProductSalesEntity product = new ProductSalesEntity()
                {
                    ProductCode = item.ProductCode,
                    ProductName = item.ProductName,
                    UnitCost = item.UnitCost,
                    Quantity = item.Quantity,
                    Discount = item.Discount,
                    SalesMetadataId = result.Id
                };

                var resultProducts = await _productSalesRepository.CreateAsync(product);

                if (!resultProducts)
                {
                    //Delete all the products and the metadata
                    await _productSalesRepository.DeleteAsync(c => c.SalesMetadataId == result.Id);
                    await _salesMetadataRepository.DeleteAsync(c => c.Id == result.Id);
                    return new ServiceResult<CreateSalesResponse>(salesResponse, false, HttpStatusCode.InternalServerError, "Cannot add product: "+product.ProductCode);
                }
            }            
        }


        return new ServiceResult<CreateSalesResponse>(salesResponse, true, HttpStatusCode.OK, "");
    }
}
