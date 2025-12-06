using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;

namespace WebApplication6.Pages.Uporabnik
{
    public class PrekliciModel : PageModel
    {
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public TerminVadbe Termin { get; set; }

        public IActionResult OnGet(int terminId)
        {
            if (HttpContext.Session.GetString("Role") != UserRole.Uporabnik.ToString())
                return RedirectToPage("/Login");

            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
                return RedirectToPage("/Login");

            Termin = FakeTerminDb.Termini.FirstOrDefault(t => t.Id == terminId);
            if (Termin == null)
            {
                ErrorMessage = "Termin ne obstaja.";
                return Page();
            }

            var rezervacija = FakeRezervacijeDb.Rezervacije
                .FirstOrDefault(r => r.TerminId == terminId && r.UporabnikUsername == username);

            if (rezervacija == null)
            {
                ErrorMessage = "Za ta termin nimaš rezervacije.";
                return Page();
            }

            // 🔹 odstranimo rezervacijo
            FakeRezervacijeDb.Rezervacije.Remove(rezervacija);

            // 🔹 sprostimo mesto
            if (Termin.ZasedenaMesta > 0)
                Termin.ZasedenaMesta--;

            SuccessMessage = "Rezervacija uspešno preklicana.";
            return Page();
        }
    }
}
