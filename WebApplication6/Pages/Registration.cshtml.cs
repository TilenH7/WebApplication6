using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication6.Models;
using System.Linq;

namespace WebApplication6.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty] public string Ime { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }

        // 🔹 novo
        [BindProperty]
        public UserRole Role { get; set; } = UserRole.Uporabnik;

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            Ime = Ime?.Trim();
            Email = Email?.Trim();

            if (string.IsNullOrWhiteSpace(Ime) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Ime, e-mail in geslo so obvezna polja.";
                return Page();
            }

            if (FakeUserDb.Users.Any(u => u.Email.Equals(Email, StringComparison.OrdinalIgnoreCase)))
            {
                ErrorMessage = "Uporabnik s tem e-mailom že obstaja.";
                return Page();
            }

            if (FakeUserDb.Users.Any(u => u.Username != null &&
                                          u.Username.Equals(Ime, StringComparison.OrdinalIgnoreCase)))
            {
                ErrorMessage = "To uporabniško ime je že uporabljeno.";
                return Page();
            }

            FakeUserDb.Users.Add(new AppUser
            {
                Username = Ime,
                Ime = Ime,
                Email = Email,
                Password = Password,
                Role = Role
            });

            SuccessMessage = "Registracija uspešna. Zdaj se lahko prijaviš.";
            return Page();
        }

    }
}
