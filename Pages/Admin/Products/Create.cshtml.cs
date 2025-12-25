using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Models;

namespace OnlineStore.Pages.Admin.Products
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProductInputModel Input { get; set; } = new();

        public List<Category> Categories { get; set; } = new();

        public class ProductInputModel
        {
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public int? CategoryId { get; set; }
            public string? ImageUrl { get; set; }
        }

        public async Task OnGetAsync()
        {
            Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
                return Page();
            }

            var product = new Product
            {
                Name = Input.Name,
                Description = Input.Description,
                Price = Input.Price,
                Stock = Input.Stock,
                CategoryId = Input.CategoryId ?? 0,
                ImageUrl = string.IsNullOrWhiteSpace(Input.ImageUrl) ? "/images/placeholder.png" : Input.ImageUrl,
                CreatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Товар добавлен";
            return RedirectToPage("/Admin/Index", new { section = "products" });
        }
    }
}
