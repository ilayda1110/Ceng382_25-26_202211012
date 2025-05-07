using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MyRazorApp.Pages;

public class LogoutModel : PageModel
{
    // Logout.cshtml.cs
public async Task<IActionResult> OnGetAsync()
{
        // Clear the authentication cookie
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    // Clear session and custom cookies (optional)
    HttpContext.Session.Clear();
    Response.Cookies.Delete("username");
    Response.Cookies.Delete("token");
    Response.Cookies.Delete("session_id");

    return RedirectToPage("/Index");
}
}