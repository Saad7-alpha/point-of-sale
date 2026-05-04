namespace PosInventorySystem.Services;

public interface ISalesService
{
    Task<Sale> CreateSaleAsync(Sale sale, List<SaleItem> items);
    Task<Sale?> GetSaleByIdAsync(int id);
    Task<Sale?> GetSaleByInvoiceNumberAsync(string invoiceNumber);
    Task<List<Sale>> GetAllSalesAsync();
    Task<List<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<Sale>> GetTodaySalesAsync();
    Task<decimal> GetTotalRevenueAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<int> GetTotalTransactionsCountAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<bool> ProcessRefundAsync(int saleId);
    Task<string> GenerateInvoiceNumberAsync();
}
