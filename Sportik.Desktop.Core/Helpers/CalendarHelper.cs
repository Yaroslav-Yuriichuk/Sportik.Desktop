using System;

namespace Sportik.Desktop.Core.Helpers
{
    public static class CalendarHelper
    {
        public static DateTime GetFirstDayOfWeek(DateTime date)
        {
            int delta = (date.DayOfWeek - DayOfWeek.Monday + 7) % 7;
            return date.AddDays(-delta).Date;
        }

        public static DateTime GetLastDayOfWeek(DateTime date)
        {
            return GetFirstDayOfWeek(date).AddDays(6);
        }

        public static bool IsBeetween(DateTime date, DateTime startDate, DateTime endDate)
        {
            return date.Date >= startDate.Date && date.Date <= endDate.Date;
        }
    }
}
