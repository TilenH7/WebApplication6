using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication6.Models;

namespace WebApplication6.Pages.Uporabnik
{
    public class ZgodovinaModel : PageModel
    {
        public string Username { get; set; }

        public List<TerminVadbe> ZakljuceniTermini { get; set; } = new();

        public double SkupajUr { get; set; } = 0;

        // tip -> (št. vadb, skupaj minut)
        public Dictionary<string, (int Count, int TotalMinutes)> StatistikaPoTipih { get; set; }
            = new();

        public void OnGet()
        {
            Username = HttpContext.Session.GetString("Username") ?? "user1";

            // vsi termini, ki jih je uporabnik rezerviral
            var rezerviraniIds = FakeRezervacijeDb.Rezervacije
                .Where(r => r.UporabnikUsername == Username)
                .Select(r => r.TerminId)
                .ToList();

            // samo zakljuèeni = pretekli
            ZakljuceniTermini = FakeTerminDb.Termini
                .Where(t => rezerviraniIds.Contains(t.Id) && t.DatumInCas < DateTime.Now)
                .OrderByDescending(t => t.DatumInCas)
                .ToList();

            // vsota ur
            var skupajMin = ZakljuceniTermini.Sum(t => t.TrajanjeMin);
            SkupajUr = skupajMin / 60.0;

            // statistika po tipih (samo tipi, ki jih je dejansko obiskal)
            StatistikaPoTipih = ZakljuceniTermini
                .GroupBy(t => t.TipVadbe ?? "Neznano")
                .ToDictionary(
                    g => g.Key,
                    g => (g.Count(), g.Sum(x => x.TrajanjeMin))
                );
        }
    }
}
