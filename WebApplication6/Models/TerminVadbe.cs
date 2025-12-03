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
    }

    public static class FakeTerminDb
    {
        public static List<TerminVadbe> Termini { get; } = new()
        {
            new TerminVadbe
            {
                Id = 1,
                TrenerUsername = "trener1",
                DatumInCas = new DateTime(2069, 1, 1, 18, 0, 0), // 1.1.2069 ob 18:00
                Lokacija = "Ljubljana - vnaprej definiran termin"
            }
        };
    }
}
