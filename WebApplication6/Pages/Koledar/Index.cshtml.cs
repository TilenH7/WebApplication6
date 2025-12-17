using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication6.Models;

namespace WebApplication6.Pages.Koledar
{
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int Year { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Month { get; set; }

        public DateTime FirstDayOfMonth { get; set; }
        public int StartOffset { get; set; } // koliko praznih celic pred 1. dnem
        public int DaysInMonth { get; set; }

        // datum -> termini na ta dan
        public Dictionary<DateTime, List<TerminVadbe>> TerminiPoDnevih { get; set; } = new();
        public Dictionary<int, int> SteviloPrijav { get; set; } = new();


        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(role))
                return RedirectToPage("/Login");

            if (Year == 0) Year = DateTime.Today.Year;
            if (Month == 0) Month = DateTime.Today.Month;

            FirstDayOfMonth = new DateTime(Year, Month, 1);
            DaysInMonth = DateTime.DaysInMonth(Year, Month);

            // Koledar zaènemo z ponedeljkom (Mon=0 ... Sun=6)
            var dow = (int)FirstDayOfMonth.DayOfWeek; // Sun=0 ... Sat=6
            StartOffset = (dow == 0) ? 6 : dow - 1;

            IEnumerable<TerminVadbe> terminiQuery;

            if (role == UserRole.Trener.ToString())
            {
                // trener vidi vse svoje termine
                terminiQuery = FakeTerminDb.Termini
                    .Where(t => t.TrenerUsername == username);
            }
            else
            {
                // uporabnik vidi samo termine, na katere je prijavljen
                var ids = FakeRezervacijeDb.Rezervacije
                    .Where(r => r.UporabnikUsername == username)
                    .Select(r => r.TerminId)
                    .ToHashSet();

                terminiQuery = FakeTerminDb.Termini
                    .Where(t => ids.Contains(t.Id));
            }

            // filtriramo samo termine v izbranem mesecu
            terminiQuery = terminiQuery.Where(t => t.DatumInCas.Year == Year && t.DatumInCas.Month == Month);

            TerminiPoDnevih = terminiQuery
                .GroupBy(t => t.DatumInCas.Date)
                .ToDictionary(g => g.Key, g => g.OrderBy(x => x.DatumInCas).ToList());

            SteviloPrijav = FakeRezervacijeDb.Rezervacije
                .GroupBy(r => r.TerminId)
                .ToDictionary(g => g.Key, g => g.Count());


            return Page();
        }

        public string MonthName()
        {
            return new DateTime(Year, Month, 1).ToString("MMMM yyyy");
        }
    }
}
