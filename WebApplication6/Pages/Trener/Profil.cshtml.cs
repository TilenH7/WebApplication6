using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication6.Models;
using System.Linq;
using System.Collections.Generic;

namespace WebApplication6.Pages.Trener
{
    public class ProfilModel : PageModel
    {
        public string TrenerUsername { get; set; }
        public List<OcenaTrenerja> Ocene { get; set; }

        public void OnGet(string trener)
        {
            TrenerUsername = trener;
            Ocene = FakeOceneDb.Ocene
                .Where(o => o.TrenerUsername == trener)
                .ToList();
        }
    }
}

