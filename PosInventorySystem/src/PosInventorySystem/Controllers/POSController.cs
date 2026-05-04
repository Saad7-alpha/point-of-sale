using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosInventorySystem.Services;
using PosInventorySystem.ViewModels;

namespace PosInventorySystem.Controllers;

[Authorize]
public class POSController : Controller
{
    private readonly IProductService _productService;
    private readonly ISalesService _salesService;
    private readonly IXmlExportService _xmlExportService;
    private readonly decimal _taxRate;

    public POSController(
        IProductService productService,
        ISalesService salesService,
        IXmlExportService xmlExportService,
        IConfiguration configuration)
    {
        _productService = productService;
        _salesService = salesService;
        _xmlExportService = xmlExportService;
        _taxRate = configuration.GetValue<decimal>("AppSettings:VATRate");
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();
        var model = new POSSessionViewModel
        {
            TaxRate = _taxRate
        };
        
        ViewBag.Products = products;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutViewModel model)
    {
        if (model.Items == null || !model.Items.Any())
        {
            TempData["Error"] = "Cart is empty!";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var sale = new Models.Sale
            {
                CashierId = User.FindSystemResourceClaim("sub")?.Value ?? 
                           User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "",
                CustomerName = string.IsNullOrEmpty(model.CustomerName) ? "Walk-in Customer" : model.CustomerName,
                CustomerPhone = model.CustomerPhone,
                PaymentMethod = model.PaymentMethod,
                Notes = model.Notes
            };

            var saleItems = model.Items.Select(i => new Models.SaleItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                DiscountAmount = i.Discount
            }).ToList();

            var createdSale = await _salesService.CreateSaleAsync(sale, saleItems);

            // Export to XML
            await _xmlExportService.ExportInvoiceToXmlAsync(createdSale.Id);

            TempData["Success"] = $"Sale completed! Invoice: {createdSale.InvoiceNumber}";
            TempData["InvoiceNumber"] = createdSale.InvoiceNumber;
            
            return RedirectToAction(nameof(Invoice), new { id = createdSale.Id });
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error processing sale: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> Invoice(int id)
    {
        var sale = await _salesService.GetSaleByIdAsync(id);
        if (sale == null) return NotFound();

        return View(sale);
    }

    [HttpGet]
    public async Task<IActionResult> GetProduct(string barcode)
    {
        var product = await _productService.GetProductByBarcodeAsync(barcode);
        if (product == null)
            return Json(new { success = false, message = "Product not found" });

        return Json(new
        {
            success = true,
            product = new
            {
                product.Id,
                product.Name,
                Price = product.SellingPrice,
                Stock = product.StockQuantity,
                Barcode = product.Barcode,
                Available = product.StockQuantity > 0
            }
        });
    }

    [HttpGet]
    public async Task<IActionResult> SearchProducts(string term)
    {
        var products = await _productService.SearchProductsAsync(term);
        return Json(products.Where(p => p.StockQuantity > 0).Select(p => new
        {
            p.Id,
            p.Name,
            Price = p.SellingPrice,
            Stock = p.StockQuantity,
            p.Barcode
        }));
    }
}
