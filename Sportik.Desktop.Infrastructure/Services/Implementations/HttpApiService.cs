using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sportik.Desktop.Infrastructure.Services.Interfaces;

namespace Sportik.Desktop.Infrastructure.Services.Implementations
{
    internal sealed class HttpApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public HttpApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> GetAsync<T>(string endpoint, string token = null, CancellationToken cancellationToken = default)
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            HttpResponseMessage response = await _httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            cancellationToken.ThrowIfCancellationRequested();

            var e = JsonConvert.DeserializeObject<T>(responseJson);
            return e;
        }

        public async Task<T> PostAsync<T>(string endpoint, object data, string token = null,
            CancellationToken cancellationToken = default)
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            string jsonData = JsonConvert.SerializeObject(data);
            using HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            cancellationToken.ThrowIfCancellationRequested();

            return JsonConvert.DeserializeObject<T>(responseJson);
        }

        public async Task<T> PutAsync<T>(string endpoint, object data, string token = null, CancellationToken cancellationToken = default)
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            string jsonData = JsonConvert.SerializeObject(data);
            using HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(endpoint, content, cancellationToken);
            response.EnsureSuccessStatusCode();

            string responseJson = await response.Content.ReadAsStringAsync();
            cancellationToken.ThrowIfCancellationRequested();

            return JsonConvert.DeserializeObject<T>(responseJson);
        }
    }
}