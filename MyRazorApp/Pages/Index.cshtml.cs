// Pages/Index.cshtml.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyRazorApp.Models;
using ProductsApp.Models;

namespace MyRazorApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string ErrorMessage { get; set; } = string.Empty;

        public class InputModel
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public IndexModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                var result = await _signInManager.PasswordSignInAsync(
                    Input.Username,
                    Input.Password,
                    isPersistent: true,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToPage("/Dashboard");
                }

                ErrorMessage = "Invalid login attempt";
                return Page();
            }
            catch (Exception)
            {
                ErrorMessage = "An error occurred during login";
                return Page();
            }
        }
    }
}