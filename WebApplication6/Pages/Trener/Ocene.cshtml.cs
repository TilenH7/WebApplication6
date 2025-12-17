using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;

namespace WebApplication6.Pages.Trener
{
    public class OceneModel : PageModel
    {
        public double? PovprecnaOcena { get; set; }

        public List<OcenaTrenerja> Ocene { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int? FilterOcena { get; set; }  // 1..5 ali null

        public IActionResult OnGet()
        {

            var trener = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(trener))
                return RedirectToPage("/Login");

            var vseOcene = FakeOceneDb.Ocene.Where(o => o.TrenerUsername == trener).ToList();
            PovprecnaOcena = vseOcene.Any() ? vseOcene.Average(o => o.Ocena) : (double?)null;

            var query = vseOcene.AsQueryable();

            if (FilterOcena.HasValue && FilterOcena.Value >= 1 && FilterOcena.Value <= 5)
                query = query.Where(o => o.Ocena == FilterOcena.Value);

            Ocene = query
                .OrderByDescending(o => o.Datum)
                .ToList();

            return Page();
        }
    }
}
