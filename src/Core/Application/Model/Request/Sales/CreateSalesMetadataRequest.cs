using Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Model.Request.Sales;

public class CreateSalesMetadataRequest
{
    [Required]
    public DateTime TransactionDate { get; set; }
    public string CustomerCode { get; set; }
    public decimal OrderTaxPercentage { get; set; }
    [Required]
    public decimal ShippingCost { get; set; }
    public string ShippingAddress { get; set; }
    [Required]
    public string Status { get; set; }
    public string Notes { get; set; }
    public string SellerCode { get; set; }
    [Required]
    public string SellerName { get; set; }
    [Required]
    public decimal TotalAmount { get; set; }
    [Required]
    public decimal CashReceived { get; set; }
    public decimal ChangeDue { get; set; }
    public string PaymentType { get; set; }
    public string PaymentTypeTransactionID { get; set; }
}
