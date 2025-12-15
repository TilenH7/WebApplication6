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
            if (string.IsNullOrWhiteSpace(Ime) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Ime, e-mail in geslo so obvezna polja.";
                return Page();
            }

            if (FakeUserDb.Users.Any(u => u.Email.ToLower() == Email.ToLower()))
            {
                ErrorMessage = "Uporabnik s tem e-mailom že obstaja.";
                return Page();
            }

            FakeUserDb.Users.Add(new AppUser
            {
                Username = Email,
                Ime = Ime,
                Email = Email,
                Password = Password,
                Role = Role   // ⚡️ vzame iz izbranega v formu
            });

            SuccessMessage = "Registracija uspešna. Zdaj se lahko prijaviš.";
            return Page();
        }
    }
}
