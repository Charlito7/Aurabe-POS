using AutoMapper;
using Core.Application.Commons.ServiceResult;
using Core.Application.Interface;
using Core.Application.Interface.Services.Sales;
using Core.Application.Model.Request.Sales;
using Core.Application.Model.Response.Sales;
using Core.Domain.Entity;
using Core.Domain.Enums;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace Infrastructure.Services.Sales
{
    public class CreateSalesMetadataService : ICreateSalesMetadataService
    {
        private readonly IRepository<SalesMetadataEntity> _repository;
        private IMapper _mapper;

        public CreateSalesMetadataService(IRepository<SalesMetadataEntity> repository,
            IMapper mapper)
        {

            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ServiceResult<CreateSalesMetadataResponse>> CreateSalesMetadataAsync(CreateSalesMetadataRequest request)
        {
            if (string.IsNullOrEmpty(request.Status.ToString()) )
            {

            }

            //Check seller info

            // Check
            if(request.ChangeDue != (request.CashReceived - request.TotalAmount)) { 

            }

            //check 
            if(request.PaymentType == PaymentMethodEnum.MONCASH.ToString() || request.PaymentType == PaymentMethodEnum.NATCASH.ToString() || request.PaymentType == PaymentMethodEnum.PAV.ToString())
            {
                if(string.IsNullOrEmpty(request.PaymentTypeTransactionID))
                {

                }
            }

            var salesMetadataEntity = new SalesMetadataEntity()
            {
                TransactionDate = request.TransactionDate,
                CustomerCode = request.CustomerCode,
                OrderTaxPercentage = request.OrderTaxPercentage,
                ShippingCost = request.ShippingCost,
                ShippingAddress = request.ShippingAddress,
                Status = request.Status.ToString(),
                Notes = request.Notes ?? string.Empty,
                SellerCode = request.SellerCode,
                SellerName = request.SellerName,
                TotalAmount = request.TotalAmount,
                CashReceived = request.CashReceived,
                ChangeDue = request.ChangeDue,
                PaymentType = request.PaymentType.ToString(),
                PaymentTypeTransactionID = request.PaymentTypeTransactionID

            };
            var result = await _repository.CreateAsync(salesMetadataEntity);
            throw new NotImplementedException();
        }
    }
}
