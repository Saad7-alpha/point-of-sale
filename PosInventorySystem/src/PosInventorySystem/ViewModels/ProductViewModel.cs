using System.ComponentModel.DataAnnotations;

namespace PosInventorySystem.ViewModels;

public class ProductViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(200, ErrorMessage = "Name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? SKU { get; set; }
    
    [StringLength(500)]
    public string? Barcode { get; set; }
    
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Purchase price is required")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Purchase Price")]
    public decimal PurchasePrice { get; set; }
    
    [Required(ErrorMessage = "Selling price is required")]
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Selling Price")]
    public decimal SellingPrice { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    [Display(Name = "Discount %")]
    public decimal? DiscountPercent { get; set; }
    
    [Required(ErrorMessage = "Stock quantity is required")]
    [Display(Name = "Stock Quantity")]
    public int StockQuantity { get; set; }
    
    [Display(Name = "Low Stock Threshold")]
    public int LowStockThreshold { get; set; } = 10;
    
    public bool IsActive { get; set; } = true;
    public bool TrackInventory { get; set; } = true;
    
    [Required(ErrorMessage = "Category is required")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }
    
    public string? CategoryName { get; set; }
    public IEnumerable<SelectListItem>? Categories { get; set; }
}
