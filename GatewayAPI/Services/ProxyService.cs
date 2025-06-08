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

        public async Task<HttpResponseMessage> ForwardAsync(string serviceKey, string endpoint, HttpMethod method, object? data = null)
        {
            var client = _factory.CreateClient();
            var baseUrl = _config[$"Services:{serviceKey}"];
            var url = $"{baseUrl}/{endpoint}";
            Console.WriteLine($"Proxying to: {url}");

            var request = new HttpRequestMessage(method, $"{baseUrl}/{endpoint}");

            if(data != null)
            {
                var json = JsonSerializer.Serialize(data);
                request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            }
            return await client.SendAsync(request);
        }
    }
}
