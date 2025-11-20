namespace HealthTrack.Core.Utils
{
    public static class DateTimeHelper
    {
        private static readonly TimeZoneInfo BrazilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

        public static DateTime Now => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, BrazilTimeZone);
    }
}