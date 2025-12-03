using WebApplication6.Models;

namespace WebApplication6.Tests
{
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
            FakeTerminDb.Termini.Add(new TerminVadbe { Id = 99, DatumInCas = DateTime.Now.AddDays(1), Lokacija = "MB" });
            Assert.AreEqual(before + 1, FakeTerminDb.Termini.Count);
        }

        [TestMethod]
        public void Test_InvalidTermin_ShouldBePastDate()
        {
            var invalid = new TerminVadbe { DatumInCas = DateTime.Now.AddDays(-1) };
            Assert.IsTrue(invalid.DatumInCas < DateTime.Now);
        }
    }
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
    [TestClass]
    public class RezervacijaTests
    {
        [TestInitialize]
        public void Setup()
        {
            // počisti obstoječe rezervacije pred vsakim testom
            FakeRezervacijeDb.Rezervacije.Clear();

            // resetiraj termine z enim obstoječim (kot v aplikaciji)
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
            // arrange
            var user = "user1";
            var terminId = 1;

            // act
            FakeRezervacijeDb.Rezervacije.Add(new Rezervacija
            {
                Id = 1,
                TerminId = terminId,
                UporabnikUsername = user
            });

            // assert
            Assert.AreEqual(1, FakeRezervacijeDb.Rezervacije.Count);
            var r = FakeRezervacijeDb.Rezervacije.First();
            Assert.AreEqual(user, r.UporabnikUsername);
            Assert.AreEqual(terminId, r.TerminId);
        }

        [TestMethod]
        public void Test_DvojnaRezervacijaNiDovoljena()
        {
            // arrange
            var user = "user1";
            var terminId = 1;

            // uporabnik že ima eno rezervacijo
            FakeRezervacijeDb.Rezervacije.Add(new Rezervacija
            {
                Id = 1,
                TerminId = terminId,
                UporabnikUsername = user
            });

            // act – poskusimo dodati isto rezervacijo
            var alreadyExists = FakeRezervacijeDb.Rezervacije
                .Any(r => r.TerminId == terminId && r.UporabnikUsername == user);

            // assert
            Assert.IsTrue(alreadyExists, "Uporabnik bi moral že imeti to rezervacijo.");
        }

        [TestMethod]
        public void Test_RezervacijaNaNeobstoječTermin()
        {
            // arrange
            var user = "user1";
            var neobstojeciTerminId = 999;

            // act
            var obstajaTermin = FakeTerminDb.Termini.Any(t => t.Id == neobstojeciTerminId);

            // assert
            Assert.IsFalse(obstajaTermin, "Termin z ID 999 ne bi smel obstajati.");
        }
    }
}
