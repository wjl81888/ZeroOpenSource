using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralExtensions.DateExtension
{
    public static class DateTimeExtensions
    {
        public static string TimeAgo(this DateTime dateTime)
        {
            var timeSpan = DateTime.Now.Subtract(dateTime);

            string result;

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0}秒前", timeSpan.Seconds);
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = string.Format("{0}分钟前", timeSpan.Minutes);
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = string.Format("{0}小时前", timeSpan.Hours);
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = string.Format("{0}天前", timeSpan.Days);
            }
            else
            {
                result = $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}";
            }

            return result;
        }
    }
}
