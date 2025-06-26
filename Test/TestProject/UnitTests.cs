using System;
using System.IO;
using System.Text.Json;

using DotNetEnv;

using MosadagClient;
using MosadagClient.Model;
using MosadagClient.Services;

using Postgrest.Attributes;
using Postgrest.Models;

namespace TestProject
{
    public class UnitTests
    {
        MosadagApi mosadagClient;
        const string FakeHashedId = "fakedataid65415615";

        [SetUp]
        public void Setup()
        {
            var a = Env.Load(".env");
            mosadagClient = new MosadagApi(nopcommerceUrl: Environment.GetEnvironmentVariable("NC_URL"), supabaseUrl: Environment.GetEnvironmentVariable("SP_URL"), supabaseKey: Environment.GetEnvironmentVariable("SP_KEY"));

            mosadagClient.init(new FileSaveLoginDataService()).Wait();
        }

        [Test()]
        [Order(1)]
        public void latestlicenseTest()
        {
            var data = mosadagClient.NopcommerceClient.GetLatestDeviceLicenseAsync("1513wdfwq151454wqfd").Result;
            Assert.NotNull(data);
        }

        [Test]
        [Order(2)]
        public void LicenseTest()
        {
            var data = mosadagClient.NopcommerceClient.CheckLicenseAsync("{\"payload\":\"eyJMaWNlbnNlSWQiOiIxIiwiSXNzdWVkVG8iOiJlY2VjYXNlY3ZhZHZhIiwiSXNzdWVkQXQiOiIyMDI1LTA1LTEzVDEyOjM0OjA4LjYxNTUzNCIsIkV4cGlyeSI6IjIwMjktMTEtMjlUMDA6MDA6MDAiLCJMaWNlbnNlVHlwZSI6bnVsbCwiRmVhdHVyZXMiOm51bGwsIkRldmljZUhhc2giOiJ2ZHN2ZHN2ZHN2LHZzZHZkc3Zkc3ZkIHN2LHZkc3Zkc3ZkcyIsIkFwcElkIjpudWxsLCJNaW5pbXVtVmVyc2lvbiI6IjU1NTU1NSJ9\",\"signature\":\"TQ7kqc/70qST/iiwRYDOoq7SbcOl1uPF2JeQJgiNZoSfzpy6aC7KW\\u002B87CAEKG9f5Y8kpcWVBKRNiYocsNc/G/zs1k58NKForCSXrYxlFj9TyZcc9mV94wUEC3Zq\\u002By8QTW7A/86LdnS4TlXY/SLOcmuWYeQwcBwISeW3hlcw7pQwK3mpI3ZoV44bs7TJ5goDEdZm5txQpEVy4ARE8F7G3JXuOav8e7mHBwB0gOyoP4gldgS8A\\u002B5GGTcYJR8hOWAKH8fYYHZxiwuql91CtsHku8bAE4/tCK/G2m4G0raUHjGyEyxS1Lr2rLjevkFTIG0JMqQj8W4aSz/AQgClBmMKpqg==\"}").Result;
            Assert.NotNull(data);
        }
        [Test]
        [Order(3)]
        public void TestOtp()
        {
            mosadagClient.SendWhatsAppOtp("201556791003").Wait();
            var token = "0000000000";
            var res = mosadagClient.VerifyOtp("201556791003", token, null).Result;
            Assert.That(res != null);
        }
        [Test]
        [Order(4)]
        public void TestRepos()
        {
            var repo = mosadagClient.GetRepository<Transactions>();
            var page = repo.GetPaginatedAsync(new PaginationOptions() { PageNumber = 1, PageSize = 10, }).Result;


            Assert.NotNull(page);
            var data = repo.AddAsync(new Transactions() { DataJson = "Naaah" }).Result;
            Assert.NotNull(data);
            data.DataJson = "Naaah2";
            data = repo.UpdateAsync(data).Result;
            var res = repo.GetPaginatedAsync(new PaginationOptions() { PageSize = 1000, Filters = new Dictionary<string, object>() { { "id", data.Id.ToString() } } }).Result.Items.First();
            Assert.That(res != null && res.DataJson == "Naaah2");



            repo.DeleteAsync(data.Id).Wait();
            res = repo.GetSingleAsync(a => a.Id == data.Id).Result;
            Assert.That(res == null);

        }
    }

    [Table("Spendo.Transactions")]
    public class Transactions : BaseModel, IBaseModel
    {
        [PrimaryKey("id", false)] // false indicates this is not auto-incrementing
        public Guid Id { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("data")]
        public string? DataJson { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }
    }
    public class FileSaveLoginDataService : SaveLoginDataService
    {
        private readonly string _filePath;

        public FileSaveLoginDataService(string filePath = "logindata.json")
        {
            _filePath = filePath;
        }

        public override void DestroySession()
        {
            if (File.Exists(_filePath))
            {
                File.Delete(_filePath);
            }
        }

        public override LoginData? LoadSession()
        {
            if (!File.Exists(_filePath))
            {
                return null;
            }

            try
            {
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<LoginData>(json);
            }
            catch (Exception)
            {
                // Log error if needed
                return null;
            }
        }

        public override void SaveSession(LoginData session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            var json = JsonSerializer.Serialize(session);
            File.WriteAllText(_filePath, json);
        }
    }


}