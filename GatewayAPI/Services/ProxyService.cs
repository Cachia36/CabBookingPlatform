using System.Text.Json;

namespace GatewayAPI.Services
{
    public class ProxyService
    {
        private readonly IHttpClientFactory _factory;
        private readonly IConfiguration _config;

        public ProxyService(IHttpClientFactory factory, IConfiguration config)
        {
            _factory = factory;
            _config = config;
        }
        public async Task<HttpResponseMessage> ForwardAsync(string serviceKey, string endpoint, HttpMethod method, object? data = null, CancellationToken ct = default)
        {
            var client = _factory.CreateClient();

            var baseUrl = _config[$"Services:{serviceKey}"]
                          ?? throw new InvalidOperationException($"Missing Services:{serviceKey} in config.");

            // Normalize and combine
            var baseUri = new Uri(baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/");
            var relative = endpoint.TrimStart('/');
            var requestUri = new Uri(baseUri, relative);

            Console.WriteLine($"Proxying to: {requestUri}");

            var request = new HttpRequestMessage(method, requestUri);

            if (data != null)
            {
                var json = JsonSerializer.Serialize(data);
                request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }

            return await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
        }

    }
}
