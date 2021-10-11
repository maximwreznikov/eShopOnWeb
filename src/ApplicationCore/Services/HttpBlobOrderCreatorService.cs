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

        public HttpBlobOrderCreatorService(HttpClient httpClient) 
            => _httpClient = httpClient;

        public async Task<int> SaveOrderAsync(Order order, CancellationToken token = default)
        {
            var response = await _httpClient.PostAsJsonAsync("", order, token);
            response.EnsureSuccessStatusCode();

            return order.Id;
        } 
    }
}