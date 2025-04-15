/*
Task :
User Information Source
o Store user login information in a file named users.json located under
wwwroot/data/.
o You must define a User class (Models/User.cs) to match the structure of this JSON
file.
o Below is the UML representation of the User class:
+--------------------+
| User |
+--------------------+
| - Username : string|
| - Password : string|
| - Role : string|
| - IsActive : bool |
| - CreatedAt: DateTime |
+--------------------+
Login Functionality
o When the login form is submitted, read the users from the JSON file.
o Check whether the given credentials match an active user in the list.
Session and Cookies
o Upon successful login:
▪ Generate a simple token
▪ Store the following in the session:
▪ username
▪ token
▪ session_id (use HttpContext.Session.Id)
▪ Store the same values in cookies using the following cookie settings:
▪ Expires in 30 minutes
▪ HttpOnly = true
▪ Secure = true
▪ SameSite = Strict
Access Control
o On all protected pages, check whether the token, username, and session_id from
cookies match those in the session.
o If both token and username values match between the session and cookie, then
you may consider the login valid.
o If the check fails, use errors and warnings to say “username or password is
incorrect.” Or something like this message.
Logout
o Create a logout button that clears the session and removes all cookies related to
login.
o Upon successful logout, the user should be redirected to the login page.
Redirection after Login
o Upon successful login, the user should be redirected to the table page you
implemented last week. This redirection should be implemented in the OnPostAsync
method inside Login.cshtml.cs.
For Cookies :
Example of Setting a cookie :
CookieOptions options = new CookieOptions
{
   Expires = DateTime.UtcNow.AddMinutes(20),
  HttpOnly = true,
  Secure = true,
  SameSite = SameSiteMode.Strict
};

Response.Cookies.Append("favoriteColor", "blue", options);

Example of Reading from cookie
CookieOptions options = new CookieOptions
{
   Expires = DateTime.UtcNow.AddMinutes(20),
  HttpOnly = true,
  Secure = true,
  SameSite = SameSiteMode.Strict
};

Response.Cookies.Append("favoriteColor", "blue", options);
(source code provided)



Gives error as "InvalidOperationException: Session has not been configured for this application or request." for string? sessionUsername = HttpContext.Session.GetString("username");



(source code provided)
Do this:
1- When i do "dotnet run", it needs to open Login page first. Add some if structure at OnGet() to check if authenticated or not. If needed, delete repetitive code from other files.



You completed task1. Now your second task is to bring my table back. I dont know what happened but it doesnt display my table now



Okay now you completed task2. Your third task is to complete cookies task. When admin enters the page, it needs to add details on inspect-application-cookies settings



Do this for cookies:
Upon successful login:
▪ Generate a simple token
▪ Store the following in the session:
▪ username
▪ token
▪ session_id (use HttpContext.Session.Id)
▪ Store the same values in cookies using the following cookie settings:
▪ Expires in 30 minutes
▪ HttpOnly = true
▪ Secure = true
▪ SameSite = Strict
*/



using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Reflection;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MyRazorApp.Pages;

public class LoginModel : PageModel
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
        // Validate user credentials
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

        // Generate security values
        var token = Guid.NewGuid().ToString();
        var sessionId = HttpContext.Session.Id;

        // Store values in session
        HttpContext.Session.SetString("username", user.Username);
        HttpContext.Session.SetString("token", token);
        HttpContext.Session.SetString("session_id", sessionId);
        await HttpContext.Session.CommitAsync(); // Ensure session ID is finalized

        // Configure cookie settings
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.UtcNow.AddMinutes(30),
            HttpOnly = true,
            Secure = true, // Requires HTTPS in production
            SameSite = SameSiteMode.Strict
        };

        // Set cookies
        Response.Cookies.Append("username", user.Username, cookieOptions);
        Response.Cookies.Append("token", token, cookieOptions);
        Response.Cookies.Append("session_id", sessionId, cookieOptions);

        return RedirectToPage("/Index");
    }
    catch (Exception ex)
    {
        // Log error here
        ErrorMessage = "An error occurred during login";
        return Page();
    }
}
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}