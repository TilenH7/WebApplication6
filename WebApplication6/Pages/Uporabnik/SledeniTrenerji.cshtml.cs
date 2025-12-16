using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;
using System.Collections.Generic;
using System.Linq;

namespace WebApplication6.Pages.Uporabnik
{
    public class SledeniTrenerjiModel : PageModel
    {
        public List<TrenerProfile> Trenerji { get; set; } = new();

        public IActionResult OnGet()
        {
            var role = HttpContext.Session.GetString("Role");
            var user = HttpContext.Session.GetString("Username");

            if (role != UserRole.Uporabnik.ToString() || string.IsNullOrEmpty(user))
                return RedirectToPage("/Login");

            var sledeniUsernames = FakeSledenjeDb.Sledenja
                .Where(s => s.UporabnikUsername == user)
                .Select(s => s.TrenerUsername)
                .Distinct()
                .ToList();

            Trenerji = FakeTrenerDb.Trenerji
                .Where(t => sledeniUsernames.Contains(t.Username))
                .ToList();

            return Page();
        }
    }
}
