namespace BugFixer.Application.Extensions
{
    public static class DateExtensions
    {
        public static string ToPersianDate(this DateTime dateTime)
        {
            // This method converts a DateTime to a Persian date string.
            // It uses the PersianCalendar to format the date.
            var persianCalendar = new System.Globalization.PersianCalendar();
            return $"{persianCalendar.GetYear(dateTime)}/{persianCalendar.GetMonth(dateTime):00}/{persianCalendar.GetDayOfMonth(dateTime):00}";
        }

        public static DateTime ToMiladi(this string persianDate)
        {
            var parts = persianDate.Split('/');

            var year = parts[0];
            var month = parts[1];
            var day = parts[2];

            return new DateTime(int.Parse(year), int.Parse(month), int.Parse(day), new System.Globalization.PersianCalendar());
        }
    }
}
