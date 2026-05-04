# POS Inventory System - Enterprise Edition

A complete, production-ready Point of Sale (POS) and Inventory Management System built with ASP.NET Core MVC (.NET 8), SQL Server, and Bootstrap 5.

**Live Developed By Saad Tech**

---

## 🚀 FEATURES

### Core Modules
- ✅ **Authentication & Authorization** - ASP.NET Core Identity with Admin/Cashier roles
- ✅ **Product Management** - Full CRUD with barcode/QR code generation
- ✅ **POS Billing System** - Lightning-fast cashier interface
- ✅ **Invoice System** - FBR Pakistan compliant with VAT (18%)
- ✅ **Sales Reporting** - Daily, monthly, date-range reports
- ✅ **Stock Management** - Real-time inventory tracking with low-stock alerts
- ✅ **XML Export** - Invoice, daily sales, and backup exports
- ✅ **Barcode/QR Generation** - ZXing.Net integration

### Business Features
- Thermal printer-friendly invoice format
- Configurable VAT tax system
- Multi-payment method support (Cash, Card, Online)
- Customer information tracking
- Discount support per product
- Profit estimation ready

---

## 🛠️ TECHNICAL REQUIREMENTS

### Prerequisites
- **Visual Studio 2022/2025** (with ASP.NET Core development workload)
- **.NET 8.0 SDK**
- **SQL Server 2016+** (Express or higher)
- **SQL Server Management Studio (SSMS)**

### NuGet Packages
```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
<PackageReference Include="Microsoft.VisualStudio.Web.CodeGeneration.Design" Version="8.0.0" />
<PackageReference Include="ZXing.Net" Version="0.16.9" />
<PackageReference Include="ZXing.Net.Bindings.Windows.Compatibility" Version="0.16.12" />
<PackageReference Include="System.Drawing.Common" Version="8.0.0" />
```

---

## 📥 INSTALLATION STEPS

### Step 1: Open Solution in Visual Studio
1. Launch **Visual Studio 2022/2025**
2. Click **File → Open → Project/Solution**
3. Navigate to `PosInventorySystem.sln` and open it

### Step 2: Configure Database Connection
Edit `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=PosInventoryDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

**For named SQL Server instance:**
```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=PosInventoryDB;Trusted_Connection=True;TrustServerCertificate=True;"
```

### Step 3: Restore NuGet Packages
- Right-click solution → **Restore NuGet Packages**
- Or run in Package Manager Console:
```
Update-Package
```

### Step 4: Apply Entity Framework Migrations
Open **Package Manager Console** (Tools → NuGet Package Manager → Package Manager Console):

```powershell
# Enable migrations (if needed)
Add-Migration InitialCreate

# Update database
Update-Database
```

### Step 5: Build Solution
- Press **Ctrl+Shift+B** or
- Build → Build Solution

### Step 6: Run Application
- Press **F5** or click **IIS Express** (Green play button)
- Application will launch in your default browser

---

## 🔐 DEFAULT LOGIN CREDENTIALS

### Admin Account
- **Email:** admin@pos.com
- **Password:** Admin@123
- **Role:** Admin (Full access)

### Cashier Account
- **Email:** cashier@pos.com
- **Password:** Cashier@123
- **Role:** Cashier (POS billing only)

---

## 📊 DATABASE SCHEMA

### Tables Created
1. **AspNetUsers** - User accounts (extended with ApplicationUser)
2. **AspNetRoles** - Roles (Admin, Cashier)
3. **Categories** - Product categories
4. **Products** - Product inventory with barcodes
5. **Sales** - Invoice headers
6. **SaleItems** - Invoice line items

### Relationships
- Users → Sales (One-to-Many)
- Categories → Products (One-to-Many)
- Sales → SaleItems (One-to-Many)
- Products → SaleItems (One-to-Many)

---

## 🎯 KEY PAGES & NAVIGATION

### Sidebar Navigation
1. **Dashboard** - Overview with statistics and alerts
2. **Products** - Product management (CRUD operations)
3. **POS Billing** - Fast checkout interface
4. **Reports** - Sales reports and exports (Admin only)

### Quick Actions
- **Scan Barcode** - Use barcode scanner at POS screen
- **Print Invoice** - Auto-print after sale completion
- **Export XML** - Backup and reporting

---

## ⚡ PERFORMANCE OPTIMIZATIONS

- Async/await throughout for non-blocking operations
- Database indexing on frequently queried fields
- Client-side cart management for instant POS updates
- Minimal database round-trips
- Cached category lists
- Efficient stock deduction with transactions

---

## 🔒 SECURITY FEATURES

- Password hashing with ASP.NET Core Identity
- Role-based authorization
- CSRF protection on all forms
- SQL injection prevention via EF Core
- Session management with secure cookies
- Input validation on all models

---

## 🖨️ INVOICE PRINTING

The system supports:
- **Thermal Printers** (58mm/80mm)
- **Standard Printers** (A4/A5)
- **PDF Export** (via browser print dialog)
- **Auto-print** option after checkout

Customize invoice footer in `Views/POS/Invoice.cshtml`

---

## 📦 XML EXPORT LOCATIONS

Exports are saved to: `wwwroot/Exports/`

- **Invoices:** `/Exports/Exports/Invoice_INV-*.xml`
- **Daily Sales:** `/Exports/Exports/DailySales_*.xml`
- **Products:** `/Exports/Exports/Products_*.xml`
- **Backups:** `/Exports/Exports/Backup_*.xml`

---

## 🛠️ CUSTOMIZATION GUIDE

### Change Company Information
Edit `appsettings.json`:
```json
"AppSettings": {
  "VATRate": 18,
  "CompanyName": "Your Company Name",
  "CompanyAddress": "Your Address",
  "CompanyPhone": "+92-XXX-XXXXXXX",
  "CompanyNTN": "NTN: XXX-XXXXXXX-X",
  "CurrencySymbol": "PKR"
}
```

### Modify Low Stock Threshold
Default is 10 units. Change in:
- `appsettings.json` → `LowStockThreshold`
- Or per-product in Product edit screen

### Add New Roles
Use ASP.NET Core Identity UserManager in code or SQL:
```sql
INSERT INTO AspNetRoles (Id, Name, NormalizedName) 
VALUES (NEWID(), 'Manager', 'MANAGER')
```

---

## 🐛 TROUBLESHOOTING

### Database Connection Error
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure Windows Authentication is enabled

### Migration Errors
```powershell
Remove-Database
Update-Database
```

### Login Issues
- Reset password via SQL or recreate users
- Check if user is active in database

### Barcode Not Generating
- Ensure `System.Drawing.Common` is installed
- Check write permissions on `wwwroot/barcodes/`

---

## 📞 SUPPORT

For commercial licensing, customization, or support:

**Saad Tech**
- Email: support@saadtech.com
- Website: www.saadtech.com

---

## 📄 LICENSE

This is a commercial-grade system. Contact Saad Tech for licensing terms.

---

## 🎉 GETTING STARTED CHECKLIST

- [ ] Install Visual Studio 2022/2025
- [ ] Install .NET 8.0 SDK
- [ ] Install SQL Server
- [ ] Clone/Open solution in Visual Studio
- [ ] Update connection string
- [ ] Run `Update-Database`
- [ ] Build solution (Ctrl+Shift+B)
- [ ] Run application (F5)
- [ ] Login with admin credentials
- [ ] Add your products
- [ ] Start billing!

---

**Live Developed By Saad Tech** © 2024
