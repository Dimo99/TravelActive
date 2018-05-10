using System;

namespace TravelActive.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static bool IsInThisWeek(this DateTime date)
        {
            
            int dayOfTheWeek = (int) DateTime.Now.DayOfWeek;
            if (date.Year == DateTime.Now.Year)
            {
                if (date.DayOfYear >= DateTime.Now.DayOfYear - dayOfTheWeek 
                    && date.DayOfYear <= DateTime.Now.DayOfYear + 6 - dayOfTheWeek)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsInThisMonth(this DateTime date)
        {
            return DateTime.Now.Year == date.Year && DateTime.Now.Month == date.Month;
        }

        public static bool IsInThisYear(this DateTime date)
        {
            return DateTime.Now.Year == date.Year;
        }

        public static bool IsToday(this DateTime date)
        {
            return DateTime.Now.Year == date.Year && DateTime.Now.Month == date.Month && DateTime.Now.Day == date.Day;
        }
    }
}