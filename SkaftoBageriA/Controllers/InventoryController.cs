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
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var inventories = await _context.Inventories
                .Include(i => i.Product)
                .Include(i => i.Supplier)
                .ToListAsync();
            return View(inventories);
        }

        [Authorize(Roles = "Admin,User")]
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inventory inventory)
        {
            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return View(inventory);
            }

            try
            {
                _context.Inventories.Add(inventory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while adding the inventory.");
                PopulateDropdowns();
                return View(inventory);
            }
        }

        private void PopulateDropdowns()
        {
            ViewData["Products"] = new SelectList(_context.Products.OrderBy(p => p.ProductName), "ProductId", "ProductName");
            ViewData["Suppliers"] = new SelectList(_context.Suppliers.OrderBy(s => s.Name), "SupplierId", "Name");
        }
    }
}
