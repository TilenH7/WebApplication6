using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication6.Models;
using System.Linq;
using System.Collections.Generic;
using System;

namespace WebApplication6.Pages.Trener
{
    public class ProfilModel : PageModel
    {
        public TrenerProfile Trener { get; set; }

        public double? PovprecnaOcena { get; set; }
        public List<OcenaTrenerja> ZadnjeOcene { get; set; } = new();
        public List<TerminVadbe> PrihajajociTermini { get; set; } = new();

        // 💰 NOVO:
        public decimal? PovprecnaCena { get; set; }
        public decimal? NajnizjaCena { get; set; }
        public decimal? NajvisjaCena { get; set; }

        public void OnGet(string trener)
        {
            // osnovni podatki
            Trener = FakeTrenerDb.Trenerji
                .FirstOrDefault(t => t.Username == trener);

            if (Trener == null)
            {
                // če ni trenerja, lahko samo pustimo prazno / 404, odvisno kaj želiš
                return;
            }

            // ocene
            var oceneTrenerja = FakeOceneDb.Ocene
                .Where(o => o.TrenerUsername == trener)
                .ToList();

            if (oceneTrenerja.Any())
            {
                PovprecnaOcena = oceneTrenerja.Average(o => o.Ocena);
                ZadnjeOcene = oceneTrenerja
                    .OrderByDescending(o => o.Id)
                    .Take(3)
                    .ToList();
            }

            // prihajajoči termini
            PrihajajociTermini = FakeTerminDb.Termini
                .Where(t => t.TrenerUsername == trener &&
                            t.DatumInCas >= DateTime.Now)
                .OrderBy(t => t.DatumInCas)
                .ToList();

            // 💰 CENE – vzemi VSE njegove termine s ceno > 0 (ne samo prihodnje)
            var terminiSCenami = FakeTerminDb.Termini
                .Where(t => t.TrenerUsername == trener && t.Cena > 0)
                .ToList();

            if (terminiSCenami.Any())
            {
                PovprecnaCena = terminiSCenami.Average(t => t.Cena);
                NajnizjaCena = terminiSCenami.Min(t => t.Cena);
                NajvisjaCena = terminiSCenami.Max(t => t.Cena);
            }
            else
            {
                // fallback: če še nima terminov s ceno, lahko uporabiš CenaNaUro iz profila
                PovprecnaCena = Trener.CenaNaUro;
            }
        }
    }
}
