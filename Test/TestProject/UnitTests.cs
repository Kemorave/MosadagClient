using DotNetEnv;

using MosadagClient;

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

            mosadagClient.init().Wait();
        }

        [Test()]
        [Order(2)]
        public void latestlicenseTest()
        {
            var data = mosadagClient.NopcommerceClient.GetlatestlicenseAsync("1513wdfwq151454wqfd").Result;
            Assert.NotNull(data);
        }

        [Test]
        [Order(4)]
        public void LicenseTest()
        {
            var data = mosadagClient.NopcommerceClient.ChecklicenseAsync("{\"payload\":\"eyJMaWNlbnNlSWQiOiIxIiwiSXNzdWVkVG8iOiJlY2VjYXNlY3ZhZHZhIiwiSXNzdWVkQXQiOiIyMDI1LTA1LTEzVDEyOjM0OjA4LjYxNTUzNCIsIkV4cGlyeSI6IjIwMjktMTEtMjlUMDA6MDA6MDAiLCJMaWNlbnNlVHlwZSI6bnVsbCwiRmVhdHVyZXMiOm51bGwsIkRldmljZUhhc2giOiJ2ZHN2ZHN2ZHN2LHZzZHZkc3Zkc3ZkIHN2LHZkc3Zkc3ZkcyIsIkFwcElkIjpudWxsLCJNaW5pbXVtVmVyc2lvbiI6IjU1NTU1NSJ9\",\"signature\":\"TQ7kqc/70qST/iiwRYDOoq7SbcOl1uPF2JeQJgiNZoSfzpy6aC7KW\\u002B87CAEKG9f5Y8kpcWVBKRNiYocsNc/G/zs1k58NKForCSXrYxlFj9TyZcc9mV94wUEC3Zq\\u002By8QTW7A/86LdnS4TlXY/SLOcmuWYeQwcBwISeW3hlcw7pQwK3mpI3ZoV44bs7TJ5goDEdZm5txQpEVy4ARE8F7G3JXuOav8e7mHBwB0gOyoP4gldgS8A\\u002B5GGTcYJR8hOWAKH8fYYHZxiwuql91CtsHku8bAE4/tCK/G2m4G0raUHjGyEyxS1Lr2rLjevkFTIG0JMqQj8W4aSz/AQgClBmMKpqg==\"}").Result;
            Assert.NotNull(data);
        }
    }

}