using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;

namespace WebApplication6.Pages.Uporabnik
{
    public class IndexModel : PageModel
    {
        public string Username { get; set; }
        public List<TerminVadbe> Termini { get; set; } = new();
        public List<TerminVadbe> MojeRezervacije { get; set; } = new();

        public void OnGet()
        {
            Username = HttpContext.Session.GetString("Username") ?? "neznan";

            // razpoložljivi prihodnji termini
            Termini = FakeTerminDb.Termini
                .Where(t => t.DatumInCas >= DateTime.Now)
                .OrderBy(t => t.DatumInCas)
                .ToList();

            // moje rezervacije
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
