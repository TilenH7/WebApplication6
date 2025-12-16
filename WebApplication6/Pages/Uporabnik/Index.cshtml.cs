using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication6.Models;

namespace WebApplication6.Pages.Uporabnik
{
    public class IndexModel : PageModel
    {
        public string Username { get; set; }

        public List<TerminVadbe> Termini { get; set; } = new();
        public List<TerminVadbe> MojeRezervacije { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchLokacija { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? SearchDatum { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("Role") != UserRole.Uporabnik.ToString())
                return RedirectToPage("/Login");

            Username = HttpContext.Session.GetString("Username") ?? "neznan";

            var zdaj = DateTime.Now;

            // ===== razpoložljivi termini =====
            var query = FakeTerminDb.Termini
                .Where(t => t.DatumInCas >= zdaj && t.ZasedenaMesta < t.Kapaciteta);

            if (!string.IsNullOrWhiteSpace(SearchLokacija))
            {
                query = query.Where(t =>
                    t.Lokacija != null &&
                    t.Lokacija.Contains(SearchLokacija, StringComparison.OrdinalIgnoreCase));
            }

            if (SearchDatum.HasValue)
            {
                var d = SearchDatum.Value.Date;
                query = query.Where(t => t.DatumInCas.Date == d);
            }

            Termini = query
                .OrderBy(t => t.DatumInCas)
                .ToList();

            // ===== MOJE REZERVACIJE – KLJUČNO: FILTRIRAJ PO UPORABNIKU =====
            var rezervacijeUserja = FakeRezervacijeDb.Rezervacije
                .Where(r => r.UporabnikUsername == Username)   // 🔥 samo moje
                .ToList();

            var mojiTerminIds = rezervacijeUserja
                .Select(r => r.TerminId)
                .ToList();

            MojeRezervacije = FakeTerminDb.Termini
                .Where(t => mojiTerminIds.Contains(t.Id))
                .OrderBy(t => t.DatumInCas)
                .ToList();

            return Page();
        }
    }
}
