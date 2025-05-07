/*
I already created a login page (Index.cshtml.cs), dashboard page for adming login (Dashboard.cshtml.cs) and database connections. I will provide a tutorial for you but you dont have to follow all of the steps (dont use _LoginPartial or scaffolding for example sinc i already have a login page and i will use migrations) Our task:
This week, we'll be enhancing our application by adding user authentication and authorization capabilities through ASP.NET Core Identity. By the end of this session, you will learn how to:

Scaffold the ASP.NET Core Identity system into our existing application
Merge the Identity database schema with our existing Products database
Implement login and registration functionality
Secure our Products pages so that only authenticated users can perform CRUD operations
Step 1: Install Required Packages
Run these commands in your project directory:

dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Microsoft.AspNetCore.Identity.UI
Step 2: Update Your DbContext
Modify your existing DbContext class to inherit from IdentityDbContext instead of DbContext:

# Create ApplicationUser class (optional if you need to extend the default user)
dotnet new class -n ApplicationUser -o Models
Update the ApplicationUser.cs file:

using Microsoft.AspNetCore.Identity;

namespace ProductsApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Add custom user properties here if needed
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
Update your DbContext class:

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductsApp.Models;

namespace ProductsApp.Data
{
    public class ProductsDbContext : IdentityDbContext<ApplicationUser>
    {
        public ProductsDbContext(DbContextOptions<ProductsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Any custom model configurations for your Products table
        }
    }
}
Step 3: Update Program.cs
Update your Program.cs file to include Identity services:

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductsApp.Data;
using ProductsApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ProductsDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    // Configure identity options here
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ProductsDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
Step 4: Scaffold Identity Pages
Use the following command to scaffold Identity pages:

dotnet aspnet-codegenerator identity -dc ProductsApp.Data.ProductsDbContext --files "Account.Login;Account.Logout;Account.Register;Account.Manage.Index"
This command will create the basic login, logout, register, and manage account pages in your application.

Step 5: Create a Migration for Identity Tables
First, update your ProductsDbContext class to properly configure the Identity entity keys in the OnModelCreating method:
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Configure Identity tables with proper keys
    modelBuilder.Entity<IdentityUserLogin<string>>()
        .HasKey(l => new { l.LoginProvider, l.ProviderKey });

    modelBuilder.Entity<IdentityUserRole<string>>()
        .HasKey(r => new { r.UserId, r.RoleId });

    modelBuilder.Entity<IdentityUserToken<string>>()
        .HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

    // Keep your existing Products table configuration
    modelBuilder.Entity<Product>(entity =>
    {
        entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CDBA11BBF0");

        entity.Property(e => e.Category).HasMaxLength(50);
        entity.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");
        entity.Property(e => e.Description).HasMaxLength(500);
        entity.Property(e => e.InStock).HasDefaultValue(true);
        entity.Property(e => e.Name).HasMaxLength(100);
        entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
    });
}
Create a migration to add Identity tables to your database:

dotnet ef migrations add AddIdentityTablesOnly
Create a script-only migration (which lets you see the SQL without running it):

 dotnet ef migrations script
Edit the SQL script to remove any statements related to creating or altering the Products table.

Run the modified SQL script directly against your database using SQL Server Management Studio

This will merge the Identity tables with your existing Products database.

NOTE: Look for these lines in your Program.cs and make sure you only have ONE of them (not both):

// Make sure you have only ONE of these methods, not both:
 builder.Services.AddDefaultIdentity()... // Delete this configuration
 builder.Services.AddIdentity()...
Step 6: Add Authorization to Product Pages
Update your Product page models to require authentication:

For each of your Product page models (Index.cshtml.cs, Create.cshtml.cs, Edit.cshtml.cs, Delete.cshtml.cs, Details.cshtml.cs), add the [Authorize] attribute at the top of the class:

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProductsApp.Data;
using ProductsApp.Models;

namespace ProductsApp.Pages.Products
{
    [Authorize]
    public class IndexModel : PageModel
    {
        // Existing code...
    }
}
Step 7: Update _Layout.cshtml
Update your _Layout.cshtml file to include the login partial:

<!-- Find the navbar section and add this line -->
<div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
    <ul class="navbar-nav flex-grow-1">
        <li class="nav-item">
            <a class="nav-link text-dark" asp-area="" asp-page="/Index">Home</a>
        </li>
        <li class="nav-item">
            <a class="nav-link text-dark" asp-area="" asp-page="/Privacy">Privacy</a>
        </li>
        <li class="nav-item">
            <a class="nav-link text-dark" asp-area="" asp-page="/Products/Index">Products</a>
        </li>
    </ul>
    <partial name="_LoginPartial" />
</div>
Step 8 (Optional): Associate Products with Users
If you want to track which user created which product, update your Product model:

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    // Other existing properties

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}
Then update your Create.cshtml.cs file to assign the current user:

using System.Security.Claims;

// In the OnPostAsync method
public async Task<IActionResult> OnPostAsync()
{
    if (!ModelState.IsValid)
    {
        return Page();
    }

    // Add the current user ID to the product
    Product.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    _context.Products.Add(Product);
    await _context.SaveChangesAsync();

    return RedirectToPage("./Index");
}
Create a migration for this change:

dotnet ef migrations add AddUserToProducts
dotnet ef database update
Step 9 (Optional): Add Role-Based Authorization
If you want to implement role-based authorization (e.g., only admins can delete products):

# Add to Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});
Then update specific page models:

// In Delete.cshtml.cs
[Authorize(Roles = "Admin")]
public class DeleteModel : PageModel
{
    // Existing code...
}
To seed an admin user and role:

// Create a utility class to seed the database
using Microsoft.AspNetCore.Identity;
using ProductsApp.Models;

namespace ProductsApp.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Create Admin role if it doesn't exist
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Create Admin user if it doesn't exist
            const string adminEmail = "admin@example.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "AdminPass1!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
Call this seed method from Program.cs:

// Add after app.Build()
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}
Testing the Application
After implementing these changes, run your application:

dotnet run
Navigate to your application (typically https://localhost:5001 or similar). You should now:

See Register/Login links in the header
Be redirected to the login page when trying to access the Products pages
Only be able to manage products after logging in
Conclusion
We've now successfully integrated ASP.NET Core Identity with our existing Products application using the CLI approach. This implementation:

Merges the Identity tables with our Products database
Restricts access to Products pages to authenticated users only
Provides login, logout, and registration functionality
Optionally associates products with the users who created them
Optionally implements role-based authorization

I already have these:
Index.cshtml.cs - Index.cshtml
Dashboard.csthml.cs - Dashboard.cshtml
Program.cs
User.cs
users.json (u may want to not use this since we are gonna seed initial admin at start to database)
SchoolDbContext.cs



I already have Logout.cshtml.cs btw and let me provide you my files.
(source code provided)

I already had a migration named as "InitialCreate" but i deleted my database. How do i run the migrations now?

I cant login as admin right now. Also my 100 sentetic data isnt working. Im looking at dbo.Classes and dbo.AspNetUsers

Here is my SeedData.cs
(source code provided)

Before i change migrations I want to provide the error i get when i run the program with InitialCreate.cs migration:
fail: Program[0]
      An error occurred seeding the DB
      Microsoft.EntityFrameworkCore.DbUpdateException: An error occurred while saving the entity changes. See the inner exception for details.  
       ---> Microsoft.Data.SqlClient.SqlException (0x80131904): Cannot insert the value NULL into column 'FirstName', table 'SchoolDb.dbo.AspNetUsers'; column does not allow nulls. INSERT fails.
      The statement has been terminated.

My ApplicationUser shouldnt have IsActive btw. And i still cant see admin and my 100 data in database
(source code provided)

Okay now its working. But when i try to log out, it gave error as "InvalidOperationException: No sign-out authentication handler is registered for the scheme 'Cookies'. The registered sign-out schemes are: Identity.Application, Identity.External, Identity.TwoFactorRememberMe, Identity.TwoFactorUserId. Did you forget to call AddAuthentication().AddCookie("Cookies",...)?"
(source code provided)

When i press logout, my link is still http://localhost:5254/Logout but the page is empty.
(source code provided)

I have my logout button in dashboard thats why here is my Dashboard.cshtml:
(source code provided)
*/
using Microsoft.AspNetCore.Identity;

namespace ProductsApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}