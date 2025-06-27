using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Gun_Site.Areas.Identity.Data;

namespace Gun_Site.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public void OnGet()
        {
            // Optionally, you could add logic to display a confirmation or logout page
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Perform logout
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            // Redirect to home page after logout
            return RedirectToPage("/Index");
        }
    }
}
