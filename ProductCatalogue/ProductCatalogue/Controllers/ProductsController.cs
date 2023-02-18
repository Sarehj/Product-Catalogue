using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProductCatalogue.Models;

namespace ProductCatalogue.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductContext _context;

        public ProductsController(ProductContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return _context.Product != null ?
                        View(await _context.Product.ToListAsync()) :
                        Problem("Entity set 'ProductContext.Product'  is null.");
        }

        public IActionResult Create()
        {
            return View();
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,Name,Type,Price")] Product product)
        {
            var existingProduct = _context.Product.FirstOrDefault(p => p.Name == product.Name);
            if (existingProduct != null)
            {
                ModelState.AddModelError("", "A product with the same name already exists.");
                return View(product);
            }

            ValidatePrice(product);
            if (ModelState.IsValid)
            {
                product.id = Guid.NewGuid();
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

          return View(product);
        }

    
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("id,Name,Type,Price")] Product product)
        {
            if (id != product.id)
            {
                return NotFound();
            }
            var existingProduct = _context.Product.FirstOrDefault(p => p.Name == product.Name && p.id != product.id);
            if (existingProduct != null)
            {
                ModelState.AddModelError("", "A product with the same name already exists.");
                return View(product);
            }
            
            ValidatePrice(product);
            if (ModelState.IsValid)
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

          return View(product);
        }

 
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                                .FirstOrDefaultAsync(m => m.id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'ProductContext.Product'  is null.");
            }
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public void ValidatePrice(Product p)
        {
            if (p.Type == "Integrated" && (p.Price is < 1000 or > 2600))
            {
                ModelState.AddModelError("", "The price of the Integrated must be between 1000 and 2600 dollars.");
            }
            if (p.Type == "Peripheral" && p.Price <= 0)
            {
                ModelState.AddModelError("", "The price of the Peripheral must be a positive, non-zero value.");
            }

        }
    }
}
