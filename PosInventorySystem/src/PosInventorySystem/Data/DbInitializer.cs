using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PosInventorySystem.Models;

namespace PosInventorySystem.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context, IServiceProvider serviceProvider)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Create Roles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        
        if (!await roleManager.RoleExistsAsync("Cashier"))
        {
            await roleManager.CreateAsync(new IdentityRole("Cashier"));
        }

        // Create Admin User
        var adminEmail = "admin@pos.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                FullName = "System Administrator",
                EmployeeId = "EMP001",
                IsActive = true,
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(adminUser, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // Create Cashier User
        var cashierEmail = "cashier@pos.com";
        var cashierUser = await userManager.FindByEmailAsync(cashierEmail);
        if (cashierUser == null)
        {
            cashierUser = new ApplicationUser
            {
                UserName = "cashier",
                Email = cashierEmail,
                FullName = "John Cashier",
                EmployeeId = "EMP002",
                IsActive = true,
                EmailConfirmed = true
            };
            
            var result = await userManager.CreateAsync(cashierUser, "Cashier@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(cashierUser, "Cashier");
            }
        }

        // Seed Categories
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Electronics", Description = "Electronic devices and accessories" },
                new Category { Name = "Groceries", Description = "Food and daily essentials" },
                new Category { Name = "Clothing", Description = "Apparel and fashion items" },
                new Category { Name = "Home & Garden", Description = "Home improvement and garden supplies" },
                new Category { Name = "Sports", Description = "Sports equipment and accessories" }
            };
            
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Seed Products
        if (!context.Products.Any())
        {
            var electronicsCategory = context.Categories.FirstOrDefault(c => c.Name == "Electronics");
            var groceriesCategory = context.Categories.FirstOrDefault(c => c.Name == "Groceries");
            
            if (electronicsCategory != null && groceriesCategory != null)
            {
                var products = new List<Product>
                {
                    new Product 
                    { 
                        Name = "Wireless Mouse", 
                        SKU = "ELEC-001",
                        Barcode = "1234567890123",
                        SellingPrice = 2500,
                        PurchasePrice = 1500,
                        StockQuantity = 50,
                        LowStockThreshold = 10,
                        CategoryId = electronicsCategory.Id
                    },
                    new Product 
                    { 
                        Name = "USB Keyboard", 
                        SKU = "ELEC-002",
                        Barcode = "1234567890124",
                        SellingPrice = 3500,
                        PurchasePrice = 2000,
                        StockQuantity = 30,
                        LowStockThreshold = 5,
                        CategoryId = electronicsCategory.Id
                    },
                    new Product 
                    { 
                        Name = "HDMI Cable 2m", 
                        SKU = "ELEC-003",
                        Barcode = "1234567890125",
                        SellingPrice = 800,
                        PurchasePrice = 400,
                        StockQuantity = 100,
                        LowStockThreshold = 20,
                        CategoryId = electronicsCategory.Id
                    },
                    new Product 
                    { 
                        Name = "Rice 5kg", 
                        SKU = "GROC-001",
                        Barcode = "1234567890126",
                        SellingPrice = 1200,
                        PurchasePrice = 900,
                        StockQuantity = 200,
                        LowStockThreshold = 50,
                        CategoryId = groceriesCategory.Id
                    },
                    new Product 
                    { 
                        Name = "Sugar 1kg", 
                        SKU = "GROC-002",
                        Barcode = "1234567890127",
                        SellingPrice = 150,
                        PurchasePrice = 100,
                        StockQuantity = 500,
                        LowStockThreshold = 100,
                        CategoryId = groceriesCategory.Id
                    }
                };
                
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}
