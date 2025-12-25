using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using OnlineStore.Models;

namespace OnlineStore.Pages.Admin.Products
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProductInput Input { get; set; } = new();

        public List<Category> Categories { get; set; } = new();

        public class ProductInput
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public decimal Price { get; set; }
            public int Stock { get; set; }
            public int? CategoryId { get; set; }
            public string? ImageUrl { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (!id.HasValue) return NotFound();

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id.Value);
            if (product == null) return NotFound();

            Input = new ProductInput
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl
            };

            Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Categories = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
                return Page();
            }

            var product = await _context.Products.FindAsync(Input.Id);
            if (product == null) return NotFound();

            product.Name = Input.Name;
            product.Description = Input.Description;
            product.Price = Input.Price;
            product.Stock = Input.Stock;
            product.CategoryId = Input.CategoryId ?? 0;
            product.ImageUrl = string.IsNullOrWhiteSpace(Input.ImageUrl) ? "/images/placeholder.png" : Input.ImageUrl;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Товар обновлён";
            return RedirectToPage("/Admin/Index", new { section = "products" });
        }
    }
}