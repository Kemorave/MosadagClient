using MosadagClient;

namespace TestProject
{
    public class UnitTest1
    {
        Client client = new Client(new HttpClient() { BaseAddress = new Uri("https://localhost:55336") });
        [Fact]
        public async void Test1()
        {
            var login = await client.LoginAsync("1.0.01", "1513wdfwq151454wqfd", "China Mobile", "123456789", "0121127623");
            Assert.NotNull(login);
        }
        [Fact]
        public async void LicenseTest()
        {
            var data = await client.ChecklicenseAsync("{\"payload\":\"eyJMaWNlbnNlSWQiOiIxIiwiSXNzdWVkVG8iOiJlY2VjYXNlY3ZhZHZhIiwiSXNzdWVkQXQiOiIyMDI1LTA1LTEzVDEyOjM0OjA4LjYxNTUzNCIsIkV4cGlyeSI6IjIwMjktMTEtMjlUMDA6MDA6MDAiLCJMaWNlbnNlVHlwZSI6bnVsbCwiRmVhdHVyZXMiOm51bGwsIkRldmljZUhhc2giOiJ2ZHN2ZHN2ZHN2LHZzZHZkc3Zkc3ZkIHN2LHZkc3Zkc3ZkcyIsIkFwcElkIjpudWxsLCJNaW5pbXVtVmVyc2lvbiI6IjU1NTU1NSJ9\",\"signature\":\"TQ7kqc/70qST/iiwRYDOoq7SbcOl1uPF2JeQJgiNZoSfzpy6aC7KW\\u002B87CAEKG9f5Y8kpcWVBKRNiYocsNc/G/zs1k58NKForCSXrYxlFj9TyZcc9mV94wUEC3Zq\\u002By8QTW7A/86LdnS4TlXY/SLOcmuWYeQwcBwISeW3hlcw7pQwK3mpI3ZoV44bs7TJ5goDEdZm5txQpEVy4ARE8F7G3JXuOav8e7mHBwB0gOyoP4gldgS8A\\u002B5GGTcYJR8hOWAKH8fYYHZxiwuql91CtsHku8bAE4/tCK/G2m4G0raUHjGyEyxS1Lr2rLjevkFTIG0JMqQj8W4aSz/AQgClBmMKpqg==\"}");
            Assert.NotNull(data);
        }
    }
}