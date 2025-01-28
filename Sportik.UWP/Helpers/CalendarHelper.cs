using System;
using System.Globalization;

namespace Sportik.UWP.Helpers
{
    internal static class CalendarHelper
    {
        public static int WeekOfYear(DateTime date)
        {
            Calendar calendar = CultureInfo.CurrentCulture.Calendar;
            return calendar.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static string DayName(DateTime date)
        {
            return CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(date.DayOfWeek);
        }
    }
}
