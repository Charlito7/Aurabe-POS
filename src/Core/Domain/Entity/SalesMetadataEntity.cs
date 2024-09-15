using Core.Domain.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entity;

[NotMapped]
public class SalesMetadataEntity : AuditableEntity
{
    public DateTime TransactionDate { get; set; }
    public string CustomerCode { get; set; }
    public decimal OrderTaxPercentage { get; set; }
    public decimal ShippingCost { get; set; }
    public string ShippingAddress { get; set; }
    public string Status { get; set; }
    public string Notes { get; set; }
    public string SellerCode { get; set; }
    public string SellerName { get; set; }
    public decimal TotalAmount { get; set; } 
    public decimal CashReceived { get; set; }
    public decimal ChangeDue { get; set; } 
    public string PaymentType { get; set; }
    public string PaymentTypeTransactionID { get; set; }  
    public ICollection<ProductSalesEntity> ProductSales { get; set; }
}
