using System.ComponentModel.DataAnnotations;

namespace PosInventorySystem.ViewModels;

public class POSCartViewModel
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal Discount { get; set; }
    public decimal Subtotal => (Price * Quantity) - Discount;
}

public class POSSessionViewModel
{
    public List<POSCartViewModel> Items { get; set; } = new();
    public decimal SubTotal => Items.Sum(i => i.Price * i.Quantity);
    public decimal TotalDiscount => Items.Sum(i => i.Discount);
    public decimal TaxableAmount => SubTotal - TotalDiscount;
    public decimal TaxAmount => TaxableAmount * (TaxRate / 100);
    public decimal GrandTotal => TaxableAmount + TaxAmount;
    public decimal TaxRate { get; set; } = 18;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = "Cash";
    public string? Notes { get; set; }
}

public class CheckoutViewModel
{
    public List<POSCartViewModel> Items { get; set; } = new();
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = "Cash";
    public string? Notes { get; set; }
}
