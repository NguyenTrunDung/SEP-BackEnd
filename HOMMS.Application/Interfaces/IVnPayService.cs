using HOMMS.Domain.Dtos;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace HOMMS.Application.Interfaces
{
    public interface IVnPayService
    {
        Task<string> GeneratePaymentUrlAsync(int orderId, int amount);
        Task<VnPayReturnDto> ProcessVnPayReturnAsync(IQueryCollection query, bool isIpn = false);
    }
}
