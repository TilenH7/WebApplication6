using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;

namespace WebApplication6.Pages.Trener
{
    public class StatistikaModel : PageModel
    {
        public int RezervacijeSkupaj { get; set; }
        public int RezervacijeMesec { get; set; }
        public double? PovprecnaOcena { get; set; }

        public IActionResult OnGet()
        {
            var trener = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(trener) || role != UserRole.Trener.ToString())
                return RedirectToPage("/Login");

            // vsi termini tega trenerja
            var mojiTerminIds = FakeTerminDb.Termini
                .Where(t => t.TrenerUsername == trener)
                .Select(t => t.Id)
                .ToHashSet();

            // rezervacije za njegove termine
            var mojeRezervacije = FakeRezervacijeDb.Rezervacije
                .Where(r => mojiTerminIds.Contains(r.TerminId));

            RezervacijeSkupaj = mojeRezervacije.Count();

            // rezervacije ta mesec (glede na datum termina)
            var danes = DateTime.Today;
            var zacetekMeseca = new DateTime(danes.Year, danes.Month, 1);
            var zacetekNaslednjega = zacetekMeseca.AddMonths(1);

            var terminiVTemMesecu = FakeTerminDb.Termini
                .Where(t => t.TrenerUsername == trener &&
                            t.DatumInCas >= zacetekMeseca &&
                            t.DatumInCas < zacetekNaslednjega)
                .Select(t => t.Id)
                .ToHashSet();

            RezervacijeMesec = FakeRezervacijeDb.Rezervacije
                .Count(r => terminiVTemMesecu.Contains(r.TerminId));

            // povpreèna ocena
            var ocene = FakeOceneDb.Ocene
                .Where(o => o.TrenerUsername == trener)
                .ToList();

            PovprecnaOcena = ocene.Any() ? ocene.Average(o => o.Ocena) : (double?)null;

            return Page();
        }
    }
}
