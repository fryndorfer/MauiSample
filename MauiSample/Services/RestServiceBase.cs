using MauiSample.Models;
using MauiSample.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace MauiSample.Services
{
    public class RestServiceBase : IRestServiceBase
    {
        private readonly HttpClient client;
        ILogger<RestServiceBase> _logger;
        protected readonly IHttpClientFactory _httpClientFactory;
        public string M42ApiPrefix { get; set; } = "/m42Services/api";

        private JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public RestServiceBase(IHttpClientFactory httpClientFactory, ILogger<RestServiceBase> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            client = _httpClientFactory.CreateClient("app");
            _logger = logger;

            if (client != null)
            {
                //var token = GetToken();
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Result.ApiToken); // TODO BEARER TOKEN NOT WORKING ON /data/fragments -> 406

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
                client.DefaultRequestHeaders.Add("Accept-Language", "de,en-US;q=0.7,en;q=0.3");
                client.DefaultRequestHeaders.Add("User-Agent", $"cubefinityMobileApp/1.0 {"Unknown"}"); // TODO insert useragent from WebView.UserAgent
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
            }
        }

        public async Task<TokenResponse?> GetToken()
        {

            var tokenRequest = new TokenRequest { Enabled = true, ExpirationDays = 14, Name = "mobileapptoken" }; // TODO GET NEW TOKEN
            var content = new StringContent(JsonSerializer.Serialize(tokenRequest), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/M42Services/api/apitoken/generateapitokenforme", content);
            var responseData = await response.Content.ReadAsStreamAsync();
            var tokenData = await JsonSerializer.DeserializeAsync<TokenResponse>(responseData, _jsonOptions);
            return tokenData;
        }

        public async Task<T?> GetAsync<T>(string url)
        {            
            using var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var publicMessage = $"Error accessing {url}. Status code: {(int)response.StatusCode}";
                _logger.LogDebug($"{publicMessage} Message: {response.ToString()}");
                var message = await response.Content.ReadAsStringAsync(); Debug.WriteLine(message);
                throw new HttpRequestException(publicMessage);
            }

            var content = await response.Content.ReadAsStringAsync(); Debug.WriteLine(content);

            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data)
        {
            //using var client = _httpClientFactory.CreateClient();
            JsonSerializerOptions options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault, PropertyNameCaseInsensitive = true };
            var json = JsonSerializer.Serialize(data, options);

            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync(); Debug.WriteLine(message);
                throw new HttpRequestException($"Error posting to {url}. Status code: {(int)response.StatusCode} Error: {message}");
            }

            var responseData = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse?>(responseData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<string> PutAsync<TRequest>(string url, TRequest data)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

            var json = JsonSerializer.Serialize(data, options);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await client.PutAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error updating resource at {url}. Status code: {(int)response.StatusCode}");
            }

            var responseData = await response.Content.ReadAsStringAsync();

            return responseData;
        }
        public async Task<bool> DeleteAsync(string url)
        {
            //using var client = _httpClientFactory.CreateClient();
            using var response = await client.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync(); Debug.WriteLine(message);
                throw new HttpRequestException($"Error deleting resource at {url}. Status code: {(int)response.StatusCode} Error: {message}");
                return false;
            }

            return true;
        }

        ~RestServiceBase()
        {
            this.client.CancelPendingRequests();
            this.client.Dispose();
        }
    }
}
