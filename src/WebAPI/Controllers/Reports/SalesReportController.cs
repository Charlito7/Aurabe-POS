using Core.Application.Interface.Services.Reports;
using Core.Application.Interface.Services.Sales;
using Core.Application.Interface.Token;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;


namespace WebAPI.Controllers.Reports
{
    [Route("sales/report")]
    public class SalesReportController : Controller
    {
        private readonly ISalesSummaryReports _salesSummary;


        public SalesReportController(
            ISalesSummaryReports salesSummary)
        {
            _salesSummary = salesSummary;
        }
        [HttpPost("daily")]
        public async Task<IActionResult> DailyReportsAsync([FromHeader(Name = "X-Signature")] string signature, [FromHeader(Name = "X-Timestamp")] string timestamp)
        {
            var secretKey = Environment.GetEnvironmentVariable("DAILY_JOB_SECRET_KEY")!;
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(timestamp));
            var expectedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

            if (signature != expectedSignature)
            {
                return Unauthorized("Invalid signature");
            }

            await _salesSummary.DailySalesSummaryReports();
            return Ok("Secure task processed");
        }

    }
}
