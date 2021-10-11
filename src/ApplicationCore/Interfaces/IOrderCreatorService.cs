using System.Threading;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;

namespace Microsoft.eShopWeb.ApplicationCore.Interfaces
{
    public interface IOrderCreatorService
    {
        Task<int> SaveOrderAsync(Order order, CancellationToken token = default);
    }
}