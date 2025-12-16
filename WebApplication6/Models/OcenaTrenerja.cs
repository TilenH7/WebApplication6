using System;
using System.Collections.Generic;

namespace WebApplication6.Models
{
    public class OcenaTrenerja
    {
        public int Id { get; set; }
        public string TrenerUsername { get; set; }
        public string UporabnikUsername { get; set; }
        public int Ocena { get; set; } // 1–5
        public string Komentar { get; set; }

        public DateTime Datum { get; set; }
    }

    public static class FakeOceneDb
    {
        public static List<OcenaTrenerja> Ocene { get; } = new();
    }
}

