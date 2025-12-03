using System.Collections.Generic;

namespace WebApplication6.Models
{
    public class Rezervacija
    {
        public int Id { get; set; }
        public int TerminId { get; set; }
        public string UporabnikUsername { get; set; }
    }

    public static class FakeRezervacijeDb
    {
        public static List<Rezervacija> Rezervacije { get; } = new();
    }
}
