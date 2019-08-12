using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace VCAuthn.ACAPy
{
    public interface IACAPYClient
    {
        Task<bool> CreatePresentationExchange(PresentationConfiguration.PresentationConfiguration presentationConfiguration);
        Task<WalletDidPublicResponse> WalletDidPublic();
    }

    public class ACAPYClient : IACAPYClient
    {
        private readonly ILogger<ACAPYClient> _logger;
        private readonly string _baseUrl;
        private HttpClient _httpClient;

        public ACAPYClient(IConfiguration config, ILogger<ACAPYClient> logger)
        {
            _httpClient = new HttpClient();
            _logger = logger;
            _baseUrl = config.GetValue<string>("BaseUrl");
        }
        
        public async Task<bool> CreatePresentationExchange(PresentationConfiguration.PresentationConfiguration presentationConfiguration)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"{_baseUrl}/presentation_exchange/create_request"),
                Content = new StringContent(presentationConfiguration.ToJson(), Encoding.UTF8, MediaTypeNames.Application.Json)
            };

            try
            {
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogDebug($"Status: [{response.StatusCode}], Content: [{responseContent}, Headers: [{response.Headers}]");

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return true;
                    default:
                        throw new Exception($"Presentation Exchange creation error. Code: {response.StatusCode}");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create Presentation Exchange", e);
            }
        }
        
        public async Task<WalletDidPublicResponse> WalletDidPublic()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"{_baseUrl}/wallet/did/public")
            };

            try
            {
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogDebug($"Status: [{response.StatusCode}], Content: [{responseContent}, Headers: [{response.Headers}]");

                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return JsonConvert.DeserializeObject<WalletDidPublicResponse>(responseContent) ;
                    default:
                        throw new Exception($"Wallet Did public request error. Code: {response.StatusCode}");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Wallet Did public request failed.", e);
            }
        }
    }
}