using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace  Microsoft.eShopWeb.ApplicationCore.Services
{
    public class HttpBlobOrderCreatorService : IOrderCreatorService
    {
        private readonly HttpClient _httpClient;
        private readonly IAppLogger<IOrderCreatorService> _logger;

        public HttpBlobOrderCreatorService(HttpClient httpClient,
            IAppLogger<IOrderCreatorService> logger)
        { 
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<int> SaveOrderAsync(Order order, CancellationToken token = default)
        {
            var response = await _httpClient.PostAsJsonAsync("", order, token);
            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync(token);
                _logger.LogWarning(msg);
                return 0;
            }
            
            response.EnsureSuccessStatusCode();

            return order.Id;
        } 
    }
}