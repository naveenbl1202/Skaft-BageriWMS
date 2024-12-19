using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using SkaftoBageriA.Data;
using SkaftoBageriA.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SkaftoBageriA.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Supplier)
                .ToListAsync();
            return View(products);
        }

        [Authorize(Roles = "Admin,User")]
        public IActionResult Create()
        {
            PopulateSuppliersDropdown();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                PopulateSuppliersDropdown();
                return View(product);
            }

            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while adding the product.");
                PopulateSuppliersDropdown();
                return View(product);
            }
        }

        private void PopulateSuppliersDropdown()
        {
            ViewData["Suppliers"] = new SelectList(_context.Suppliers.OrderBy(s => s.Name), "SupplierId", "Name");
        }
    }
}
