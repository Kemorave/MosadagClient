using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

using MosadagClient.Services;

using Postgrest.Models;

using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;




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
        public SaveLoginDataService DataService { get; private set; }

        /// <summary>
        /// Sets or removes the authorization token for the HTTP client.
        /// </summary>
        /// <param name="token">The authorization token to set. If null or empty, the authorization header is removed.</param>
        /// <remarks>No return value.</remarks>
        private void _SetNopAuthorize(string? token)
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
        public async Task Logout()
        {
            _SetNopAuthorize(null);
            await SupabaseClient.Auth.SignOut();
            DataService.DestroySession();
        }
        public async Task SendWhatsAppOtp(string phone, string? username = null)
        {
            if (string.IsNullOrEmpty(username))
            {
                await NopcommerceClient.LoginWithOTPAsync(new OtpLoginModel() { PhoneNumber = phone, UserName = username, RegisteredOnly = true });
            }
            else
            {
                await NopcommerceClient.LoginWithOTPAsync(new OtpLoginModel() { PhoneNumber = phone, UserName = username, RegisteredOnly = false });

            }
        }
        public async Task<AuthRes> VerifyOtp(string phone, string token, DeviceData deviceData)
        {
            var res = await NopcommerceClient.VerifyOTPAsync(new VerifyOTPModel() { PhoneNumber = phone, Code = token, DeviceData = deviceData });
            var session = await SupabaseClient.Auth.SetSession(res.SupabaseAccessToken, res.SupabaseRefreshToken,true);
            await SupabaseClient.Auth.Update(new Supabase.Gotrue.UserAttributes()
            {
                Data = new System.Collections.Generic.Dictionary<string, object>() { { "nop_token", res.Token } }
            });
            _SetNopAuthorize(res.Token);
            DataService.SaveSession(new Model.LoginData() { NopcommerceToken = res.Token, Session = session });
            return res;
        }

        public SupabaseRepository<T> GetRepository<T>() where T : BaseModel, IBaseModel, new() => new SupabaseRepository<T>(SupabaseClient);
        public async Task init(SaveLoginDataService dataService)
        {
            if (dataService is null)
            {
                throw new ArgumentNullException(nameof(dataService));
            }
            this.DataService = dataService;
            var session = dataService?.LoadSession();
            var nopToken = session?.NopcommerceToken;

            SupabaseClient.Auth.SetPersistence(new SaveLoginDataWrapperService(service: dataService!));
            await SupabaseClient.InitializeAsync();
            SupabaseClient.Auth.LoadSession();
            if (! string.IsNullOrEmpty(nopToken))
            {
                _SetNopAuthorize(nopToken);
            }
        }
    }

}
