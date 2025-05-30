using System.Globalization;

namespace HOMMS.Common.Helpers
{
    /// <summary>
    /// Helper class for VND (Vietnamese Dong) currency operations
    /// </summary>
    public static class CurrencyHelper
    {
        /// <summary>
        /// Formats a VND amount for display
        /// </summary>
        /// <param name="amount">Amount in VND</param>
        /// <param name="includeCurrencySymbol">Whether to include VND symbol</param>
        /// <returns>Formatted string</returns>
        public static string FormatVND(long amount, bool includeCurrencySymbol = true)
        {
            var formatted = amount.ToString("N0", CultureInfo.GetCultureInfo("vi-VN"));
            return includeCurrencySymbol ? $"{formatted} ₫" : formatted;
        }
        
        /// <summary>
        /// Formats a VND amount for display with custom format
        /// </summary>
        /// <param name="amount">Amount in VND</param>
        /// <returns>Formatted string with thousand separators</returns>
        public static string FormatVNDWithSeparator(long amount)
        {
            return amount.ToString("#,##0") + " VND";
        }
        
        /// <summary>
        /// Validates if an amount is valid for VND currency
        /// </summary>
        /// <param name="amount">Amount to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool IsValidVNDAmount(long amount)
        {
            // VND amounts should be positive and reasonable
            return amount >= 0 && amount <= 999_999_999_999; // Max ~1 trillion VND
        }
        
        /// <summary>
        /// Parses a VND string to long value
        /// </summary>
        /// <param name="vndString">String representation of VND amount</param>
        /// <returns>Parsed amount or null if invalid</returns>
        public static long? ParseVND(string vndString)
        {
            if (string.IsNullOrWhiteSpace(vndString))
                return null;
                
            // Remove common VND symbols and separators
            var cleanString = vndString.Replace("₫", "")
                                      .Replace("VND", "")
                                      .Replace("vnd", "")
                                      .Replace("đ", "")
                                      .Replace(",", "")
                                      .Replace(".", "")
                                      .Trim();
            
            if (long.TryParse(cleanString, out long result) && IsValidVNDAmount(result))
            {
                return result;
            }
            
            return null;
        }
        
        /// <summary>
        /// Common VND denominations for quick selection
        /// </summary>
        public static readonly long[] CommonVNDAmounts = 
        {
            10_000,     // 10k VND
            20_000,     // 20k VND
            50_000,     // 50k VND
            100_000,    // 100k VND
            200_000,    // 200k VND
            500_000,    // 500k VND
            1_000_000,  // 1M VND
            2_000_000,  // 2M VND
            5_000_000   // 5M VND
        };
        
        /// <summary>
        /// Gets friendly display name for common amounts
        /// </summary>
        /// <param name="amount">VND amount</param>
        /// <returns>Friendly name (e.g., "10K", "1M")</returns>
        public static string GetFriendlyAmountName(long amount)
        {
            return amount switch
            {
                >= 1_000_000 when amount % 1_000_000 == 0 => $"{amount / 1_000_000}M",
                >= 1_000 when amount % 1_000 == 0 => $"{amount / 1_000}K",
                _ => FormatVND(amount, false)
            };
        }
    }
} 