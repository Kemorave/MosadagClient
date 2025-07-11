using System;
using System.Net.Http;
using System.Threading.Tasks;

using MosadagClient.Services;

using Supabase;
using Supabase.Gotrue;
using Supabase.Interfaces;
using Supabase.Postgrest.Models;





namespace MosadagClient
{
    public sealed class MosadagApi
    {
        public MosadagApi(string nopcommerceUrl, string supabaseUrl, string supabaseKey)
        {
            _httpClient = new System.Net.Http.HttpClient() { BaseAddress = new Uri(nopcommerceUrl), };
            NopcommerceClient = new Client(_httpClient);
            SupabaseClient = new Supabase.Client(supabaseUrl, supabaseKey, new SupabaseOptions() { AutoRefreshToken = true, StorageClientOptions = new Supabase.Storage.ClientOptions() { HttpRequestTimeout = TimeSpan.FromMinutes(10), } });
        }
        public Supabase.Client SupabaseClient { get; set; }
        HttpClient _httpClient;
        public Client NopcommerceClient { get; }
        public SaveLoginDataService? DataService { get; private set; }

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
            DataService?.DestroySession();
        }
        public async Task SendWhatsAppOtp(string phone, string? username = null, SignInWithPasswordlessPhoneOptions.MessagingChannel channel = SignInWithPasswordlessPhoneOptions.MessagingChannel.WHATSAPP)
        {
            if (string.IsNullOrEmpty(username))
            {
                await SupabaseClient.Auth.SignInWithOtp(new SignInWithPasswordlessPhoneOptions(phone) { Channel = channel, ShouldCreateUser = false });
            }
            else
            {
                await SupabaseClient.Auth.SignInWithOtp(new SignInWithPasswordlessPhoneOptions(phone) { Channel = channel, ShouldCreateUser = true, Data = new System.Collections.Generic.Dictionary<string, object>() { { "display_name", username ?? "" } } });
            }
        }
        public async Task<AuthRes> VerifyOtp(string phone, string token, DeviceData deviceData)
        {
            var session = await SupabaseClient.Auth.VerifyOTP( phone, token, Supabase.Gotrue.Constants.MobileOtpType.SMS);
            if (session==null)
            {
                throw new Exception("Session is null");
            }
            var res = await NopcommerceClient.VerifySupbaseTokenAsync(new VerifySupbaseTokenModel() { DeviceData = deviceData, Token = session.AccessToken, PhoneNumber = session.User!.Phone, });
            _SetNopAuthorize(res.Token);
            DataService?.SaveSession(new Model.LoginData() { NopcommerceToken = res.Token, Session = session });
            return res;
        }

        public SupabaseRepository<T> GetRepository<T>() where T : BaseModel, IBaseModel, new() => new SupabaseRepository<T>(this);
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
            if (!string.IsNullOrEmpty(nopToken))
            {
                _SetNopAuthorize(nopToken);
                await RefreshToken();
            }
        }
        public async Task<T> MakeRequestWithRefresh<T>(Func<Task<T>> apiCall)
        {
            try
            {
                return await apiCall();
            }
            //catch (PostgrestException ex) when (ex.Message.Contains("PGRST301") || ex.Message.Contains("JWT expired"))
            //{
            //    // Refresh token and retry
            //    await RefreshToken();
            //    return await apiCall();
            //}
            catch
            {
                // Refresh token and retry
                await RefreshToken();
                return await apiCall();
            }
        }
        public async Task RefreshToken()
        {
            try
            {
                await SupabaseClient.Auth.RefreshSession();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }

}
