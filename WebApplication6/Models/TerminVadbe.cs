using System;
using System.Collections.Generic;

namespace WebApplication6.Models
{
    public class TerminVadbe
    {
        public int Id { get; set; }
        public string TrenerUsername { get; set; }
        public DateTime DatumInCas { get; set; }
        public string Lokacija { get; set; }

        public int Kapaciteta { get; set; } = 10;
        public int ZasedenaMesta { get; set; } = 0;

        // 💰 NOVO
        public decimal Cena { get; set; } = 10m;
        public string TipVadbe { get; set; } = "Kardio"; // npr. Kardio, Moč, Joga...
        public int TrajanjeMin { get; set; } = 60;       // npr. 60 = 1h

    }

    public static class FakeTerminDb
    {
        public static List<TerminVadbe> Termini { get; } = new()
        {
            new TerminVadbe
            {
                Id = 1,
                TrenerUsername = "trener1",
                DatumInCas = new DateTime(2069, 1, 1, 18, 0, 0),
                Lokacija = "Ljubljana - vnaprej definiran termin",
                Kapaciteta = 10,
                ZasedenaMesta = 0,
                Cena = 15m,
                TipVadbe = "Moč",
                TrajanjeMin = 60,

            },
            new TerminVadbe
            {
                Id = 2,
                TrenerUsername = "trener1",
                DatumInCas = new DateTime(2069, 1, 2, 18, 0, 0),
                Lokacija = "Maribor",
                Kapaciteta = 10,
                ZasedenaMesta = 0,
                Cena = 12m,
                TipVadbe = "Moč",
                TrajanjeMin = 60,

            }
        };
    }
}
