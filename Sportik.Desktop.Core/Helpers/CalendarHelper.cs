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

        public static DateTime GetFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).Date;
        }

        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            return GetFirstDayOfMonth(date).AddMonths(1).AddDays(-1);
        }

        public static DateTime GetFirstDayOfYear(DateTime date)
        {
            return new DateTime(date.Year, 1, 1).Date;
        }

        public static DateTime GetLastDayOfYear(DateTime date)
        {
            return GetFirstDayOfYear(date).AddYears(1).AddDays(-1);
        }

        public static int GetWeekDayIndex(DayOfWeek dayOfWeek)
        {
            return (dayOfWeek - DayOfWeek.Monday + 7) % 7;
        }

        public static bool IsBetween(DateTime date, DateTime startDate, DateTime endDate)
        {
            return date.Date >= startDate.Date && date.Date <= endDate.Date;
        }

        public static DateTime Min(DateTime date1, DateTime date2)
        {
            return date1 < date2 ? date1 : date2;
        }

        public static DateTime Max(DateTime date1, DateTime date2)
        {
            return date1 > date2 ? date1 : date2;
        }
    }
}
