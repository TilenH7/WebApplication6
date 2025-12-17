using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;

namespace WebApplication6.Pages.Uporabnik
{
    public class UrediProfilModel : PageModel
    {
        [BindProperty] public string Ime { get; set; }
        [BindProperty] public string Priimek { get; set; }
        [BindProperty] public string Lokacija { get; set; }
        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Telefon { get; set; }

        // geslo: staro + novo (novo je optional)
        [BindProperty] public string StaroGeslo { get; set; }
        [BindProperty] public string NovoGeslo { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username) || role != UserRole.Uporabnik.ToString())
                return RedirectToPage("/Login");

            var user = FakeUserDb.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return RedirectToPage("/Login");

            // ? napolni trenutne podatke
            Ime = user.Ime;
            Priimek = user.Priimek;
            Lokacija = user.Lokacija;
            Email = user.Email;
            Telefon = user.Telefon;

            return Page();
        }

        public IActionResult OnPost()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username) || role != UserRole.Uporabnik.ToString())
                return RedirectToPage("/Login");

            var user = FakeUserDb.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return RedirectToPage("/Login");

            // ? validacija email
            if (!string.IsNullOrWhiteSpace(Email))
            {
                var emailOk = Regex.IsMatch(Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                if (!emailOk)
                {
                    ErrorMessage = "Napaèen format e-pošte.";
                    return Page();
                }
            }

            // ? validacija gesla (samo èe user želi menjat)
            if (!string.IsNullOrWhiteSpace(NovoGeslo))
            {
                if (NovoGeslo.Length < 6)
                {
                    ErrorMessage = "Novo geslo mora imeti vsaj 6 znakov.";
                    return Page();
                }

                // preveri staro geslo
                if (StaroGeslo != user.Password)
                {
                    ErrorMessage = "Staro geslo ni pravilno.";
                    return Page();
                }

                // ? spremeni geslo
                user.Password = NovoGeslo;
            }

            // ? shrani ostale spremembe
            user.Ime = Ime;
            user.Priimek = Priimek;
            user.Lokacija = Lokacija;
            user.Email = Email;
            user.Telefon = Telefon;

            SuccessMessage = "Profil je uspešno posodobljen.";
            return Page();
        }
    }
}
