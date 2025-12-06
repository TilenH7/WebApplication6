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

        // nova polja
        public int Kapaciteta { get; set; } = 10;     // max št. mest
        public int ZasedenaMesta { get; set; } = 0;   // trenutno zasedenih mest
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
                ZasedenaMesta = 0
            },
              new TerminVadbe
                {
                    Id = 2,
                    TrenerUsername = "trener1",
                    DatumInCas = new DateTime(2069, 1, 2, 18, 0, 0),
                    Lokacija = "Maribor"
                },

        };
    }
}
