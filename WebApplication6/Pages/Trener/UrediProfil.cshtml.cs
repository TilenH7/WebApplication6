using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;
using System.Linq;

namespace WebApplication6.Pages.Trener
{
    public class UrediProfilModel : PageModel
    {
        [BindProperty] public string Ime { get; set; }
        [BindProperty] public string Opis { get; set; }
        [BindProperty] public string Specializacije { get; set; }
        [BindProperty] public string Lokacija { get; set; }
        [BindProperty] public decimal? CenaNaUro { get; set; }
        [BindProperty] public string SlikaUrl { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // v pravem svetu bi vzel iz Session; tu fallback na "trener1"
            var username = HttpContext.Session.GetString("Username") ?? "trener1";

            var trener = FakeTrenerDb.Trenerji.FirstOrDefault(t => t.Username == username);
            if (trener != null)
            {
                Ime = trener.Ime;
                Opis = trener.Opis;
                Specializacije = trener.Specializacije;
                Lokacija = trener.Lokacija;
                CenaNaUro = trener.CenaNaUro;
                SlikaUrl = trener.SlikaUrl;
            }
        }

        public IActionResult OnPost()
        {
            var username = HttpContext.Session.GetString("Username") ?? "trener1";

            if (string.IsNullOrWhiteSpace(Ime))
            {
                ErrorMessage = "Ime je obvezno.";
                return Page(); // vrni isto stran z napako
            }

            var trener = FakeTrenerDb.Trenerji.FirstOrDefault(t => t.Username == username);
            if (trener == null)
            {
                ErrorMessage = "Trener ne obstaja.";
                return Page();
            }

            // posodobimo podatke
            trener.Ime = Ime;
            trener.Opis = Opis;
            trener.Specializacije = Specializacije;
            trener.Lokacija = Lokacija;
            trener.CenaNaUro = CenaNaUro;
            trener.SlikaUrl = SlikaUrl;

            // preusmeritev na profil → posodobljeni podatki takoj vidni
            return RedirectToPage("/Trener/Profil", new { trener = username });
        }
    }
}
