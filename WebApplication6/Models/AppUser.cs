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
        public string Email { get; set; }         // novo obvezno polje
        public UserRole Role { get; set; }
    }

    public static class FakeUserDb
    {
        public static List<AppUser> Users = new()
        {
            new AppUser
            {
                Username = "trener1",
                Password = "test123",
                Ime = "Trener Prvi",
                Email = "trener1@test.com",
                Role = UserRole.Trener
            },
            new AppUser
            {
                Username = "user1",
                Password = "test123",
                Ime = "Uporabnik Prvi",
                Email = "user1@test.com",
                Role = UserRole.Uporabnik
            }
        };
    }
}
