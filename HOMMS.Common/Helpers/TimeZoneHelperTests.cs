using System;
using HOMMS.Common.Helpers;

namespace HOMMS.Common.Tests
{
    /// <summary>
    /// Simple tests for TimeZoneHelper to verify the date parsing fix
    /// </summary>
    public static class TimeZoneHelperTests
    {
        public static void RunTests()
        {
            Console.WriteLine("Running TimeZoneHelper Tests...");
            
            TestDateOnlyParsing();
            TestDateTimeParsing();
            TestMenuDateParsing();
            
            Console.WriteLine("All tests completed!");
        }
        
        private static void TestDateOnlyParsing()
        {
            Console.WriteLine("\n--- Testing Date-Only Parsing ---");
            
            var testDate = "2025-08-25";
            var result = TimeZoneHelper.ParseApiDate(testDate);
            
            Console.WriteLine($"Input: {testDate}");
            Console.WriteLine($"Output UTC: {result:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine($"Expected: 2025-08-25 00:00:00 UTC");
            Console.WriteLine($"Test Passed: {result.Date == new DateTime(2025, 8, 25)}");
        }
        
        private static void TestDateTimeParsing()
        {
            Console.WriteLine("\n--- Testing DateTime Parsing ---");
            
            var testDateTime = "2025-08-25T10:30:00";
            var result = TimeZoneHelper.ParseApiDate(testDateTime);
            
            Console.WriteLine($"Input: {testDateTime}");
            Console.WriteLine($"Output UTC: {result:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine($"Expected: Date should be 2025-08-25");
            Console.WriteLine($"Test Passed: {result.Date == new DateTime(2025, 8, 25)}");
        }
        
        private static void TestMenuDateParsing()
        {
            Console.WriteLine("\n--- Testing Menu Date Parsing ---");
            
            var testDate = "2025-08-25";
            var result = TimeZoneHelper.ParseMenuDate(testDate);
            
            Console.WriteLine($"Input: {testDate}");
            Console.WriteLine($"Output UTC: {result:yyyy-MM-dd HH:mm:ss} UTC");
            Console.WriteLine($"Expected: 2025-08-25 00:00:00 UTC");
            Console.WriteLine($"Test Passed: {result.Date == new DateTime(2025, 8, 25)}");
        }
    }
}
