using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApplication6.Models;
using WebApplication6.Pages;
using WebApplication6.Pages.Trener;
using WebApplication6.Pages.Uporabnik;



namespace WebApplication6.Tests
{
    // ===== Shared helper: TestSession =====
    public class TestSession : ISession
    {
        private readonly Dictionary<string, byte[]> _store = new();

        public bool IsAvailable => true;
        public string Id { get; } = "test-session";
        public IEnumerable<string> Keys => _store.Keys;

        public void Clear() => _store.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public void Remove(string key) => _store.Remove(key);
        public void Set(string key, byte[] value) => _store[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value);
    }

    // ===== OceniModelTests =====
    [TestClass]
    public class OceniModelTests
    {
        private List<OcenaTrenerja> _backupOcene;

        [TestInitialize]
        public void Setup()
        {
            _backupOcene = FakeOceneDb.Ocene.ToList();
            FakeOceneDb.Ocene.Clear();
        }

        [TestCleanup]
        public void Cleanup()
        {
            FakeOceneDb.Ocene.Clear();
            FakeOceneDb.Ocene.AddRange(_backupOcene);
        }

        private OceniModel CreatePageWithSession(string usernameInSession = null)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();

            if (!string.IsNullOrWhiteSpace(usernameInSession))
                httpContext.Session.SetString("Username", usernameInSession);

            var page = new OceniModel();
            page.PageContext = new PageContext { HttpContext = httpContext };
            return page;
        }

        [TestMethod]
        public void OnGet_WithoutTrener_ShouldRedirectToUporabnikIndex()
        {
            var page = CreatePageWithSession();
            var result = page.OnGet(null);

            var redirect = result as RedirectToPageResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("/Uporabnik/Index", redirect.PageName);
        }
    }

    // ===== Dummy tests =====
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Assert.IsTrue(true);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Assert.IsTrue(true);
        }
    }

    // ===== LoginLogicTests (FakeUserDb lookup) =====
    [TestClass]
    public class LoginLogicTests
    {
        [TestMethod]
        public void Test_ValidLogin_ShouldFindUser()
        {
            var user = FakeUserDb.Users.FirstOrDefault(u => u.Username == "trener1" && u.Password == "test123");
            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void Test_InvalidPassword_ShouldReturnNull()
        {
            var user = FakeUserDb.Users.FirstOrDefault(u => u.Username == "trener1" && u.Password == "wrong");
            Assert.IsNull(user);
        }

        [TestMethod]
        public void Test_UnknownUser_ShouldReturnNull()
        {
            var user = FakeUserDb.Users.FirstOrDefault(u => u.Username == "neobstaja");
            Assert.IsNull(user);
        }
    }

    // ===== TerminVadbeTests =====
    [TestClass]
    public class TerminVadbeTests
    {
        [TestMethod]
        public void Test_CreateTermin_ShouldStoreValuesCorrectly()
        {
            var t = new TerminVadbe
            {
                Id = 1,
                TrenerUsername = "trener1",
                DatumInCas = DateTime.Now.AddDays(1),
                Lokacija = "Ljubljana"
            };

            Assert.AreEqual("Ljubljana", t.Lokacija);
            Assert.IsTrue(t.DatumInCas > DateTime.Now);
        }

        [TestMethod]
        public void Test_AddTerminToFakeDb_ShouldIncreaseCount()
        {
            int before = FakeTerminDb.Termini.Count;
            FakeTerminDb.Termini.Add(new TerminVadbe
            {
                Id = 99,
                DatumInCas = DateTime.Now.AddDays(1),
                Lokacija = "MB"
            });

            Assert.AreEqual(before + 1, FakeTerminDb.Termini.Count);
        }

        [TestMethod]
        public void Test_InvalidTermin_ShouldBePastDate()
        {
            var invalid = new TerminVadbe { DatumInCas = DateTime.Now.AddDays(-1) };
            Assert.IsTrue(invalid.DatumInCas < DateTime.Now);
        }
    }

    // ===== AppUserTests =====
    [TestClass]
    public class AppUserTests
    {
        [TestMethod]
        public void Test_CreateUser_ShouldHaveCorrectValues()
        {
            var user = new AppUser { Username = "ana", Password = "123", Role = UserRole.Uporabnik };
            Assert.AreEqual("ana", user.Username);
            Assert.AreEqual("123", user.Password);
            Assert.AreEqual(UserRole.Uporabnik, user.Role);
        }

        [TestMethod]
        public void Test_FakeUserDb_ShouldContainDefaultUsers()
        {
            Assert.IsTrue(FakeUserDb.Users.Count >= 2);
        }

        [TestMethod]
        public void Test_FakeUserDb_TrenerExists()
        {
            var trener = FakeUserDb.Users.FirstOrDefault(u => u.Role == UserRole.Trener);
            Assert.IsNotNull(trener);
        }
    }

    // ===== RezervacijaTests =====
    [DoNotParallelize]
    [TestClass]
    public class RezervacijaTests
    {
        [TestInitialize]
        public void Setup()
        {
            FakeRezervacijeDb.Rezervacije.Clear();

            FakeTerminDb.Termini.Clear();
            FakeTerminDb.Termini.Add(new TerminVadbe
            {
                Id = 1,
                TrenerUsername = "trener1",
                DatumInCas = new DateTime(2069, 1, 1, 18, 0, 0),
                Lokacija = "Ljubljana"
            });
        }

        [TestMethod]
        public void Test_UspešnaRezervacija()
        {
            var user = "user1";
            var terminId = 1;

            FakeRezervacijeDb.Rezervacije.Add(new Rezervacija
            {
                Id = 1,
                TerminId = terminId,
                UporabnikUsername = user
            });

            Assert.AreEqual(1, FakeRezervacijeDb.Rezervacije.Count);
            var r = FakeRezervacijeDb.Rezervacije.First();
            Assert.AreEqual(user, r.UporabnikUsername);
            Assert.AreEqual(terminId, r.TerminId);
        }

        [TestMethod]
        public void Test_DvojnaRezervacijaNiDovoljena()
        {
            var user = "user1";
            var terminId = 1;

            FakeRezervacijeDb.Rezervacije.Add(new Rezervacija
            {
                Id = 1,
                TerminId = terminId,
                UporabnikUsername = user
            });

            var alreadyExists = FakeRezervacijeDb.Rezervacije
                .Any(r => r.TerminId == terminId && r.UporabnikUsername == user);

            Assert.IsTrue(alreadyExists, "Uporabnik bi moral že imeti to rezervacijo.");
        }

        [TestMethod]
        public void Test_RezervacijaNaNeobstoječTermin()
        {
            var neobstojeciTerminId = 999;
            var obstajaTermin = FakeTerminDb.Termini.Any(t => t.Id == neobstojeciTerminId);

            Assert.IsFalse(obstajaTermin, "Termin z ID 999 ne bi smel obstajati.");
        }
    }

    // ===== RegisterModelTests (PREMESTI iz RezervacijaTests!) =====
    [DoNotParallelize]
    [TestClass]
    public class RegisterModelTests
    {
        private List<AppUser> _backupUsers;

        [TestInitialize]
        public void Setup()
        {
            _backupUsers = FakeUserDb.Users.ToList();
        }

        [TestCleanup]
        public void Cleanup()
        {
            FakeUserDb.Users.Clear();
            FakeUserDb.Users.AddRange(_backupUsers);
        }

        [TestMethod]
        public void Register_MissingFields_ShouldSetErrorMessage_AndNotAddUser()
        {
            var page = new RegisterModel
            {
                Ime = "",
                Email = "new@test.com",
                Password = "123",
                Role = UserRole.Uporabnik
            };

            int before = FakeUserDb.Users.Count;

            var result = page.OnPost();

            Assert.IsInstanceOfType(result, typeof(PageResult));
            Assert.AreEqual("Ime, e-mail in geslo so obvezna polja.", page.ErrorMessage);
            Assert.IsTrue(string.IsNullOrEmpty(page.SuccessMessage));
            Assert.AreEqual(before, FakeUserDb.Users.Count);
        }

        [TestMethod]
        public void Register_ValidInput_ShouldAddUser_AndSetSuccessMessage()
        {
            var page = new RegisterModel
            {
                Ime = "Ana",
                Email = "ana@test.com",
                Password = "123",
                Role = UserRole.Trener
            };

            int before = FakeUserDb.Users.Count;

            var result = page.OnPost();

            Assert.IsInstanceOfType(result, typeof(PageResult));
            Assert.IsTrue(string.IsNullOrEmpty(page.ErrorMessage));
            Assert.AreEqual("Registracija uspešna. Zdaj se lahko prijaviš.", page.SuccessMessage);
            Assert.AreEqual(before + 1, FakeUserDb.Users.Count);

            var added = FakeUserDb.Users.FirstOrDefault(u => u.Email == "ana@test.com");
            Assert.IsNotNull(added);
            Assert.AreEqual("ana@test.com", added.Username);
            Assert.AreEqual("Ana", added.Ime);
            Assert.AreEqual(UserRole.Trener, added.Role);
        }
    }

    // ===== NehajSleditiTrenerjaModelTests =====
    [DoNotParallelize]
    [TestClass]
    public class NehajSleditiTrenerjaModelTests
    {
        private List<SledenjeTrenerju> _backup;

        [TestInitialize]
        public void Setup()
        {
            _backup = FakeSledenjeDb.Sledenja.ToList();
            FakeSledenjeDb.Sledenja.Clear();
        }

        [TestCleanup]
        public void Cleanup()
        {
            FakeSledenjeDb.Sledenja.Clear();
            FakeSledenjeDb.Sledenja.AddRange(_backup);
        }

        private NehajSleditiTrenerjaModel CreatePage(string role = null, string username = null)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();

            if (role != null) httpContext.Session.SetString("Role", role);
            if (username != null) httpContext.Session.SetString("Username", username);

            var page = new NehajSleditiTrenerjaModel();
            page.PageContext = new PageContext { HttpContext = httpContext };
            return page;
        }

        [TestMethod]
        public void OnGet_WrongRoleOrMissingUser_ShouldRedirectToLogin()
        {
            var page = CreatePage(role: UserRole.Trener.ToString(), username: "trener1");

            var result = page.OnGet("trener1");

            var redirect = result as RedirectToPageResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("/Login", redirect.PageName);
        }

        [TestMethod]
        public void OnGet_UserUnfollows_ShouldRemoveFollow_AndRedirectToList_WhenRedirectToLista()
        {
            FakeSledenjeDb.Sledenja.Add(new SledenjeTrenerju
            {
                Id = 1,
                UporabnikUsername = "user1",
                TrenerUsername = "trener1"
            });
            FakeSledenjeDb.Sledenja.Add(new SledenjeTrenerju
            {
                Id = 2,
                UporabnikUsername = "user1",
                TrenerUsername = "trener2"
            });

            var page = CreatePage(role: UserRole.Uporabnik.ToString(), username: "user1");

            var result = page.OnGet("trener1", redirectTo: "lista");

            Assert.IsFalse(FakeSledenjeDb.Sledenja.Any(s => s.UporabnikUsername == "user1" && s.TrenerUsername == "trener1"));
            Assert.IsTrue(FakeSledenjeDb.Sledenja.Any(s => s.UporabnikUsername == "user1" && s.TrenerUsername == "trener2"));

            var redirect = result as RedirectToPageResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("/Uporabnik/SledeniTrenerji", redirect.PageName);
        }
    }
    [DoNotParallelize]
    [TestClass]
    public class UrediProfilModelTests
    {
        private List<AppUser> _backupUsers;

        [TestInitialize]
        public void Setup()
        {
            _backupUsers = FakeUserDb.Users.ToList();

            // Počisti in pripravi minimalen set userjev za te teste
            FakeUserDb.Users.Clear();
            FakeUserDb.Users.Add(new AppUser
            {
                Username = "user1",
                Password = "oldpass",
                Role = UserRole.Uporabnik,
                Ime = "Ana",
                Priimek = "Novak",
                Lokacija = "MB",
                Email = "ana@test.com",
                Telefon = "040123456"
            });
        }

        [TestCleanup]
        public void Cleanup()
        {
            FakeUserDb.Users.Clear();
            FakeUserDb.Users.AddRange(_backupUsers);
        }


        private WebApplication6.Pages.Uporabnik.UrediProfilModel CreatePage(string role = null, string username = null)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();

            if (role != null) httpContext.Session.SetString("Role", role);
            if (username != null) httpContext.Session.SetString("Username", username);

            var page = new WebApplication6.Pages.Uporabnik.UrediProfilModel();
            page.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
            {
                HttpContext = httpContext
            };

            return page;
        }



        [TestMethod]
        public void OnGet_NotLoggedIn_ShouldRedirectToLogin()
        {
            // arrange
            var page = CreatePage(role: UserRole.Uporabnik.ToString(), username: null);

            // act
            var result = page.OnGet();

            // assert
            var redirect = result as RedirectToPageResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("/Login", redirect.PageName);
        }

        [TestMethod]
        public void OnGet_ValidUser_ShouldFillFields_AndReturnPage()
        {
            // arrange
            var page = CreatePage(role: UserRole.Uporabnik.ToString(), username: "user1");

            // act
            var result = page.OnGet();

            // assert
            Assert.IsInstanceOfType(result, typeof(PageResult));
            Assert.AreEqual("Ana", page.Ime);
            Assert.AreEqual("Novak", page.Priimek);
            Assert.AreEqual("MB", page.Lokacija);
            Assert.AreEqual("ana@test.com", page.Email);
            Assert.AreEqual("040123456", page.Telefon);
        }

        [TestMethod]
        public void OnPost_InvalidEmail_ShouldReturnPage_AndSetError_AndNotUpdateEmail()
        {
            // arrange
            var page = CreatePage(role: UserRole.Uporabnik.ToString(), username: "user1");
            page.Ime = "Ana";
            page.Priimek = "Novak";
            page.Lokacija = "MB";
            page.Email = "slab-email"; // invalid
            page.Telefon = "040111111";

            // act
            var result = page.OnPost();

            // assert
            Assert.IsInstanceOfType(result, typeof(PageResult));
            Assert.IsTrue((page.ErrorMessage ?? "").Contains("e-pošte") || (page.ErrorMessage ?? "").Contains("e-po"),
                "Pričakovan error za napačen email format.");

            var user = FakeUserDb.Users.First(u => u.Username == "user1");
            Assert.AreEqual("ana@test.com", user.Email, "Email se ne bi smel spremeniti ob napaki.");
        }

        [TestMethod]
        public void OnPost_ChangePassword_WithCorrectOldPassword_ShouldUpdatePassword_AndSaveFields()
        {
            // arrange
            var page = CreatePage(role: UserRole.Uporabnik.ToString(), username: "user1");

            page.Ime = "Ana2";
            page.Priimek = "Novak2";
            page.Lokacija = "Ljubljana";
            page.Email = "ana2@test.com";
            page.Telefon = "040999999";

            page.StaroGeslo = "oldpass";
            page.NovoGeslo = "newpass123"; // >= 6

            // act
            var result = page.OnPost();

            // assert
            Assert.IsInstanceOfType(result, typeof(PageResult));
            Assert.IsTrue((page.SuccessMessage ?? "").Contains("posodobljen"), "Pričakovan success message.");

            var user = FakeUserDb.Users.First(u => u.Username == "user1");
            Assert.AreEqual("newpass123", user.Password, "Geslo bi se moralo spremeniti.");
            Assert.AreEqual("Ana2", user.Ime);
            Assert.AreEqual("Novak2", user.Priimek);
            Assert.AreEqual("Ljubljana", user.Lokacija);
            Assert.AreEqual("ana2@test.com", user.Email);
            Assert.AreEqual("040999999", user.Telefon);
        }
    }
    [DoNotParallelize]
    [TestClass]
    public class IzbrisiTerminModelTests
    {
        private List<TerminVadbe> _backupTermini;
        private List<Rezervacija> _backupRez;

        [TestInitialize]
        public void Setup()
        {
            _backupTermini = FakeTerminDb.Termini.ToList();
            _backupRez = FakeRezervacijeDb.Rezervacije.ToList();

            FakeTerminDb.Termini.Clear();
            FakeRezervacijeDb.Rezervacije.Clear();
        }

        [TestCleanup]
        public void Cleanup()
        {
            FakeTerminDb.Termini.Clear();
            FakeRezervacijeDb.Rezervacije.Clear();

            FakeTerminDb.Termini.AddRange(_backupTermini);
            FakeRezervacijeDb.Rezervacije.AddRange(_backupRez);
        }

        private IzbrisiTerminModel CreatePage(string role = null, string username = null)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();

            if (role != null) httpContext.Session.SetString("Role", role);
            if (username != null) httpContext.Session.SetString("Username", username);

            var page = new IzbrisiTerminModel();
            page.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
            {
                HttpContext = httpContext
            };

            return page;
        }

        [TestMethod]
        public void OnGet_NotTrainer_ShouldRedirectToLogin()
        {
            // arrange
            var page = CreatePage(role: UserRole.Uporabnik.ToString(), username: "user1");

            // act
            var result = page.OnGet(1);

            // assert
            var redirect = result as RedirectToPageResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("/Login", redirect.PageName);
        }

        [TestMethod]
        public void OnGet_TerminNotFoundOrNotOwned_ShouldSetErrorMessage_AndReturnPage()
        {
            // arrange: termin obstaja, ampak pripada drugemu trenerju
            FakeTerminDb.Termini.Add(new TerminVadbe
            {
                Id = 1,
                TrenerUsername = "trener2",
                DatumInCas = new DateTime(2069, 1, 1, 18, 0, 0),
                Lokacija = "Ljubljana"
            });

            var page = CreatePage(role: UserRole.Trener.ToString(), username: "trener1");

            // act
            var result = page.OnGet(1);

            // assert
            Assert.IsInstanceOfType(result, typeof(PageResult));
            Assert.IsNotNull(page.ErrorMessage);
            Assert.IsTrue(page.ErrorMessage.Contains("ni bil najden") || page.ErrorMessage.Contains("ni tvoj"));
            Assert.IsNull(page.Termin); // ker ni tvoj
        }

        [TestMethod]
        public void OnPost_ValidDelete_ShouldRemoveTerminAndItsReservations_AndRedirect()
        {
            // arrange
            FakeTerminDb.Termini.Add(new TerminVadbe
            {
                Id = 1,
                TrenerUsername = "trener1",
                DatumInCas = new DateTime(2069, 1, 1, 18, 0, 0),
                Lokacija = "MB"
            });

            FakeRezervacijeDb.Rezervacije.Add(new Rezervacija { Id = 1, TerminId = 1, UporabnikUsername = "user1" });
            FakeRezervacijeDb.Rezervacije.Add(new Rezervacija { Id = 2, TerminId = 1, UporabnikUsername = "user2" });
            FakeRezervacijeDb.Rezervacije.Add(new Rezervacija { Id = 3, TerminId = 999, UporabnikUsername = "user3" }); // mora ostati

            var page = CreatePage(role: UserRole.Trener.ToString(), username: "trener1");

            // act
            var result = page.OnPost(1);

            // assert: redirect
            var redirect = result as RedirectToPageResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("/Trener/Termini", redirect.PageName);

            // assert: termin pobrisan
            Assert.IsFalse(FakeTerminDb.Termini.Any(t => t.Id == 1));

            // assert: rezervacije za ta termin pobrisane
            Assert.IsFalse(FakeRezervacijeDb.Rezervacije.Any(r => r.TerminId == 1));

            // assert: druge rezervacije ostanejo
            Assert.IsTrue(FakeRezervacijeDb.Rezervacije.Any(r => r.TerminId == 999));
        }
    }
    [DoNotParallelize]
    [TestClass]
    public class TrenerIndexModelTests
    {
        private List<TerminVadbe> _backupTermini;
        private List<SledenjeTrenerju> _backupSledenja;

        [TestInitialize]
        public void Setup()
        {
            _backupTermini = FakeTerminDb.Termini.ToList();
            _backupSledenja = FakeSledenjeDb.Sledenja.ToList();

            FakeTerminDb.Termini.Clear();
            FakeSledenjeDb.Sledenja.Clear();
        }

        [TestCleanup]
        public void Cleanup()
        {
            FakeTerminDb.Termini.Clear();
            FakeSledenjeDb.Sledenja.Clear();

            FakeTerminDb.Termini.AddRange(_backupTermini);
            FakeSledenjeDb.Sledenja.AddRange(_backupSledenja);
        }

        private WebApplication6.Pages.Trener.IndexModel CreatePage(string username)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession();
            httpContext.Session.SetString("Username", username);

            var page = new WebApplication6.Pages.Trener.IndexModel();
            page.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
            {
                HttpContext = httpContext
            };
            return page;
        }


        [TestMethod]
        public void OnGet_ShouldComputeMinMaxPrice_ForTrainersTermini()
        {
            // arrange
            FakeTerminDb.Termini.Add(new TerminVadbe { Id = 1, TrenerUsername = "trener1", Cena = 10 });
            FakeTerminDb.Termini.Add(new TerminVadbe { Id = 2, TrenerUsername = "trener1", Cena = 25 });
            FakeTerminDb.Termini.Add(new TerminVadbe { Id = 3, TrenerUsername = "trener2", Cena = 999 });

            var page = CreatePage("trener1");

            // act
            page.OnGet();

            // assert
            Assert.AreEqual(10m, page.NajnizjaCena);
            Assert.AreEqual(25m, page.NajvisjaCena);
        }

        [TestMethod]
        public void OnGet_ShouldLoadDistinctFollowers_ForTrainer()
        {
            // arrange
            FakeSledenjeDb.Sledenja.Add(new SledenjeTrenerju { Id = 1, TrenerUsername = "trener1", UporabnikUsername = "u1" });
            FakeSledenjeDb.Sledenja.Add(new SledenjeTrenerju { Id = 2, TrenerUsername = "trener1", UporabnikUsername = "u1" }); // duplicate
            FakeSledenjeDb.Sledenja.Add(new SledenjeTrenerju { Id = 3, TrenerUsername = "trener1", UporabnikUsername = "u2" });
            FakeSledenjeDb.Sledenja.Add(new SledenjeTrenerju { Id = 4, TrenerUsername = "trener2", UporabnikUsername = "u3" });

            var page = CreatePage("trener1");

            // act
            page.OnGet();

            // assert
            Assert.AreEqual(2, page.Sledilci.Count);
            Assert.IsTrue(page.Sledilci.Contains("u1"));
            Assert.IsTrue(page.Sledilci.Contains("u2"));
        }
    }
}
