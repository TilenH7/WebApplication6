using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication6.Pages.Uporabnik
{
    public class IndexModel : PageModel
    {
        public string Username { get; set; }
        public List<TerminVadbe> Termini { get; set; } = new();
        public List<TerminVadbe> MojeRezervacije { get; set; } = new();

        // 🔎 filterji (query string / GET)
        [BindProperty(SupportsGet = true)]
        public string SearchLokacija { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? SearchDatum { get; set; }

        public void OnGet()
        {
            Username = HttpContext.Session.GetString("Username") ?? "neznan";

            // osnovni query – vsi prihodnji termini
            var query = FakeTerminDb.Termini
                .Where(t => t.DatumInCas >= DateTime.Now);

            // filter po lokaciji
            if (!string.IsNullOrWhiteSpace(SearchLokacija))
            {
                query = query.Where(t =>
                    !string.IsNullOrEmpty(t.Lokacija) &&
                    t.Lokacija.Contains(SearchLokacija, StringComparison.OrdinalIgnoreCase));
            }

            // filter po datumu (čas ignoriramo, samo dan)
            if (SearchDatum.HasValue)
            {
                var d = SearchDatum.Value.Date;
                query = query.Where(t => t.DatumInCas.Date == d);
            }

            Termini = query
                .OrderBy(t => t.DatumInCas)
                .ToList();

            // ----- moje rezervacije ostane isto kot prej -----
            var mojaRezerviranaIds = FakeRezervacijeDb.Rezervacije
                .Where(r => r.UporabnikUsername == Username)
                .Select(r => r.TerminId)
                .ToList();

            MojeRezervacije = FakeTerminDb.Termini
                .Where(t => mojaRezerviranaIds.Contains(t.Id))
                .OrderBy(t => t.DatumInCas)
                .ToList();
        }
    }
}
