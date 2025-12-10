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

        public void OnGet(string trener)
        {
            // osnovni podatki
            Trener = FakeTrenerDb.Trenerji
                .FirstOrDefault(t => t.Username == trener);

            // ocene
            var oceneTrenerja = FakeOceneDb.Ocene
                .Where(o => o.TrenerUsername == trener)
                .ToList();

            if (oceneTrenerja.Any())
            {
                PovprecnaOcena = oceneTrenerja.Average(o => o.Ocena);
                // zadnjih nekaj komentarjev (recimo 3)
                ZadnjeOcene = oceneTrenerja
                    .OrderByDescending(o => o.Id)
                    .Take(3)
                    .ToList();
            }

            // prihajajoèi termini
            PrihajajociTermini = FakeTerminDb.Termini
                .Where(t => t.TrenerUsername == trener &&
                            t.DatumInCas >= DateTime.Now)
                .OrderBy(t => t.DatumInCas)
                .ToList();
        }
    }
}
