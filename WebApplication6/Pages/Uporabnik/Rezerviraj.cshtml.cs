using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;

namespace WebApplication6.Pages.Uporabnik
{
    public class RezervirajModel : PageModel
    {
        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }
        public TerminVadbe Termin { get; set; }

        public IActionResult OnGet(int terminId)
        {
            // samo uporabnik
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

            // preveri, èe je že rezerviral ta termin
            var existing = FakeRezervacijeDb.Rezervacije
                .FirstOrDefault(r => r.TerminId == terminId && r.UporabnikUsername == username);

            if (existing != null)
            {
                ErrorMessage = "Na ta termin si že prijavljen.";
                return Page();
            }

            // ustvari novo rezervacijo
            var rezervacija = new Rezervacija
            {
                Id = FakeRezervacijeDb.Rezervacije.Count + 1,
                TerminId = terminId,
                UporabnikUsername = username
            };

            FakeRezervacijeDb.Rezervacije.Add(rezervacija);

            SuccessMessage = "Uspešno si rezerviral termin.";
            return Page();
        }
    }
}
