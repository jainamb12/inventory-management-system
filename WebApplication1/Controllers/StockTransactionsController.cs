using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [Authorize(Roles = "InventoryManager")]
    public class StockTransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StockTransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StockTransactions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StockTransactions.Include(s => s.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StockTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockTransaction = await _context.StockTransactions
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockTransaction == null)
            {
                return NotFound();
            }

            return View(stockTransaction);
        }

        // GET: StockTransactions/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        //// POST: StockTransactions/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,ProductId,TransactionType,Quantity,TransactionDate,UserEmail")] StockTransaction stockTransaction)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(stockTransaction);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", stockTransaction.ProductId);
        //    return View(stockTransaction);
        //}

        // POST: StockTransactions/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,ProductId,TransactionType,Quantity")] StockTransaction stockTransaction)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // MAGIC: Auto-fill the date and the logged-in user!
        //        stockTransaction.TransactionDate = DateTime.Now;
        //        stockTransaction.UserEmail = User.Identity?.Name ?? "Admin";

        //        _context.Add(stockTransaction);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // If it fails, reload the dropdown
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", stockTransaction.ProductId);
        //    return View(stockTransaction);
        //}

        // POST: StockTransactions/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,ProductId,TransactionType,Quantity")] StockTransaction stockTransaction)
        //{
        //    // 1. Tell the strict security check to ignore these missing fields
        //    ModelState.Remove("UserEmail");
        //    ModelState.Remove("TransactionDate");
        //    ModelState.Remove("Product");

        //    // 2. NOW check if the form is valid
        //    if (ModelState.IsValid)
        //    {
        //        // Auto-fill the date and the logged-in user
        //        stockTransaction.TransactionDate = DateTime.Now;
        //        stockTransaction.UserEmail = User.Identity?.Name ?? "Admin";

        //        _context.Add(stockTransaction);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // If saving fails, reload the dropdown
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", stockTransaction.ProductId);
        //    return View(stockTransaction);
        //}

        // POST: StockTransactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,TransactionType,Quantity")] StockTransaction stockTransaction)
        {
            // Ignore missing fields from the form
            ModelState.Remove("UserEmail");
            ModelState.Remove("TransactionDate");
            ModelState.Remove("Product");

            if (ModelState.IsValid)
            {
                // 1. Auto-fill the date and the logged-in user
                stockTransaction.TransactionDate = DateTime.Now;
                stockTransaction.UserEmail = User.Identity?.Name ?? "Admin";

                // --- NEW BUSINESS LOGIC: UPDATE THE INVENTORY ---

                // Find the specific product in the database
                var product = await _context.Products.FindAsync(stockTransaction.ProductId);

                if (product != null)
                {
                    if (stockTransaction.TransactionType == "IN")
                    {
                        // Add to the stock
                        product.Quantity += stockTransaction.Quantity;
                    }
                    else if (stockTransaction.TransactionType == "OUT")
                    {
                        // Safeguard: Prevent negative inventory!
                        if (product.Quantity < stockTransaction.Quantity)
                        {
                            ModelState.AddModelError("Quantity", $"Error: You only have {product.Quantity} in stock. You cannot remove {stockTransaction.Quantity}.");
                            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", stockTransaction.ProductId);
                            return View(stockTransaction);
                        }

                        // Subtract from the stock
                        product.Quantity -= stockTransaction.Quantity;
                    }

                    // Tell the database we changed the product
                    _context.Update(product);
                }
                // ------------------------------------------------

                // Save BOTH the new ledger entry and the updated product quantity at the same time
                _context.Add(stockTransaction);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // If saving fails, reload the dropdown
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", stockTransaction.ProductId);
            return View(stockTransaction);
        }

        // GET: StockTransactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockTransaction = await _context.StockTransactions.FindAsync(id);
            if (stockTransaction == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", stockTransaction.ProductId);
            return View(stockTransaction);
        }

        // POST: StockTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,TransactionType,Quantity,TransactionDate,UserEmail")] StockTransaction stockTransaction)
        {
            if (id != stockTransaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stockTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockTransactionExists(stockTransaction.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", stockTransaction.ProductId);
            return View(stockTransaction);
        }

        // GET: StockTransactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockTransaction = await _context.StockTransactions
                .Include(s => s.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stockTransaction == null)
            {
                return NotFound();
            }

            return View(stockTransaction);
        }

        // POST: StockTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stockTransaction = await _context.StockTransactions.FindAsync(id);
            if (stockTransaction != null)
            {
                _context.StockTransactions.Remove(stockTransaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockTransactionExists(int id)
        {
            return _context.StockTransactions.Any(e => e.Id == id);
        }
    }
}
