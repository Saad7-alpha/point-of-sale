using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PosInventorySystem.Models;

public class SaleItem
{
    [Key]
    public int Id { get; set; }
    
    public int SaleId { get; set; }
    public int ProductId { get; set; }
    
    public int Quantity { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
    
    // Navigation properties
    public virtual Sale? Sale { get; set; }
    public virtual Product? Product { get; set; }
}
