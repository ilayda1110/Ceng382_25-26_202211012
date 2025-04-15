using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MyRazorApp.Pages;

public class LogoutModel : PageModel
{
    // Logout.cshtml.cs
public IActionResult OnGet()
{
    HttpContext.Session.Clear();
    
    // Cookie removal with same options
    var cookieOptions = new CookieOptions
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict
    };
    
    Response.Cookies.Delete("username", cookieOptions);
    Response.Cookies.Delete("token", cookieOptions);
    Response.Cookies.Delete("session_id", cookieOptions);

    return RedirectToPage("/Login");
}
}