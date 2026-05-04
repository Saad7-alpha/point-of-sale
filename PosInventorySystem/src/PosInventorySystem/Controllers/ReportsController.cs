using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosInventorySystem.Services;
using PosInventorySystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using PosInventorySystem.Data;

namespace PosInventorySystem.Controllers;

[Authorize(Roles = "Admin")]
public class ReportsController : Controller
{
    private readonly ISalesService _salesService;
    private readonly IXmlExportService _xmlExportService;
    private readonly ApplicationDbContext _context;

    public ReportsController(
        ISalesService salesService,
        IXmlExportService xmlExportService,
        ApplicationDbContext context)
    {
        _salesService = salesService;
        _xmlExportService = xmlExportService;
        _context = context;
    }

    public IActionResult Index()
    {
        var model = new SalesReportViewModel
        {
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> GenerateReport(SalesReportViewModel model)
    {
        var sales = await _salesService.GetSalesByDateRangeAsync(model.StartDate, model.EndDate);
        
        model.TotalTransactions = sales.Count;
        model.TotalRevenue = sales.Sum(s => s.GrandTotal);
        model.TotalTax = sales.Sum(s => s.TaxAmount);
        model.TotalDiscount = sales.Sum(s => s.DiscountAmount);
        
        model.Sales = sales.Select(s => new SaleDetailViewModel
        {
            Id = s.Id,
            InvoiceNumber = s.InvoiceNumber,
            SaleDate = s.SaleDate,
            SubTotal = s.SubTotal,
            TaxAmount = s.TaxAmount,
            DiscountAmount = s.DiscountAmount,
            GrandTotal = s.GrandTotal,
            PaymentMethod = s.PaymentMethod,
            CustomerName = s.CustomerName,
            CashierName = s.Cashier != null ? s.Cashier.FullName : "Unknown",
            Items = s.SaleItems?.Select(si => new SaleItemViewModel
            {
                ProductName = si.Product != null ? si.Product.Name : "Unknown",
                Barcode = si.Product?.Barcode,
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                TotalAmount = si.TotalAmount
            }).ToList()
        }).ToList();

        return View("ReportResult", model);
    }

    public async Task<IActionResult> ExportDailySales(DateTime date)
    {
        var filePath = await _xmlExportService.ExportDailySalesToXmlAsync(date);
        TempData["Success"] = $"Daily sales exported to: {filePath}";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ExportProducts()
    {
        var filePath = await _xmlExportService.ExportProductsToXmlAsync();
        TempData["Success"] = $"Products exported to: {filePath}";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ExportBackup()
    {
        var filePath = await _xmlExportService.ExportBackupToXmlAsync();
        TempData["Success"] = $"Backup exported to: {filePath}";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DailySales(DateTime? date)
    {
        var selectedDate = date ?? DateTime.UtcNow.Date;
        var sales = await _salesService.GetSalesByDateRangeAsync(selectedDate, selectedDate);
        
        var model = new SalesReportViewModel
        {
            StartDate = selectedDate,
            EndDate = selectedDate,
            TotalTransactions = sales.Count,
            TotalRevenue = sales.Sum(s => s.GrandTotal),
            TotalTax = sales.Sum(s => s.TaxAmount),
            Sales = sales.Select(s => new SaleDetailViewModel
            {
                Id = s.Id,
                InvoiceNumber = s.InvoiceNumber,
                SaleDate = s.SaleDate,
                GrandTotal = s.GrandTotal,
                PaymentMethod = s.PaymentMethod,
                CashierName = s.Cashier?.FullName
            }).ToList()
        };

        return View("ReportResult", model);
    }
}
