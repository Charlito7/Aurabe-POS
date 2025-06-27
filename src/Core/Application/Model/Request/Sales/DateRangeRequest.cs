using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Model.Request.Sales;

public class DateRangeRequest
{
    [Required]
    public string StartDate { get; set; }
    [Required]
    public string EndDate { get; set; }
    public int? TimeZoneOffset { get; set; }

}
