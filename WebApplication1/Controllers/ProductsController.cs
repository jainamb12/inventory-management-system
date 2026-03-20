using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "InventoryManager")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Products.ToListAsync());
        //}

        // GET: Products
        //public async Task<IActionResult> Index()
        //{
        //    // The .Include() commands are the magic words! 
        //    // They tell SQL to perform a JOIN and fetch the actual Category and Supplier names.
        //    var smartInventoryContext = _context.Products
        //        .Include(p => p.Category)
        //        .Include(p => p.Supplier);

        //    return View(await smartInventoryContext.ToListAsync());
        //}

        // GET: Products
        //public async Task<IActionResult> Index(string searchString)
        //{
        //    // Save the search word so we can keep it in the search box after the page reloads
        //    ViewData["CurrentFilter"] = searchString;

        //    // Start building the query (but don't execute it yet)
        //    var inventoryQuery = _context.Products
        //        .Include(p => p.Category)
        //        .Include(p => p.Supplier)
        //        .AsQueryable();

        //    // If the user typed something in the search box, apply the filters!
        //    if (!string.IsNullOrEmpty(searchString))
        //    {
        //        inventoryQuery = inventoryQuery.Where(p =>
        //            p.Name.Contains(searchString) ||
        //            (p.Category != null && p.Category.Name.Contains(searchString)) ||
        //            (p.Supplier != null && p.Supplier.Name.Contains(searchString)));
        //    }

        //    // Execute the query and send the filtered list to the view
        //    return View(await inventoryQuery.ToListAsync());
        //}

        // GET: Products
        public async Task<IActionResult> Index(string searchString, int? pageNumber)
        {
            // Save the search word to keep it in the box
            ViewData["CurrentFilter"] = searchString;

            var inventoryQuery = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .AsQueryable();

            // 1. Apply the Search Filter (if any)
            if (!string.IsNullOrEmpty(searchString))
            {
                inventoryQuery = inventoryQuery.Where(p =>
                    p.Name.Contains(searchString) ||
                    (p.Category != null && p.Category.Name.Contains(searchString)) ||
                    (p.Supplier != null && p.Supplier.Name.Contains(searchString)));

                // If they search, reset to page 1 to show the new results
                pageNumber = 1;
            }

            // --- PAGINATION MATH ---
            int pageSize = 10; // Show 10 items per page
            int pageIndex = pageNumber ?? 1; // If no page number is provided, default to 1

            // Count total items AFTER the search filter is applied
            int totalItems = await inventoryQuery.CountAsync();

            // Calculate total pages (e.g., 50 items / 10 = 5 pages)
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Apply Skip and Take to the database query
            var items = await inventoryQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Send the pagination details to the HTML View
            ViewBag.PageIndex = pageIndex;
            ViewBag.TotalPages = totalPages;
            // -----------------------

            return View(items);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        //// GET: Products/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Products/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name,Category,Price,Quantity")] Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(product);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(product);
        //}

        // GET: Products/Create
        public IActionResult Create()
        {
            // We change "Id", "Id" to "Id", "Name" so the dropdown shows the actual word instead of a number
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CategoryId,SupplierId,Price,Quantity")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If the form fails, we must reload the dropdowns before showing the page again!
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", product.SupplierId);
            return View(product);
        }

        //// GET: Products/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products.FindAsync(id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // These lines build the dropdowns for the Edit page
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", product.SupplierId);
            return View(product);
        }

        //// POST: Products/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Category,Price,Quantity")] Product product)
        //{
        //    if (id != product.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(product);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProductExists(product.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(product);
        //}

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CategoryId,SupplierId,Price,Quantity")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // If saving fails, reload the dropdowns!
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            ViewData["SupplierId"] = new SelectList(_context.Suppliers, "Id", "Name", product.SupplierId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
