namespace FamnancesServices.Business
{
    public class DateTimeEast
    {
        public static DateTime Now { get { return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")); } }
    }
}
