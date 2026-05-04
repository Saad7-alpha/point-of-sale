using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosInventorySystem.Data;
using PosInventorySystem.Services;
using PosInventorySystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace PosInventorySystem.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ISalesService _salesService;
    private readonly IProductService _productService;

    public DashboardController(
        ApplicationDbContext context,
        ISalesService salesService,
        IProductService productService)
    {
        _context = context;
        _salesService = salesService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel();

        // Total products and categories
        model.TotalProducts = await _context.Products.CountAsync(p => p.IsActive);
        model.TotalCategories = await _context.Categories.CountAsync(c => c.IsActive);

        // Today's sales
        var today = DateTime.UtcNow.Date;
        var todaySales = await _context.Sales
            .Where(s => s.SaleDate >= today && s.SaleDate < today.AddDays(1) && !s.IsRefunded)
            .ToListAsync();
        
        model.TodaySales = todaySales.Count;
        model.TodayRevenue = todaySales.Sum(s => s.GrandTotal);

        // Month revenue
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthSales = await _context.Sales
            .Where(s => s.SaleDate >= monthStart && s.SaleDate < today.AddDays(1) && !s.IsRefunded)
            .ToListAsync();
        
        model.MonthRevenue = monthSales.Sum(s => s.GrandTotal);
        model.MonthTransactions = monthSales.Count;

        // Low stock products
        var lowStockThreshold = 10; // Could be from config
        model.LowStockProducts = await _context.Products
            .CountAsync(p => p.StockQuantity <= lowStockThreshold && p.IsActive);

        // Recent sales
        model.RecentSales = await _context.Sales
            .Include(s => s.Cashier)
            .Include(s => s.SaleItems)
            .Where(s => !s.IsRefunded)
            .OrderByDescending(s => s.SaleDate)
            .Take(10)
            .Select(s => new SaleSummaryViewModel
            {
                Id = s.Id,
                InvoiceNumber = s.InvoiceNumber,
                SaleDate = s.SaleDate,
                GrandTotal = s.GrandTotal,
                CashierName = s.Cashier != null ? s.Cashier.FullName : "Unknown",
                ItemCount = s.SaleItems != null ? s.SaleItems.Count : 0
            })
            .ToListAsync();

        // Low stock items
        model.LowStockItems = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.StockQuantity <= lowStockThreshold && p.IsActive)
            .OrderBy(p => p.StockQuantity)
            .Take(5)
            .Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                StockQuantity = p.StockQuantity,
                LowStockThreshold = p.LowStockThreshold,
                CategoryName = p.Category != null ? p.Category.Name : "Uncategorized"
            })
            .ToListAsync();

        return View(model);
    }
}
