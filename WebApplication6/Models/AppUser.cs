using System.Collections.Generic;

namespace WebApplication6.Models
{
    public enum UserRole
    {
        Trener,
        Uporabnik
    }

    public class AppUser
    {
        public string Username { get; set; }      // ostane za login
        public string Password { get; set; }      // za prototip OK plain text
        public string Ime { get; set; }           // novo obvezno polje
        public string Priimek { get; set; }
        public string Email { get; set; }         // novo obvezno polje
        public UserRole Role { get; set; }
        public string Lokacija { get; set; }
        public string Telefon { get; set; }
    }

    public static class FakeUserDb
    {
        public static List<AppUser> Users = new()
        {
            new AppUser
            {
                Username = "Uroš",
                Password = "test123",
                Ime = "Uros",
                Priimek = "Novak",
                Email = "trener1@test.com",
                Role = UserRole.Trener
            },
            new AppUser
            {
                Username = "Marko",
                Password = "test123",
                Ime = "Uporabnik Prvi",
                Priimek = "Horvat",
                Email = "user1@test.com",
                Role = UserRole.Uporabnik
            }
        };
    }

}
