using Microsoft.EntityFrameworkCore;
using PosInventorySystem.Data;
using PosInventorySystem.Models;

namespace PosInventorySystem.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        product.CreatedAt = DateTime.UtcNow;
        
        // Generate barcode if not provided
        if (string.IsNullOrEmpty(product.Barcode))
        {
            product.Barcode = GenerateBarcode();
        }
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product?> UpdateProductAsync(int id, Product product)
    {
        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null)
            return null;

        existingProduct.Name = product.Name;
        existingProduct.SKU = product.SKU;
        existingProduct.Barcode = product.Barcode;
        existingProduct.Description = product.Description;
        existingProduct.PurchasePrice = product.PurchasePrice;
        existingProduct.SellingPrice = product.SellingPrice;
        existingProduct.DiscountPercent = product.DiscountPercent;
        existingProduct.StockQuantity = product.StockQuantity;
        existingProduct.LowStockThreshold = product.LowStockThreshold;
        existingProduct.IsActive = product.IsActive;
        existingProduct.TrackInventory = product.TrackInventory;
        existingProduct.CategoryId = product.CategoryId;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existingProduct;
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Product>> GetLowStockProductsAsync(int threshold)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.StockQuantity <= threshold && p.IsActive)
            .OrderBy(p => p.StockQuantity)
            .ToListAsync();
    }

    public async Task<Product?> GetProductByBarcodeAsync(string barcode)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Barcode == barcode && p.IsActive);
    }

    public async Task<List<Product>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await GetAllProductsAsync();

        searchTerm = searchTerm.ToLower();
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive && 
                       (p.Name.ToLower().Contains(searchTerm) || 
                        p.SKU!.ToLower().Contains(searchTerm) || 
                        p.Barcode!.Contains(searchTerm)))
            .OrderBy(p => p.Name)
            .Take(50)
            .ToListAsync();
    }

    public async Task UpdateStockAsync(int productId, int quantityChange)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product != null && product.TrackInventory)
        {
            product.StockQuantity += quantityChange;
            if (product.StockQuantity < 0)
                product.StockQuantity = 0;
            product.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
        }
    }

    private string GenerateBarcode()
    {
        // Generate a 13-digit barcode (EAN-13 format)
        var random = new Random();
        var barcode = "";
        for (int i = 0; i < 12; i++)
        {
            barcode += random.Next(0, 10).ToString();
        }
        
        // Calculate check digit
        int sum = 0;
        for (int i = 0; i < 12; i++)
        {
            int digit = int.Parse(barcode[i].ToString());
            sum += (i % 2 == 0) ? digit : digit * 3;
        }
        int checkDigit = (10 - (sum % 10)) % 10;
        barcode += checkDigit.ToString();
        
        return barcode;
    }
}
