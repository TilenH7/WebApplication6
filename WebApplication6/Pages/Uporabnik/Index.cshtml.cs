using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace WebApplication6.Pages.Uporabnik
{
    public class IndexModel : PageModel
    {
        public string Username { get; set; }

        public void OnGet()
        {
            Username = HttpContext.Session.GetString("Username") ?? "neznan";
        }
    }
}
