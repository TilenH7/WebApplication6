using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;
using System.Linq;

namespace WebApplication6.Pages.Uporabnik
{
    public class OceniModel : PageModel
    {
        [BindProperty]
        public int Ocena { get; set; }

        [BindProperty]
        public string Komentar { get; set; }

        public string TrenerUsername { get; set; }
        public string Message { get; set; }

        public DateTime Datum { get; set; }
        public IActionResult OnGet(string trener)
        {
            if (string.IsNullOrEmpty(trener))
                return RedirectToPage("/Uporabnik/Index");

            TrenerUsername = trener;
            return Page();
        }

        public IActionResult OnPost(string trener)
        {
            TrenerUsername = trener;

            if (Ocena < 1 || Ocena > 5)
            {
                Message = "Ocena mora biti med 1 in 5.";
                return Page();
            }

            var ocen = new OcenaTrenerja
            {
                Id = FakeOceneDb.Ocene.Count + 1,
                TrenerUsername = trener,
                UporabnikUsername = HttpContext.Session.GetString("Username"),
                Ocena = Ocena,
                Komentar = Komentar,
                Datum = DateTime.Now


            };

            FakeOceneDb.Ocene.Add(ocen);
            Message = "Hvala za vašo oceno!";
            return Page();
        }
    }
}
