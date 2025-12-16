using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;
using System.Linq;

namespace WebApplication6.Pages.Uporabnik
{
    public class SlediTrenerjaModel : PageModel
    {
        public IActionResult OnGet(string trenerUsername)
        {
            var role = HttpContext.Session.GetString("Role");
            var user = HttpContext.Session.GetString("Username");

            if (role != UserRole.Uporabnik.ToString() || string.IsNullOrEmpty(user))
                return RedirectToPage("/Login");

            if (string.IsNullOrEmpty(trenerUsername))
                return RedirectToPage("/Uporabnik/Index");

            // že sledi? → nič ne dodaj
            var already = FakeSledenjeDb.Sledenja
                .Any(s => s.UporabnikUsername == user &&
                          s.TrenerUsername == trenerUsername);

            if (!already)
            {
                var id = FakeSledenjeDb.Sledenja.Any()
                    ? FakeSledenjeDb.Sledenja.Max(s => s.Id) + 1
                    : 1;

                FakeSledenjeDb.Sledenja.Add(new SledenjeTrenerju
                {
                    Id = id,
                    UporabnikUsername = user,
                    TrenerUsername = trenerUsername
                });
            }

            // nazaj na profil trenerja
            return RedirectToPage("/Trener/Profil", new { trener = trenerUsername });
        }
    }
}
