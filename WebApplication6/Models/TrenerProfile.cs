using System.Collections.Generic;

namespace WebApplication6.Models
{
    public class TrenerProfile
    {
        public string Username { get; set; }      // "trener1"
        public string Ime { get; set; }           // npr. "Marko Novak"
        public string Opis { get; set; }          // opis trenerja
        public string Specializacije { get; set; } // npr. "Crossfit, HIIT"
        public string Lokacija { get; set; }      // npr. "Ljubljana"
    }

    public static class FakeTrenerDb
    {
        public static List<TrenerProfile> Trenerji { get; } = new()
        {
            new TrenerProfile
            {
                Username = "trener1",
                Ime = "Marko Novak",
                Opis = "Specialist za funkcionalne vadbe in individualno pripravo.",
                Specializacije = "Funkcionalna vadba, HIIT, kondicijska priprava",
                Lokacija = "Ljubljana"
            }
        };
    }
}
