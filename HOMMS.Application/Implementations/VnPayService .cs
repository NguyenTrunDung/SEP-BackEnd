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
using System.Web;
using System;
using System.Net;
using System.Net.Sockets;

namespace HOMMS.Application.Implementations
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VnPayService(IConfiguration configuration, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GeneratePaymentUrlAsync(int orderId, int amount)
        {
            var vnp_Url = _configuration["VnPay:Url"];
            var vnp_Returnurl = _configuration["VnPay:ReturnUrl"];
            var vnp_IpnUrl = _configuration["VnPay:IpnUrl"];
            var vnp_TmnCode = _configuration["VnPay:TmnCode"];
            var vnp_HashSecret = _configuration["VnPay:HashSecret"];

            string txnRef = orderId.ToString();
            string createDate = DateTime.Now.ToString("yyyyMMddHHmmss");

            // Lấy IP động chuẩn
            string ipAddress = GetIpAddress(_httpContextAccessor?.HttpContext);

            var vnp_Params = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", vnp_TmnCode },
                { "vnp_Amount", (amount * 100).ToString() },
                { "vnp_CreateDate", createDate },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", ipAddress },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", $"Thanh toan don hang {orderId}" },
                { "vnp_OrderType", "other" },
                { "vnp_ReturnUrl", vnp_Returnurl },
                { "vnp_TxnRef", txnRef }
            };
            
            // Chỉ thêm IPN URL nếu có cấu hình
            if (!string.IsNullOrEmpty(vnp_IpnUrl))
            {
                vnp_Params.Add("vnp_IpnUrl", vnp_IpnUrl);
            }
            // Nếu có bankCode thì mới add
            string bankCode = null; // hoặc lấy từ request nếu có
            if (!string.IsNullOrEmpty(bankCode))
                vnp_Params.Add("vnp_BankCode", bankCode);

            // Validate parameters before creating signature
            if (!ValidateVnPayParameters(vnp_Params))
            {
                throw new InvalidOperationException("Invalid VNPay parameters");
            }

            // Tạo rawData để ký: URL-encode cả key và value, chỉ nối các param có value khác rỗng
            var rawData = string.Join("&", vnp_Params
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));
            Console.WriteLine($"[VnPayService] Raw data for signing (URL-encoded): {rawData}");
            Console.WriteLine($"[VnPayService] Hash secret: {vnp_HashSecret}");

            // Tạo secure hash HMAC SHA512
            var hash = HmacSHA512(vnp_HashSecret, rawData);
            Console.WriteLine($"[VnPayService] Generated hash: {hash}");

            // Add security parameters
            vnp_Params.Add("vnp_SecureHash", hash);

            // Build final payment URL: encode key và value, chỉ nối các param có value khác rỗng
            var urlParams = string.Join("&", vnp_Params
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));
            var paymentUrl = $"{vnp_Url}?{urlParams}";
            Console.WriteLine($"[VnPayService] Final payment URL: {paymentUrl}");

            return await Task.FromResult(paymentUrl);
        }

        public async Task<VnPayReturnDto> ProcessVnPayReturnAsync(IQueryCollection query, bool isIpn = false)
        {
            var vnp_HashSecret = _configuration["VnPay:HashSecret"];
            var vnp_SecureHash = query["vnp_SecureHash"].ToString();
            var vnp_ResponseCode = query["vnp_ResponseCode"].ToString();
            var vnp_TxnRef = query["vnp_TxnRef"].ToString();
            var vnp_TransactionNo = query["vnp_TransactionNo"].ToString();
            var vnp_Amount = query["vnp_Amount"].ToString();

            // Bước 1: Lọc và sắp xếp tham số theo VNPay specification
            var filtered = query
                .Where(kv => kv.Key != "vnp_SecureHash" && kv.Key != "vnp_SecureHashType")
                .ToDictionary(kv => kv.Key, kv => kv.Value.ToString());

            var sorted = new SortedDictionary<string, string>(filtered);

            // Bước 2: Tạo chuỗi raw data để verify signature (URL-encoded, không có param rỗng)
            var rawData = string.Join("&", sorted
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));
            Console.WriteLine($"[VnPayService] Return verification - Raw data (URL-encoded): {rawData}");
            Console.WriteLine($"[VnPayService] Return verification - Received hash: {vnp_SecureHash}");

            var calculatedHash = HmacSHA512(vnp_HashSecret, rawData);
            Console.WriteLine($"[VnPayService] Return verification - Calculated hash: {calculatedHash}");

            // Bước 2: So sánh hash để xác minh
            if (!string.Equals(calculatedHash, vnp_SecureHash, StringComparison.OrdinalIgnoreCase))
                return new VnPayReturnDto(false, "Sai chữ ký (Secure Hash không hợp lệ)", null, vnp_TransactionNo, null, vnp_ResponseCode);

            // Bước 3: Kiểm tra mã phản hồi từ VNPAY
            if (vnp_ResponseCode == "00")
            {
                if (!int.TryParse(vnp_TxnRef, out var orderId))
                    return new VnPayReturnDto(false, "Mã đơn hàng không hợp lệ", null, vnp_TransactionNo, null, vnp_ResponseCode);

                var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId);
                if (order == null)
                    return new VnPayReturnDto(false, "Không tìm thấy đơn hàng", orderId, vnp_TransactionNo, null, vnp_ResponseCode);

                // Bước 4: Cập nhật trạng thái thanh toán
                if (!order.IsPaid) // tránh cập nhật lại đơn đã thanh toán
                {
                    order.IsPaid = true; // Cập nhật trạng thái đơn hàng
                    order.Status = "Completed";
                    order.PaymentMethod = OrderPaymentMethod.Vnpay;
                    await _unitOfWork.SaveChangesAsync();
                }

                // Parse amount for return
                long.TryParse(vnp_Amount, out var amount);

                return new VnPayReturnDto(
                    true, 
                    isIpn ? "IPN: Thanh toán thành công" : "Thanh toán thành công",
                    orderId,
                    vnp_TransactionNo,
                    amount,
                    vnp_ResponseCode
                );
            }

            // Parse values for failed case
            int.TryParse(vnp_TxnRef, out var failedOrderId);
            long.TryParse(vnp_Amount, out var failedAmount);

            return new VnPayReturnDto(
                false, 
                $"Thanh toán thất bại với mã lỗi: {vnp_ResponseCode}",
                failedOrderId == 0 ? null : failedOrderId,
                vnp_TransactionNo,
                failedAmount == 0 ? null : failedAmount,
                vnp_ResponseCode
            );
        }

        private string CreateSha256Hash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower(); // VNPay accepts both upper/lower, keeping lowercase for consistency
        }

        /// <summary>
        /// Alternative SHA256 hash method following exact VNPay specification
        /// </summary>
        private string CreateSha256HashVnPay(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes).ToUpper(); // VNPay expects uppercase hash
        }

        /// <summary>
        /// Original SHA256 hash method for comparison
        /// </summary>
        private string CreateSha256HashOriginal(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
        }

        /// <summary>
        /// Build payment URL with proper URL encoding according to VNPay specification
        /// </summary>
        private string BuildPaymentUrl(string baseUrl, SortedDictionary<string, string> parameters)
        {
            var queryParams = new List<string>();
            
            foreach (var param in parameters)
            {
                // URL encode the value according to VNPay specification
                var encodedValue = HttpUtility.UrlEncode(param.Value, Encoding.UTF8);
                queryParams.Add($"{param.Key}={encodedValue}");
            }
            
            var queryString = string.Join("&", queryParams);
            return $"{baseUrl}?{queryString}";
        }

        /// <summary>
        /// Validate VNPay parameters according to specification
        /// </summary>
        private bool ValidateVnPayParameters(SortedDictionary<string, string> parameters)
        {
            // Required parameters according to VNPay specification
            var requiredParams = new[] { "vnp_Amount", "vnp_Command", "vnp_CreateDate", "vnp_CurrCode", "vnp_IpAddr", "vnp_Locale", "vnp_OrderInfo", "vnp_OrderType", "vnp_ReturnUrl", "vnp_TmnCode", "vnp_TxnRef", "vnp_Version" };
            
            foreach (var required in requiredParams)
            {
                if (!parameters.ContainsKey(required) || string.IsNullOrEmpty(parameters[required]))
                {
                    Console.WriteLine($"[VnPayService] Missing required parameter: {required}");
                    return false;
                }
            }
            
            // Optional but recommended parameters
            var optionalParams = new[] { "vnp_BankCode", "vnp_IpnUrl" };
            foreach (var optional in optionalParams)
            {
                if (!parameters.ContainsKey(optional))
                {
                    Console.WriteLine($"[VnPayService] Missing optional parameter: {optional}");
                }
            }
            
            return true;
        }

        private string GetIpAddress(HttpContext context)
        {
            var ipAddress = string.Empty;
            try
            {
                var remoteIpAddress = context.Connection.RemoteIpAddress;

                if (remoteIpAddress != null)
                {
                    if (remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        remoteIpAddress = Dns.GetHostEntry(remoteIpAddress).AddressList
                            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
                    }

                    if (remoteIpAddress != null) ipAddress = remoteIpAddress.ToString();

                    return ipAddress;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return "127.0.0.1";
        }

        private string HmacSHA512(string key, string inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }
            return hash.ToString();
        }
    }
}
