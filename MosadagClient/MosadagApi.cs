using System;
using System.Net.Http;
using System.Threading.Tasks;

using Supabase.Postgrest.Models;



namespace MosadagClient
{
    public class MosadagApi
    {
        public MosadagApi(string nopcommerceUrl, string supabaseUrl, string supabaseKey)
        {
            _httpClient = new System.Net.Http.HttpClient() { BaseAddress = new Uri(nopcommerceUrl), };
            NopcommerceClient = new Client(_httpClient);
            SupabaseClient = new Supabase.Client(supabaseUrl, supabaseKey);
        }
        public Supabase.Client SupabaseClient { get; set; }
        HttpClient _httpClient;
        public Client NopcommerceClient { get; }

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
        public SupabaseRepository<T> GetRepository<T>() where T : BaseModel, IBaseModel, new() => new SupabaseRepository<T>(SupabaseClient);
        public async Task init()
        {
            await SupabaseClient.InitializeAsync();
        }
    }
}
