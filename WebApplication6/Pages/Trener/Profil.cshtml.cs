using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication6.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Http; // ⬅️ DODAJ TO

namespace WebApplication6.Pages.Trener
{
    public class ProfilModel : PageModel
    {
        public TrenerProfile Trener { get; set; }

        public double? PovprecnaOcena { get; set; }
        public List<OcenaTrenerja> ZadnjeOcene { get; set; } = new();
        public List<TerminVadbe> PrihajajociTermini { get; set; } = new();

        // 💰 cene iz terminov
        public decimal? PovprecnaCena { get; set; }
        public decimal? NajnizjaCena { get; set; }
        public decimal? NajvisjaCena { get; set; }

        // 💚 sledenje
        public bool JePrijavljenUporabnik { get; set; }
        public bool TrenutniUporabnikSledi { get; set; }

        public void OnGet(string trener)
        {
            // osnovni podatki
            Trener = FakeTrenerDb.Trenerji
                .FirstOrDefault(t => t.Username == trener);

            if (Trener == null)
            {
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
            else
            {
                PovprecnaOcena = null;
                ZadnjeOcene = new();
            }

            // prihajajoči termini
            PrihajajociTermini = FakeTerminDb.Termini
                .Where(t => t.TrenerUsername == trener &&
                            t.DatumInCas >= DateTime.Now)
                .OrderBy(t => t.DatumInCas)
                .ToList();

            // 💰 cene iz terminov
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
                PovprecnaCena = Trener.CenaNaUro;
            }

            // 💚 sledenje – preverimo trenutno prijavljenega uporabnika
            var role = HttpContext.Session.GetString("Role");
            var username = HttpContext.Session.GetString("Username");

            if (role == UserRole.Uporabnik.ToString() && !string.IsNullOrEmpty(username))
            {
                JePrijavljenUporabnik = true;

                TrenutniUporabnikSledi = FakeSledenjeDb.Sledenja
                    .Any(s => s.UporabnikUsername == username &&
                              s.TrenerUsername == trener);
            }
        }
    }
}
