namespace PosInventorySystem.ViewModels;

public class DashboardViewModel
{
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TodaySales { get; set; }
    public decimal TodayRevenue { get; set; }
    public int LowStockProducts { get; set; }
    public decimal MonthRevenue { get; set; }
    public int MonthTransactions { get; set; }
    public List<ProductViewModel>? LowStockItems { get; set; }
    public List<SaleSummaryViewModel>? RecentSales { get; set; }
}

public class SaleSummaryViewModel
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public decimal GrandTotal { get; set; }
    public string? CashierName { get; set; }
    public int ItemCount { get; set; }
}

public class SalesReportViewModel
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalTransactions { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalTax { get; set; }
    public decimal TotalDiscount { get; set; }
    public List<SaleDetailViewModel>? Sales { get; set; }
}

public class SaleDetailViewModel
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public string? CashierName { get; set; }
    public List<SaleItemViewModel>? Items { get; set; }
}

public class SaleItemViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
}
