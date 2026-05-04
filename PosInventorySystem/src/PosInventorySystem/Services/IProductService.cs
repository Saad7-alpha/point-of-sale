namespace PosInventorySystem.Services;

public interface IProductService
{
    Task<List<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(Product product);
    Task<Product?> UpdateProductAsync(int id, Product product);
    Task<bool> DeleteProductAsync(int id);
    Task<List<Product>> GetLowStockProductsAsync(int threshold);
    Task<Product?> GetProductByBarcodeAsync(string barcode);
    Task<List<Product>> SearchProductsAsync(string searchTerm);
    Task UpdateStockAsync(int productId, int quantityChange);
}
