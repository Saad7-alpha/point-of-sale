namespace PosInventorySystem.Services;

public interface IXmlExportService
{
    Task<string> ExportInvoiceToXmlAsync(int saleId);
    Task<string> ExportDailySalesToXmlAsync(DateTime date);
    Task<string> ExportProductsToXmlAsync();
    Task<string> ExportBackupToXmlAsync();
    string SaveXmlToFile(string xmlContent, string fileName, string folder = "Exports");
}
