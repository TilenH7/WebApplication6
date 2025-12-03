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

        public void OnGet()
        {
            Username = HttpContext.Session.GetString("Username") ?? "neznan";

            // samo prihodnji termini, sortirani
            Termini = FakeTerminDb.Termini
                .Where(t => t.DatumInCas >= DateTime.Now)
                .OrderBy(t => t.DatumInCas)
                .ToList();
        }
    }
}
