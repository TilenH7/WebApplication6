using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication6.Models;
using System.Linq;


public class LoginModel : PageModel
{
    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string ErrorMessage { get; set; }

    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
    
        var user = FakeUserDb.Users
            .FirstOrDefault(u => u.Username == Username && u.Password == Password);

        if (user == null)
        {
            ErrorMessage = "Napaèno uporabniško ime ali geslo.";
            return Page();
        }



        // shrani v session
        HttpContext.Session.SetString("Username", user.Username);
        HttpContext.Session.SetString("Role", user.Role.ToString());

        // redirect glede na vlogo
        if (user.Role == UserRole.Trener)
            return RedirectToPage("/Trener/Index");

        return RedirectToPage("/Uporabnik/Index");
    }
}
