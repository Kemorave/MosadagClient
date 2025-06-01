using MosadagClient;
using MosadagClient.Models;

namespace TestProject
{
    public class UnitTests
    {
        MosadagApi mosadagClient = new MosadagApi("https://store.ensight-tech.de", "");
        const string FakeHashedId = "fakedataid65415615";
        [SetUp]
        public void Setup()
        {
            mosadagClient.initClient().Wait();
        }


        [Test()]
        [Order(0)]
        public void LoginTest()
        {
            var login = mosadagClient.Client.LoginAsync("1.0.01", "1513wdfwq151454wqfd", "China Mobile", "123456789", "0121127623").Result;
            mosadagClient.SetAuthorize(login.Token);
            Assert.NotNull(login);
        }

        [Test()]
        [Order(1)]
        public void ProfileTest()
        {
            var data = mosadagClient.Client.ProfileAsync().Result;
            Assert.NotNull(data);
        }
        [Test()]
        [Order(2)]
        public void latestlicenseTest()
        {
            var data = mosadagClient.Client.GetlatestlicenseAsync("1513wdfwq151454wqfd").Result;
            Assert.NotNull(data);
        }
        [Test()]
        [Order(3)]
        public void RegisterTest()
        {
            mosadagClient.SetAuthorize(null);
            var data = mosadagClient.Client.RegisterAsync(address: "123 Main St, Anytown, USA",
    email: "john.doe@example.com",
    firstName: "John",
    lastName: "Doe",
    password: "P@ssw0rd",
    phone: "555-123-4567",
    username: "johndoe123").Result;
            Assert.NotNull(data);
        }
        [Test]
        [Order(4)]
        public void LicenseTest()
        {
            var data = mosadagClient.Client.ChecklicenseAsync("{\"payload\":\"eyJMaWNlbnNlSWQiOiIxIiwiSXNzdWVkVG8iOiJlY2VjYXNlY3ZhZHZhIiwiSXNzdWVkQXQiOiIyMDI1LTA1LTEzVDEyOjM0OjA4LjYxNTUzNCIsIkV4cGlyeSI6IjIwMjktMTEtMjlUMDA6MDA6MDAiLCJMaWNlbnNlVHlwZSI6bnVsbCwiRmVhdHVyZXMiOm51bGwsIkRldmljZUhhc2giOiJ2ZHN2ZHN2ZHN2LHZzZHZkc3Zkc3ZkIHN2LHZkc3Zkc3ZkcyIsIkFwcElkIjpudWxsLCJNaW5pbXVtVmVyc2lvbiI6IjU1NTU1NSJ9\",\"signature\":\"TQ7kqc/70qST/iiwRYDOoq7SbcOl1uPF2JeQJgiNZoSfzpy6aC7KW\\u002B87CAEKG9f5Y8kpcWVBKRNiYocsNc/G/zs1k58NKForCSXrYxlFj9TyZcc9mV94wUEC3Zq\\u002By8QTW7A/86LdnS4TlXY/SLOcmuWYeQwcBwISeW3hlcw7pQwK3mpI3ZoV44bs7TJ5goDEdZm5txQpEVy4ARE8F7G3JXuOav8e7mHBwB0gOyoP4gldgS8A\\u002B5GGTcYJR8hOWAKH8fYYHZxiwuql91CtsHku8bAE4/tCK/G2m4G0raUHjGyEyxS1Lr2rLjevkFTIG0JMqQj8W4aSz/AQgClBmMKpqg==\"}").Result;
            Assert.NotNull(data);
        }
        [Test]
        [Order(5)]
        public void TestSettings()
        {
            var data = mosadagClient.UpdateOrCreateCloudSettings(FakeHashedId, new SettingsTest() { FakeData = "fakeData", }).Result;
            Assert.NotNull(data);
            data.FakeData = "fakeData1";
            data = mosadagClient.UpdateOrCreateCloudSettings(FakeHashedId, data).Result;
            data = mosadagClient.GetOrCreateCloudSettings<SettingsTest>(FakeHashedId).Result;
            Assert.That(data.FakeData == "fakeData1");
        }
        [Test]
        [Order(6)]
        public void TestTransactions()
        {
            mosadagClient.AddCloudTransactions(FakeHashedId, new List<TransactionsTest>() { new TransactionsTest() { FakeData = "fakeData", } }).Wait();
            var data = mosadagClient.GetAllCloudTransactions<TransactionsTest>(FakeHashedId).Result;
            Assert.That(data.Any(a => a.FakeData == "fakeData"));
            //"192.168.1.50"
        }
    }
    public class SettingsTest : IEntityModel
    {
        public long Id { get; set; }
        public string? FakeData { get; set; }
    }
    public class TransactionsTest : ISyncedEntityModel
    {
        public long Id { get; set; }
        public string? FakeData { get; set; }
        public bool IsSynced { get; set; }
    }
}