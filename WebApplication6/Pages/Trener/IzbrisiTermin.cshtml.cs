using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;

namespace WebApplication6.Pages.Trener
{
    public class IzbrisiTerminModel : PageModel
    {
        public TerminVadbe Termin { get; set; }
        public string ErrorMessage { get; set; }

        public IActionResult OnGet(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (HttpContext.Session.GetString("Role") != UserRole.Trener.ToString())
                return RedirectToPage("/Login");

            Termin = FakeTerminDb.Termini
                .FirstOrDefault(t => t.Id == id && t.TrenerUsername == username);

            if (Termin == null)
            {
                ErrorMessage = "Termin ni bil najden ali ni tvoj.";
            }

            return Page();
        }

        public IActionResult OnPost(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (HttpContext.Session.GetString("Role") != UserRole.Trener.ToString())
                return RedirectToPage("/Login");

            var termin = FakeTerminDb.Termini
                .FirstOrDefault(t => t.Id == id && t.TrenerUsername == username);

            if (termin == null)
            {
                ErrorMessage = "Termin ni bil najden.";
                return Page();
            }

            // 🔥 izbriši vse rezervacije na ta termin
            FakeRezervacijeDb.Rezervacije.RemoveAll(r => r.TerminId == id);

            // 🔥 izbriši termin
            FakeTerminDb.Termini.Remove(termin);

            // uporabniki ga zato ne vidijo več:
            // - v iskalniku
            // - v "mojih rezervacijah"
            return RedirectToPage("/Trener/Termini");
        }
    }
}
