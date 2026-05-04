using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PosInventorySystem.Models;

namespace PosInventorySystem.Models;

public class Sale
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string InvoiceNumber { get; set; } = string.Empty;
    
    public DateTime SaleDate { get; set; } = DateTime.UtcNow;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal SubTotal { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal TaxAmount { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal GrandTotal { get; set; }
    
    public decimal TaxPercentage { get; set; } = 18;
    
    public string PaymentMethod { get; set; } = "Cash"; // Cash, Card, Online
    
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? CustomerAddress { get; set; }
    
    public string? Notes { get; set; }
    
    public string CashierId { get; set; } = string.Empty;
    
    public bool IsCompleted { get; set; } = true;
    public bool IsRefunded { get; set; } = false;
    public DateTime? RefundDate { get; set; }
    
    // Navigation properties
    public virtual ApplicationUser? Cashier { get; set; }
    public virtual ICollection<SaleItem>? SaleItems { get; set; }
}
