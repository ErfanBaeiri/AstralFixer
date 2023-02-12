using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugFixer.Application.Extensions
{
    public static class DateExtensions
    {
        public static string ToShamsi(this DateTime date)
        {
            var persianCalendar = new PersianCalendar();

            return $"{persianCalendar.GetDayOfMonth(date).ToString("00")} / {persianCalendar.GetMonth(date).ToString("00")} / {persianCalendar.GetYear(date)}";
        }
    }
}
