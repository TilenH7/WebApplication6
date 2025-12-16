using System.Collections.Generic;

namespace WebApplication6.Models
{
    public class SledenjeTrenerju
    {
        public int Id { get; set; }
        public string UporabnikUsername { get; set; }
        public string TrenerUsername { get; set; }
    }

    public static class FakeSledenjeDb
    {
        public static List<SledenjeTrenerju> Sledenja { get; } = new();
    }
}
