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
        public static List<TerminVadbe> Termini { get; } = new();
    }
}
