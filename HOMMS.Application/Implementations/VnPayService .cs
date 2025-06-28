using HOMMS.Application.Interfaces;
using HOMMS.Domain.Dtos;
using HOMMS.Domain.Enums;
using HOMMS.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace HOMMS.Application.Implementations
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public VnPayService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GeneratePaymentUrlAsync(int orderId, int amount)
        {
            var vnp_Url = _configuration["VnPay:Url"];
            var vnp_Returnurl = _configuration["VnPay:ReturnUrl"];
            var vnp_TmnCode = _configuration["VnPay:TmnCode"];
            var vnp_HashSecret = _configuration["VnPay:HashSecret"];

            string txnRef = orderId.ToString();
            string createDate = DateTime.Now.ToString("yyyyMMddHHmmss");

            var vnp_Params = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", vnp_TmnCode },
                { "vnp_Amount", (amount * 100).ToString() },
                { "vnp_CreateDate", createDate },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", "127.0.0.1" },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", $"Thanh toán đơn hàng #{orderId}" },
                { "vnp_OrderType", "billpayment" },
                { "vnp_ReturnUrl", vnp_Returnurl },
                { "vnp_TxnRef", txnRef }
            };

            var rawData = string.Join('&', vnp_Params.Select(kv => $"{kv.Key}={kv.Value}"));
            var hash = CreateSha256Hash(vnp_HashSecret + rawData);

            vnp_Params.Add("vnp_SecureHashType", "SHA256");
            vnp_Params.Add("vnp_SecureHash", hash);

            var paymentUrl = QueryHelpers.AddQueryString(vnp_Url, vnp_Params);
            return await Task.FromResult(paymentUrl);
        }

        public async Task<VnPayReturnDto> ProcessVnPayReturnAsync(IQueryCollection query, bool isIpn = false)
        {
            var vnp_HashSecret = _configuration["VnPay:HashSecret"];
            var vnp_SecureHash = query["vnp_SecureHash"].ToString();
            var vnp_ResponseCode = query["vnp_ResponseCode"].ToString();
            var vnp_TxnRef = query["vnp_TxnRef"].ToString();

            // Bước 1: Xử lý tạo chuỗi để so sánh hash
            var filtered = query
                .Where(kv => kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
                .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

            var sorted = new SortedDictionary<string, string>(filtered);
            var rawData = string.Join("&", sorted.Select(kv => $"{kv.Key}={kv.Value}"));
            var calculatedHash = CreateSha256Hash(vnp_HashSecret + rawData);

            // Bước 2: So sánh hash để xác minh
            if (!string.Equals(calculatedHash, vnp_SecureHash, StringComparison.OrdinalIgnoreCase))
                return new VnPayReturnDto(false, "Sai chữ ký (Secure Hash không hợp lệ)");

            // Bước 3: Kiểm tra mã phản hồi từ VNPAY
            if (vnp_ResponseCode == "00")
            {
                if (!int.TryParse(vnp_TxnRef, out var orderId))
                    return new VnPayReturnDto(false, "Mã đơn hàng không hợp lệ");

                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return new VnPayReturnDto(false, "Không tìm thấy đơn hàng");

                // Bước 4: Cập nhật trạng thái thanh toán
                if (!order.IsPaid) // tránh cập nhật lại đơn đã thanh toán
                {
                    order.IsPaid = true;
                    order.PaymentMethod = OrderPaymentMethod.Vnpay;
                    await _unitOfWork.SaveChangesAsync();
                }

                return new VnPayReturnDto(true, isIpn ? "IPN: Thanh toán thành công" : "Thanh toán thành công");
            }

            return new VnPayReturnDto(false, $"Thanh toán thất bại với mã lỗi: {vnp_ResponseCode}");
        }

        private string CreateSha256Hash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
