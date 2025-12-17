using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;

namespace WebApplication6.Pages.Trener
{
    public class DodajTerminModel : PageModel
    {
        [BindProperty]
        public DateTime DatumInCas { get; set; }

        [BindProperty]
        public string Lokacija { get; set; }

        [BindProperty]
        public decimal Cena { get; set; }

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Role") != UserRole.Trener.ToString())
                return RedirectToPage("/Login");

            DatumInCas = DateTime.Now.Date.AddDays(1).AddHours(18);
            Cena = 10m;

            return Page();
        }

        public IActionResult OnPost()
        {
            if (HttpContext.Session.GetString("Role") != UserRole.Trener.ToString())
                return RedirectToPage("/Login");

            if (string.IsNullOrWhiteSpace(Lokacija))
            {
                ErrorMessage = "Lokacija je obvezna.";
                return Page();
            }

            if (DatumInCas < DateTime.Now)
            {
                ErrorMessage = "Datum in ura morata biti v prihodnosti.";
                return Page();
            }

            if (Cena <= 0)
            {
                ErrorMessage = "Cena mora biti večja od 0.";
                return Page();
            }

            var termin = new TerminVadbe
            {
                Id = FakeTerminDb.Termini.Count + 1,
                TrenerUsername = HttpContext.Session.GetString("Username"),
                DatumInCas = DatumInCas,
                Lokacija = Lokacija,
                Cena = Cena,
                TipVadbe = "Kardio",
                TrajanjeMin = 60,

            };

            FakeTerminDb.Termini.Add(termin);

            // če želiš pokazat message na naslednji strani:
            TempData["Success"] = "Termin je bil uspešno dodan.";

            return RedirectToPage("/Trener/Termini");

            // Če želiš takoj nazaj na seznam:
            // TempData["Success"] = "Termin je bil uspešno dodan.";
            // return RedirectToPage("/Trener/Termini");
        }
    }
}
