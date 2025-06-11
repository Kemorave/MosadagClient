using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
 


namespace MosadagClient
{
    public class MosadagApi
    {
        public MosadagApi(string baseUrl, string key)
        {
            _httpClient = new System.Net.Http.HttpClient() { BaseAddress = new Uri(baseUrl), };
            Client = new Client(_httpClient);

        }
        HttpClient _httpClient;
        public Client Client { get; }

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
