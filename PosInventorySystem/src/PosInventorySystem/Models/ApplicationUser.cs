using Microsoft.AspNetCore.Identity;

namespace PosInventorySystem.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public string? EmployeeId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<Sale>? Sales { get; set; }
}
