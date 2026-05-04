using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosInventorySystem.Data;
using PosInventorySystem.Models;
using PosInventorySystem.Services;
using PosInventorySystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace PosInventorySystem.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;
    private readonly IBarcodeService _barcodeService;

    public ProductsController(
        IProductService productService,
        ICategoryService categoryService,
        IBarcodeService barcodeService)
    {
        _productService = productService;
        _categoryService = categoryService;
        _barcodeService = barcodeService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();
        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        var categories = await _categoryService.GetActiveCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductViewModel model)
    {
        if (ModelState.IsValid)
        {
            var product = new Product
            {
                Name = model.Name,
                SKU = model.SKU,
                Barcode = model.Barcode,
                Description = model.Description,
                PurchasePrice = model.PurchasePrice,
                SellingPrice = model.SellingPrice,
                DiscountPercent = model.DiscountPercent,
                StockQuantity = model.StockQuantity,
                LowStockThreshold = model.LowStockThreshold,
                IsActive = model.IsActive,
                TrackInventory = model.TrackInventory,
                CategoryId = model.CategoryId
            };

            await _productService.CreateProductAsync(product);
            
            // Generate barcode image
            if (!string.IsNullOrEmpty(product.Barcode))
            {
                _barcodeService.GenerateBarcode(product.Barcode);
            }

            TempData["Success"] = "Product created successfully!";
            return RedirectToAction(nameof(Index));
        }

        var categories = await _categoryService.GetActiveCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        return View(model);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var product = await _productService.GetProductByIdAsync(id.Value);
        if (product == null) return NotFound();

        var model = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Barcode = product.Barcode,
            Description = product.Description,
            PurchasePrice = product.PurchasePrice,
            SellingPrice = product.SellingPrice,
            DiscountPercent = product.DiscountPercent,
            StockQuantity = product.StockQuantity,
            LowStockThreshold = product.LowStockThreshold,
            IsActive = product.IsActive,
            TrackInventory = product.TrackInventory,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name
        };

        var categories = await _categoryService.GetActiveCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", product.CategoryId);
        
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductViewModel model)
    {
        if (id != model.Id) return NotFound();

        if (ModelState.IsValid)
        {
            var product = new Product
            {
                Id = model.Id,
                Name = model.Name,
                SKU = model.SKU,
                Barcode = model.Barcode,
                Description = model.Description,
                PurchasePrice = model.PurchasePrice,
                SellingPrice = model.SellingPrice,
                DiscountPercent = model.DiscountPercent,
                StockQuantity = model.StockQuantity,
                LowStockThreshold = model.LowStockThreshold,
                IsActive = model.IsActive,
                TrackInventory = model.TrackInventory,
                CategoryId = model.CategoryId
            };

            await _productService.UpdateProductAsync(id, product);
            TempData["Success"] = "Product updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        var categories = await _categoryService.GetActiveCategoriesAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", model.CategoryId);
        return View(model);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var product = await _productService.GetProductByIdAsync(id.Value);
        if (product == null) return NotFound();

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _productService.DeleteProductAsync(id);
        TempData["Success"] = "Product deleted successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetProductByBarcode(string barcode)
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
                product.SellingPrice,
                product.StockQuantity,
                product.Barcode
            }
        });
    }

    [HttpGet]
    public async Task<IActionResult> Search(string term)
    {
        var products = await _productService.SearchProductsAsync(term);
        return Json(products.Select(p => new
        {
            p.Id,
            p.Name,
            p.SellingPrice,
            p.StockQuantity,
            p.Barcode,
            CategoryName = p.Category?.Name
        }));
    }
}
