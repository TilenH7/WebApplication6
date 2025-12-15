using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;

namespace WebApplication6.Pages.Trener
{
    public class UrediTerminModel : PageModel
    {
        [BindProperty] public int Id { get; set; }
        [BindProperty] public DateTime DatumInCas { get; set; }
        [BindProperty] public string Lokacija { get; set; }
        [BindProperty] public decimal Cena { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnGet(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            if (HttpContext.Session.GetString("Role") != UserRole.Trener.ToString())
                return RedirectToPage("/Login");

            var termin = FakeTerminDb.Termini
                .FirstOrDefault(t => t.Id == id && t.TrenerUsername == username);

            if (termin == null)
            {
                ErrorMessage = "Termin ni bil najden ali ni tvoj.";
                return Page();
            }

            Id = termin.Id;
            DatumInCas = termin.DatumInCas;
            Lokacija = termin.Lokacija;
            Cena = termin.Cena;

            return Page();
        }

        public IActionResult OnPost()
        {
            var username = HttpContext.Session.GetString("Username");
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

            var termin = FakeTerminDb.Termini
                .FirstOrDefault(t => t.Id == Id && t.TrenerUsername == username);

            if (termin == null)
            {
                ErrorMessage = "Termin ni bil najden.";
                return Page();
            }

            // dejanska posodobitev
            termin.DatumInCas = DatumInCas;
            termin.Lokacija = Lokacija;
            termin.Cena = Cena;

            return RedirectToPage("/Trener/Termini");
        }
    }
}
