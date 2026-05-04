using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PosInventorySystem.Models;

public class Product
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? SKU { get; set; }
    
    [StringLength(500)]
    public string? Barcode { get; set; }
    
    [StringLength(500)]
    public string? QRCode { get; set; }
    
    public string? Description { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchasePrice { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal SellingPrice { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DiscountPercent { get; set; }
    
    public int StockQuantity { get; set; }
    public int LowStockThreshold { get; set; } = 10;
    
    public bool IsActive { get; set; } = true;
    public bool TrackInventory { get; set; } = true;
    
    public int CategoryId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public virtual Category? Category { get; set; }
    public virtual ICollection<SaleItem>? SaleItems { get; set; }
}
