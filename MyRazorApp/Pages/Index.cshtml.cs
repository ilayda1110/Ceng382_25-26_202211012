using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;

namespace MyRazorApp.Pages;

public class IndexModel : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; } = new InputModel();

    [TempData]
    public string ErrorMessage { get; set; } = string.Empty;

    public class InputModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    // Login.cshtml.cs
public async Task<IActionResult> OnPost()
{
        try
    {
        // Validate user credentials (existing code)
        var usersFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "data", "users.json");
        if (!System.IO.File.Exists(usersFilePath))
        {
            ErrorMessage = "Server configuration error";
            return Page();
        }

        var usersJson = await System.IO.File.ReadAllTextAsync(usersFilePath);
        var users = JsonSerializer.Deserialize<List<User>>(usersJson);
        var user = users?.FirstOrDefault(u => 
            u.Username == Input.Username && 
            u.Password == Input.Password &&
            u.IsActive);

        if (user == null)
        {
            ErrorMessage = "Invalid login attempt";
            return Page();
        }

        // Create claims for the authenticated user
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        // Create an authentication cookie
        var claimsIdentity = new ClaimsIdentity(
            claims, 
            CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
            });

        return RedirectToPage("/Dashboard");
    }
    catch (Exception)
        {
        ErrorMessage = "An error occurred during login";
        return Page();
    }
}
}