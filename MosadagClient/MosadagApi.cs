using System;
using System.Net.Http;
using System.Threading.Tasks;

using Supabase.Postgrest.Models;



namespace MosadagClient
{
    public sealed class MosadagApi
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
        public async Task SendWhatsAppOtp(string phone, string? username = null)
        {
            if (string.IsNullOrEmpty(username))
            {
                await SupabaseClient.Auth.SignInWithOtp(new Supabase.Gotrue.SignInWithPasswordlessPhoneOptions(phone)
                {
                    Channel = Supabase.Gotrue.SignInWithPasswordlessPhoneOptions.MessagingChannel.WHATSAPP,
                    Phone = phone,
                    ShouldCreateUser = false,
                });
            }
            else
            {
                await SupabaseClient.Auth.SignInWithOtp(new Supabase.Gotrue.SignInWithPasswordlessPhoneOptions(phone)
                {
                    Channel = Supabase.Gotrue.SignInWithPasswordlessPhoneOptions.MessagingChannel.WHATSAPP,
                    Phone = phone,
                    ShouldCreateUser = true,
                    Data = new System.Collections.Generic.Dictionary<string, object> { { "name", username }, { "display_name", username } }
                });
            }
        }
        public async Task<Supabase.Gotrue.Session?> VerifyOtp(string phone, string token)
        {
            return await SupabaseClient.Auth.VerifyOTP(phone, token, Supabase.Gotrue.Constants.MobileOtpType.SMS);
        }

        public SupabaseRepository<T> GetRepository<T>() where T : BaseModel, IBaseModel, new() => new SupabaseRepository<T>(SupabaseClient);
        public async Task init()
        {
            await SupabaseClient.InitializeAsync();
        }
    }
}
