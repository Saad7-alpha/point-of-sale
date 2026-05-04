using Microsoft.EntityFrameworkCore;
using PosInventorySystem.Data;
using PosInventorySystem.Models;

namespace PosInventorySystem.Services;

public class SalesService : ISalesService
{
    private readonly ApplicationDbContext _context;
    private readonly decimal _taxRate;

    public SalesService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _taxRate = configuration.GetValue<decimal>("AppSettings:VATRate");
    }

    public async Task<Sale> CreateSaleAsync(Sale sale, List<SaleItem> items)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            // Generate invoice number
            sale.InvoiceNumber = await GenerateInvoiceNumberAsync();
            sale.SaleDate = DateTime.UtcNow;
            sale.TaxPercentage = _taxRate;
            
            // Calculate totals
            decimal subTotal = 0;
            foreach (var item in items)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Product with ID {item.ProductId} not found");
                
                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"Insufficient stock for product: {product.Name}");
                
                item.UnitPrice = product.SellingPrice;
                item.DiscountAmount = product.DiscountPercent.HasValue 
                    ? item.UnitPrice * (product.DiscountPercent.Value / 100) * item.Quantity 
                    : 0;
                
                decimal itemSubtotal = (item.UnitPrice * item.Quantity) - item.DiscountAmount;
                item.TaxAmount = itemSubtotal * (_taxRate / 100);
                item.TotalAmount = itemSubtotal + item.TaxAmount;
                
                subTotal += itemSubtotal;
                
                // Update stock
                if (product.TrackInventory)
                {
                    product.StockQuantity -= item.Quantity;
                    product.UpdatedAt = DateTime.UtcNow;
                }
            }
            
            sale.SubTotal = subTotal;
            sale.TaxAmount = items.Sum(i => i.TaxAmount);
            sale.GrandTotal = sale.SubTotal + sale.TaxAmount;
            
            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            
            // Add sale items
            foreach (var item in items)
            {
                item.SaleId = sale.Id;
                _context.SaleItems.Add(item);
            }
            
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return sale;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Sale?> GetSaleByIdAsync(int id)
    {
        return await _context.Sales
            .Include(s => s.Cashier)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Sale?> GetSaleByInvoiceNumberAsync(string invoiceNumber)
    {
        return await _context.Sales
            .Include(s => s.Cashier)
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .FirstOrDefaultAsync(s => s.InvoiceNumber == invoiceNumber);
    }

    public async Task<List<Sale>> GetAllSalesAsync()
    {
        return await _context.Sales
            .Include(s => s.Cashier)
            .Include(s => s.SaleItems)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<List<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Include(s => s.Cashier)
            .Include(s => s.SaleItems)
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate.AddDays(1).AddSeconds(-1))
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<List<Sale>> GetTodaySalesAsync()
    {
        var today = DateTime.UtcNow.Date;
        return await _context.Sales
            .Include(s => s.Cashier)
            .Include(s => s.SaleItems)
            .Where(s => s.SaleDate >= today && s.SaleDate < today.AddDays(1))
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        IQueryable<Sale> query = _context.Sales.Where(s => !s.IsRefunded);
        
        if (startDate.HasValue)
            query = query.Where(s => s.SaleDate >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(s => s.SaleDate <= endDate.Value);
        
        return await query.SumAsync(s => s.GrandTotal);
    }

    public async Task<int> GetTotalTransactionsCountAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        IQueryable<Sale> query = _context.Sales.Where(s => !s.IsRefunded);
        
        if (startDate.HasValue)
            query = query.Where(s => s.SaleDate >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(s => s.SaleDate <= endDate.Value);
        
        return await query.CountAsync();
    }

    public async Task<bool> ProcessRefundAsync(int saleId)
    {
        var sale = await _context.Sales
            .Include(s => s.SaleItems)
            .FirstOrDefaultAsync(s => s.Id == saleId);
        
        if (sale == null || sale.IsRefunded)
            return false;
        
        sale.IsRefunded = true;
        sale.RefundDate = DateTime.UtcNow;
        
        // Restore stock
        foreach (var item in sale.SaleItems)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product != null && product.TrackInventory)
            {
                product.StockQuantity += item.Quantity;
                product.UpdatedAt = DateTime.UtcNow;
            }
        }
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var today = DateTime.UtcNow;
        var prefix = $"INV-{today:yyyyMMdd}-";
        
        var lastInvoice = await _context.Sales
            .Where(s => s.InvoiceNumber.StartsWith(prefix))
            .OrderByDescending(s => s.InvoiceNumber)
            .FirstOrDefaultAsync();
        
        int nextNumber = 1;
        if (lastInvoice != null)
        {
            var lastNumber = lastInvoice.InvoiceNumber.Substring(prefix.Length);
            if (int.TryParse(lastNumber, out int parsedNumber))
            {
                nextNumber = parsedNumber + 1;
            }
        }
        
        return $"{prefix}{nextNumber:D4}";
    }
}
