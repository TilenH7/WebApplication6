using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;

namespace WebApplication6.Pages.Trener
{
    public class TerminiModel : PageModel
    {
        public List<TerminVadbe> Termini { get; set; } = new();

        // terminId -> seznam uporabnikov
        public Dictionary<int, List<string>> UporabnikiNaTerminu { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterOd { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? FilterDo { get; set; }

        public void OnGet()
        {
            // v resnici bi vzel iz Session; za prototip fallback na trener1
            var username = HttpContext.Session.GetString("Username") ?? "trener1";

            var zdaj = DateTime.Now;

            // vsi prihodnji termini tega trenerja
            var query = FakeTerminDb.Termini
                .Where(t => t.TrenerUsername == username &&
                            t.DatumInCas >= zdaj);

            // filter po datumu OD
            if (FilterOd.HasValue)
            {
                var od = FilterOd.Value.Date;
                query = query.Where(t => t.DatumInCas.Date >= od);
            }

            // filter po datumu DO
            if (FilterDo.HasValue)
            {
                var doDat = FilterDo.Value.Date;
                query = query.Where(t => t.DatumInCas.Date <= doDat);
            }

            Termini = query
                .OrderBy(t => t.DatumInCas)
                .ToList();

            // naložimo rezervacije za prikazane termine
            var prikazaniIds = Termini.Select(t => t.Id).ToList();

            var rezervacije = FakeRezervacijeDb.Rezervacije
                .Where(r => prikazaniIds.Contains(r.TerminId))
                .ToList();

            UporabnikiNaTerminu = rezervacije
                .GroupBy(r => r.TerminId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(r => r.UporabnikUsername).ToList()
                );
        }

        // helper za cshtml
        public List<string> GetUporabnikiZaTermin(int terminId)
        {
            if (UporabnikiNaTerminu.TryGetValue(terminId, out var list))
                return list;
            return new List<string>();
        }
    }
}
