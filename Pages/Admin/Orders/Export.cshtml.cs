using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;

namespace OnlineStore.Pages.Admin.Orders
{
    [Authorize(Roles = "Admin")]
    public class ExportModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ExportModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .OrderBy(o => o.Id)
                .ToListAsync();

            var sb = new StringBuilder();
            // Заголовки
            sb.AppendLine("OrderId,OrderDate,UserId,Status,TotalAmount,ItemProductName,ItemPrice,ItemQuantity");

            foreach (var o in orders)
            {
                if (o.Items != null && o.Items.Any())
                {
                    foreach (var it in o.Items)
                    {
                        var productName = it.Product?.Name ?? "";
                        var line = string.Join(",",
                            o.Id.ToString(),
                            o.OrderDate.ToString("o", CultureInfo.InvariantCulture),
                            EscapeCsv(o.UserId),
                            EscapeCsv(o.Status),
                            o.TotalAmount.ToString(CultureInfo.InvariantCulture),
                            EscapeCsv(productName),
                            it.Price.ToString(CultureInfo.InvariantCulture),
                            it.Quantity.ToString());
                        sb.AppendLine(line);
                    }
                }
                else
                {
                    var line = string.Join(",",
                        o.Id.ToString(),
                        o.OrderDate.ToString("o", CultureInfo.InvariantCulture),
                        EscapeCsv(o.UserId),
                        EscapeCsv(o.Status),
                        o.TotalAmount.ToString(CultureInfo.InvariantCulture),
                        "", "", "");
                    sb.AppendLine(line);
                }
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"orders_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            return File(bytes, "text/csv; charset=utf-8", fileName);
        }

        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }
    }
}