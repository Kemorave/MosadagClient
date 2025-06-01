using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using MosadagClient.Models;

using Newtonsoft.Json;

using Supabase.Postgrest;

using static Supabase.Postgrest.Constants;
using static Supabase.Postgrest.QueryOptions;

namespace MosadagClient
{
    public class MosadagApi
    {
        public MosadagApi(string baseUrl)
        {
            _httpClient = new System.Net.Http.HttpClient() { BaseAddress = new Uri(baseUrl), };
            Client = new Client(_httpClient);
            supabase = new Supabase.Client(baseUrl, "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyAgCiAgICAicm9sZSI6ICJhbm9uIiwKICAgICJpc3MiOiAic3VwYWJhc2UtZGVtbyIsCiAgICAiaWF0IjogMTY0MTc2OTIwMCwKICAgICJleHAiOiAxNzk5NTM1NjAwCn0.dc_X5iR_VP_qT0zsiyj_I_OZ2T9FtRU2BBNWN8Bu4GE");
        }
        HttpClient _httpClient;
        public Client Client { get; }

        private readonly Supabase.Client supabase;

        public async Task initClient()
        {
            await supabase.InitializeAsync();
        }

        public async Task<T> GetOrCreateCloudSettings<T>(string deviceOrUserId) where T : IEntityModel, new()
        {
            var data = await _GetCloudSettings<T>(deviceOrUserId);
            if (data == null || data.Id <= 0)
            {
                return await UpdateOrCreateCloudSettings(deviceOrUserId, new T());
            }
            return data;
        }

        private async Task<T> _GetCloudSettings<T>(string deviceOrUserId) where T : IEntityModel, new()
        {
            var settings = await supabase.From<UserSettings>().Select("*")
            .Filter("hashedId", Operator.Equals, deviceOrUserId)
            .Single();
            if (settings == null || settings.Data == null || string.IsNullOrEmpty(settings.Data)) return new T();
            var data = JsonConvert.DeserializeObject<T>(settings.Data);
            if (data == null) return new T();
            data.Id = settings.Id;
            return data;
        }

        public async Task<T> UpdateOrCreateCloudSettings<T>(string deviceOrUserId, T settings) where T : IEntityModel, new()
        {
            var json = JsonConvert.SerializeObject(settings);
            if (settings.Id <= 0)
            {
                var result = await supabase.From<UserSettings>().Insert(new UserSettings { Data = json, HashedId = deviceOrUserId }, new QueryOptions { Returning = ReturnType.Representation });
                settings.Id = result!.Model!.Id;
            }
            else
            {
                await supabase.From<UserSettings>().Update(new UserSettings { Data = json, Id = settings.Id, HashedId = deviceOrUserId });
            }

            return settings;
        }
        public async Task<List<T>> GetAllCloudTransactions<T>(string deviceOrUserId) where T : ISyncedEntityModel, new()
        {
            var transactions = await supabase.From<UserTransactions>()
                .Select("*")
                .Filter("hashedId", Operator.Equals, deviceOrUserId)
                .Get();
            if (transactions == null || transactions.Models == null || transactions.Models.Count == 0)
                return new List<T>();
            var result = new List<T>();
            foreach (var item in transactions.Models)
            {
                if (item == null || item.Data == null || string.IsNullOrEmpty(item.Data))
                {
                    continue;
                }
                var data = JsonConvert.DeserializeObject<T>(item.Data);
                if (data == null)
                {
                    continue;
                }
                data.Id = item.Id;
                data.IsSynced = true;
                result.Add(data);
            }
            return result;
        }

        public async Task AddCloudTransactions<T>(string deviceOrUserId, IEnumerable<T> transactions) where T : ISyncedEntityModel, new()
        {
            if (transactions == null || transactions.Count() == 0)
            {
                return;
            }
            var data = transactions.Select(x => new UserTransactions { Data = JsonConvert.SerializeObject(x), HashedId = deviceOrUserId, }).ToList();
            await supabase.From<UserTransactions>().Insert(data);
            foreach (var item in transactions)
            {
                item.IsSynced = true;
            }
        }





        /// <summary>
        /// Sets or removes the authorization token for the HTTP client.
        /// </summary>
        /// <param name="token">The authorization token to set. If null or empty, the authorization header is removed.</param>
        /// <remarks>No return value.</remarks>
        public void SetAuthorize(string? token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }
    }
}
