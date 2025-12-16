using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;
using System.Linq;

namespace WebApplication6.Pages.Trener
{
    public class IndexModel : PageModel
    {
        public string Username { get; set; }

        public decimal? NajnizjaCena { get; set; }
        public decimal? NajvisjaCena { get; set; }
        public List<string> Sledilci { get; set; } = new();

        public void OnGet()
        {
            Username = HttpContext.Session.GetString("Username") ?? "neznan";

            var mojiTermini = FakeTerminDb.Termini
                .Where(t => t.TrenerUsername == Username)
                .ToList();

            if (mojiTermini.Any())
            {
                NajnizjaCena = mojiTermini.Min(t => t.Cena);
                NajvisjaCena = mojiTermini.Max(t => t.Cena);
            }
            Sledilci = FakeSledenjeDb.Sledenja
                .Where(s => s.TrenerUsername == Username)
                .Select(s => s.UporabnikUsername)
                .Distinct()
                .ToList();
        }
    }
}
