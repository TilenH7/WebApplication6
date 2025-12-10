using System.Collections.Generic;

namespace WebApplication6.Models
{
    public class TrenerProfile
    {
        public string Username { get; set; }       // npr. "trener1"
        public string Ime { get; set; }            // prikazno ime
        public string Opis { get; set; }           // opis
        public string Specializacije { get; set; } // npr. "HIIT, Crossfit"
        public string Lokacija { get; set; }       // npr. "Ljubljana"
        public decimal? CenaNaUro { get; set; }    // cena na uro
        public string SlikaUrl { get; set; }       // URL slike
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
                Lokacija = "Ljubljana",
                CenaNaUro = 30m,
                SlikaUrl = "/images/trener1.png" // lahko je tudi null / prazen string
            }
        };
    }
}
