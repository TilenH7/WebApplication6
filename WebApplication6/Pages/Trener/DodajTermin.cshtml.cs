using System;
using System.Linq;
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

        public string ErrorMessage { get; set; }
        public string SuccessMessage { get; set; }

        public IActionResult OnGet()
        {
            // dovolimo samo trenerju
            if (HttpContext.Session.GetString("Role") != UserRole.Trener.ToString())
                return RedirectToPage("/Login");
            DatumInCas = DateTime.Now.Date.AddDays(1).AddHours(18);

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

            // ✅ preveri pretekli datum
            if (DatumInCas < DateTime.Now)
            {
                ErrorMessage = "Datum in ura morata biti v prihodnosti.";
                return Page();
            }

            var termin = new TerminVadbe
            {
                Id = FakeTerminDb.Termini.Count + 1,
                TrenerUsername = HttpContext.Session.GetString("Username"),
                DatumInCas = DatumInCas,
                Lokacija = Lokacija
            };

            FakeTerminDb.Termini.Add(termin);

            SuccessMessage = "Termin je bil uspešno dodan.";
            return Page();
        }
    }
}
