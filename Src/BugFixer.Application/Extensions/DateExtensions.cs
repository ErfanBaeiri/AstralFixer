namespace BugFixer.Application.Extensions
{
    public static class DateExtensions
    {
        public static string AsTimeAgo(this DateTime dateTime)
        {
            TimeSpan timeSpan = DateTime.Now.Subtract(dateTime);

            return timeSpan.TotalSeconds switch
            {
                <= 60 => $"{timeSpan.Seconds} ثانیه پیش",

                _ => timeSpan.TotalMinutes switch
                {
                    <= 1 => "حدود یک دقیقه پیش",
                    < 60 => $"حدود {timeSpan.Minutes} دقیقه پیش",
                    _ => timeSpan.TotalHours switch
                    {
                        <= 1 => "حدود یک ساعت پیش",
                        < 24 => $"حدود {timeSpan.Hours} ساعت پش",
                        _ => timeSpan.TotalDays switch
                        {
                            <= 1 => "حدود یک روز پیش",
                            <= 30 => $"حدود {timeSpan.Days} روز پیش",

                            <= 60 => "حدود یک ماه پیش",
                            < 365 => $"حدود {timeSpan.Days / 30} ماه پیش",

                            <= 365 * 1 => "حدود یک سال پیش",
                            _ => $"حدود {timeSpan.Days / 365} سال پیش"
                        }
                    }
                }
            };
        }


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
