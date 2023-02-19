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
            var persianCalender = new PersianCalendar();

            return $"{persianCalender.GetYear(date)}/{persianCalender.GetMonth(date).ToString("00")}/{persianCalender.GetDayOfMonth(date).ToString("00")}";
        }
    }
}
