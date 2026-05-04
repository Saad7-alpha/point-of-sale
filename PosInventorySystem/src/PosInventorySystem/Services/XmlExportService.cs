using System.Xml.Serialization;
using PosInventorySystem.Data;
using PosInventorySystem.Models;
using Microsoft.EntityFrameworkCore;

namespace PosInventorySystem.Services;

public class XmlExportService : IXmlExportService
{
    private readonly ApplicationDbContext _context;
    private readonly string _exportPath;

    public XmlExportService(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _exportPath = Path.Combine(environment.WebRootPath, "Exports");
        
        if (!Directory.Exists(_exportPath))
            Directory.CreateDirectory(_exportPath);
    }

    public async Task<string> ExportInvoiceToXmlAsync(int saleId)
    {
        var sale = await _context.Sales
            .Include(s => s.Cashier)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .FirstOrDefaultAsync(s => s.Id == saleId);

        if (sale == null)
            throw new Exception("Sale not found");

        var xmlSale = new XmlSale
        {
            InvoiceNumber = sale.InvoiceNumber,
            SaleDate = sale.SaleDate,
            SubTotal = sale.SubTotal,
            DiscountAmount = sale.DiscountAmount,
            TaxAmount = sale.TaxAmount,
            TaxPercentage = sale.TaxPercentage,
            GrandTotal = sale.GrandTotal,
            PaymentMethod = sale.PaymentMethod,
            CustomerName = sale.CustomerName,
            CustomerPhone = sale.CustomerPhone,
            CashierName = sale.Cashier?.FullName,
            Items = sale.SaleItems?.Select(i => new XmlSaleItem
            {
                ProductName = i.Product?.Name ?? "Unknown",
                Barcode = i.Product?.Barcode,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                DiscountAmount = i.DiscountAmount,
                TaxAmount = i.TaxAmount,
                TotalAmount = i.TotalAmount
            }).ToList() ?? new List<XmlSaleItem>()
        };

        var serializer = new XmlSerializer(typeof(XmlSale));
        using var writer = new StringWriter();
        serializer.Serialize(writer, xmlSale);
        
        return SaveXmlToFile(writer.ToString(), $"Invoice_{sale.InvoiceNumber}.xml");
    }

    public async Task<string> ExportDailySalesToXmlAsync(DateTime date)
    {
        var sales = await _context.Sales
            .Include(s => s.Cashier)
            .Include(s => s.SaleItems)
            .Where(s => s.SaleDate.Date == date.Date)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();

        var xmlDailySales = new XmlDailySales
        {
            ExportDate = DateTime.UtcNow,
            SalesDate = date,
            TotalSales = sales.Count,
            TotalRevenue = sales.Sum(s => s.GrandTotal),
            TotalTax = sales.Sum(s => s.TaxAmount),
            Sales = sales.Select(s => new XmlSaleSummary
            {
                InvoiceNumber = s.InvoiceNumber,
                SaleDate = s.SaleDate,
                SubTotal = s.SubTotal,
                TaxAmount = s.TaxAmount,
                GrandTotal = s.GrandTotal,
                PaymentMethod = s.PaymentMethod,
                CashierName = s.Cashier?.FullName,
                ItemCount = s.SaleItems?.Count ?? 0
            }).ToList()
        };

        var serializer = new XmlSerializer(typeof(XmlDailySales));
        using var writer = new StringWriter();
        serializer.Serialize(writer, xmlDailySales);
        
        return SaveXmlToFile(writer.ToString(), $"DailySales_{date:yyyyMMdd}.xml");
    }

    public async Task<string> ExportProductsToXmlAsync()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();

        var xmlProducts = new XmlProductList
        {
            ExportDate = DateTime.UtcNow,
            TotalProducts = products.Count,
            Products = products.Select(p => new XmlProduct
            {
                Id = p.Id,
                Name = p.Name,
                SKU = p.SKU,
                Barcode = p.Barcode,
                CategoryName = p.Category?.Name ?? "Uncategorized",
                PurchasePrice = p.PurchasePrice,
                SellingPrice = p.SellingPrice,
                StockQuantity = p.StockQuantity,
                LowStockThreshold = p.LowStockThreshold
            }).ToList()
        };

        var serializer = new XmlSerializer(typeof(XmlProductList));
        using var writer = new StringWriter();
        serializer.Serialize(writer, xmlProducts);
        
        return SaveXmlToFile(writer.ToString(), $"Products_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xml");
    }

    public async Task<string> ExportBackupToXmlAsync()
    {
        var backup = new XmlBackup
        {
            BackupDate = DateTime.UtcNow,
            Version = "1.0.0"
        };

        // Export categories
        backup.Categories = await _context.Categories.ToListAsync();
        
        // Export products
        backup.Products = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();
        
        // Export recent sales (last 30 days)
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        backup.Sales = await _context.Sales
            .Include(s => s.SaleItems)
            .Where(s => s.SaleDate >= thirtyDaysAgo)
            .ToListAsync();

        var serializer = new XmlSerializer(typeof(XmlBackup));
        using var writer = new StringWriter();
        serializer.Serialize(writer, backup);
        
        return SaveXmlToFile(writer.ToString(), $"Backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xml");
    }

    public string SaveXmlToFile(string xmlContent, string fileName, string folder = "Exports")
    {
        var fullPath = Path.Combine(_exportPath, folder);
        
        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);

        var filePath = Path.Combine(fullPath, fileName);
        File.WriteAllText(filePath, xmlContent);
        
        return filePath;
    }
}

// XML Serialization Classes
[XmlRoot("Invoice")]
public class XmlSale
{
    [XmlElement] public string InvoiceNumber { get; set; } = string.Empty;
    [XmlElement] public DateTime SaleDate { get; set; }
    [XmlElement] public decimal SubTotal { get; set; }
    [XmlElement] public decimal DiscountAmount { get; set; }
    [XmlElement] public decimal TaxAmount { get; set; }
    [XmlElement] public decimal TaxPercentage { get; set; }
    [XmlElement] public decimal GrandTotal { get; set; }
    [XmlElement] public string PaymentMethod { get; set; } = string.Empty;
    [XmlElement] public string? CustomerName { get; set; }
    [XmlElement] public string? CustomerPhone { get; set; }
    [XmlElement] public string? CashierName { get; set; }
    [XmlArray("Items")]
    [XmlArrayItem("Item")] public List<XmlSaleItem> Items { get; set; } = new();
}

public class XmlSaleItem
{
    [XmlElement] public string ProductName { get; set; } = string.Empty;
    [XmlElement] public string? Barcode { get; set; }
    [XmlElement] public int Quantity { get; set; }
    [XmlElement] public decimal UnitPrice { get; set; }
    [XmlElement] public decimal DiscountAmount { get; set; }
    [XmlElement] public decimal TaxAmount { get; set; }
    [XmlElement] public decimal TotalAmount { get; set; }
}

[XmlRoot("DailySales")]
public class XmlDailySales
{
    [XmlElement] public DateTime ExportDate { get; set; }
    [XmlElement] public DateTime SalesDate { get; set; }
    [XmlElement] public int TotalSales { get; set; }
    [XmlElement] public decimal TotalRevenue { get; set; }
    [XmlElement] public decimal TotalTax { get; set; }
    [XmlArray("Sales")]
    [XmlArrayItem("Sale")] public List<XmlSaleSummary> Sales { get; set; } = new();
}

public class XmlSaleSummary
{
    [XmlElement] public string InvoiceNumber { get; set; } = string.Empty;
    [XmlElement] public DateTime SaleDate { get; set; }
    [XmlElement] public decimal SubTotal { get; set; }
    [XmlElement] public decimal TaxAmount { get; set; }
    [XmlElement] public decimal GrandTotal { get; set; }
    [XmlElement] public string PaymentMethod { get; set; } = string.Empty;
    [XmlElement] public string? CashierName { get; set; }
    [XmlElement] public int ItemCount { get; set; }
}

[XmlRoot("Products")]
public class XmlProductList
{
    [XmlElement] public DateTime ExportDate { get; set; }
    [XmlElement] public int TotalProducts { get; set; }
    [XmlArray("ProductList")]
    [XmlArrayItem("Product")] public List<XmlProduct> Products { get; set; } = new();
}

public class XmlProduct
{
    [XmlElement] public int Id { get; set; }
    [XmlElement] public string Name { get; set; } = string.Empty;
    [XmlElement] public string? SKU { get; set; }
    [XmlElement] public string? Barcode { get; set; }
    [XmlElement] public string CategoryName { get; set; } = string.Empty;
    [XmlElement] public decimal PurchasePrice { get; set; }
    [XmlElement] public decimal SellingPrice { get; set; }
    [XmlElement] public int StockQuantity { get; set; }
    [XmlElement] public int LowStockThreshold { get; set; }
}

[XmlRoot("Backup")]
public class XmlBackup
{
    [XmlElement] public DateTime BackupDate { get; set; }
    [XmlElement] public string Version { get; set; } = string.Empty;
    [XmlArray("Categories")]
    [XmlArrayItem("Category")] public List<Category> Categories { get; set; } = new();
    [XmlArray("Products")]
    [XmlArrayItem("Product")] public List<Product> Products { get; set; } = new();
    [XmlArray("Sales")]
    [XmlArrayItem("Sale")] public List<Sale> Sales { get; set; } = new();
}
