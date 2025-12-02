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
        public string Username { get; set; }
        public string Password { get; set; } // za prototip OK plain text
        public UserRole Role { get; set; }
    }

    public static class FakeUserDb
    {
        public static List<AppUser> Users = new()
        {
            new AppUser { Username = "trener1", Password = "test123", Role = UserRole.Trener },
            new AppUser { Username = "user1",   Password = "test123", Role = UserRole.Uporabnik }
        };
    }
}
