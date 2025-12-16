using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApplication6.Models;
using System.Linq;

namespace WebApplication6.Pages.Uporabnik
{
    public class NehajSleditiTrenerjaModel : PageModel
    {
        public IActionResult OnGet(string trenerUsername, string redirectTo = null)
        {
            var role = HttpContext.Session.GetString("Role");
            var user = HttpContext.Session.GetString("Username");

            if (role != UserRole.Uporabnik.ToString() || string.IsNullOrEmpty(user))
                return RedirectToPage("/Login");

            if (!string.IsNullOrEmpty(trenerUsername))
            {
                FakeSledenjeDb.Sledenja.RemoveAll(s =>
                    s.UporabnikUsername == user &&
                    s.TrenerUsername == trenerUsername);
            }

            // če pride iz seznama, ga vrnemo na seznam, sicer na profil
            if (redirectTo == "lista")
                return RedirectToPage("/Uporabnik/SledeniTrenerji");

            return RedirectToPage("/Trener/Profil", new { trener = trenerUsername });
        }
    }
}
